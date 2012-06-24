/*
 * This file is part of ADFC-NewsApp
 * Copyright (C) 2012 David Hoffmann
 *
 * ADFC-NewsApp is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, version 2.
 *
 * ADFC-NewsApp is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ADFC-NewsApp; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */


using System;
using de.dhoffmann.mono.adfcnewsapp.buslog.feedimport;
using System.Data.Common;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace de.dhoffmann.mono.adfcnewsapp.buslog.database
{
	public class Rss : DataBase
	{
		public Rss ()
		{
		}
		
		
		public feedimport.Rss.RssFeed GetRssFeed(int feedID)
		{
			feedimport.Rss.RssFeed ret = new de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssFeed();
			
			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					using(DbCommand c = conn.CreateCommand())
					{
						c.CommandText = "SELECT FeedID, Title, Link, Description, LastBuildDate FROM rssfeeds WHERE FeedID=" + feedID + " LIMIT 1;";
						c.CommandType = System.Data.CommandType.Text;
						conn.Open();
						
						using (DbDataReader reader = c.ExecuteReader())
						{
							reader.Read();
							
							if (reader.HasRows)
							{
								feedimport.Rss.RssHeader rssHeader = new de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssHeader();
								rssHeader.FeedID = reader.GetInt32(0);
								
								if (!reader.IsDBNull(1))
									rssHeader.Title = reader.GetString(1);
								
								if (!reader.IsDBNull(2))
									rssHeader.Link = reader.GetString(2);
								
								if (!reader.IsDBNull(3))
									rssHeader.Description = reader.GetString(3);
								
								if (!reader.IsDBNull(4))
									rssHeader.LastBuildDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(reader.GetInt64(4)));
								
								ret.Header = rssHeader;
							}
						}
						
						conn.Close();
					}
					
					if (ret.Header != null)
					{
					
						using(DbCommand c = conn.CreateCommand())
						{
							c.CommandText = "SELECT ItemID, FeedID, Title, Link, Description, Category, PubDate, IsRead FROM rssfeeditem WHERE FeedID=" + feedID + " ORDER BY PubDate;";
							c.CommandType = System.Data.CommandType.Text;
							conn.Open();
							
							using (DbDataReader reader = c.ExecuteReader())
							{
								List<feedimport.Rss.RssItem> rssItems = new List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem>();
								
								while(reader.Read())
								{
									feedimport.Rss.RssItem rssItem = new de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem();
									
									if (reader.HasRows)
									{
										rssItem.ItemID = reader.GetInt32(0);
										rssItem.FeedID = reader.GetInt32(1);
										
										if (!reader.IsDBNull(2))
											rssItem.Title = reader.GetString(2);
										
										if (!reader.IsDBNull(3))
											rssItem.Link = reader.GetString(3);
										
										if (!reader.IsDBNull(4))
											rssItem.Description = reader.GetString(4);
										
										if (!reader.IsDBNull(5))
											rssItem.Category = reader.GetString(5);
										
										if (!reader.IsDBNull(6))
											rssItem.PubDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(reader.GetInt64(6)));

										rssItem.IsRead = reader.GetBoolean(7);
									}
									
									rssItems.Add(rssItem);
								}
								
								ret.Items = rssItems;
							}
							
							conn.Close();
						}
					}
				}
			}
			catch(SqliteException ex)
			{
				System.Diagnostics.Debug.WriteLine(this.GetType() + ".GetRssFee() - ex: " + ex.ToString());
			}
			
			return ret;
		}

		public bool SetRssFeed (feedimport.Rss.RssFeed rssFeed)
		{
			bool ret = false;
			
			// Ohne Header kann ein RssFeed nicht importiert werden.
			if (rssFeed == null || rssFeed.Header == null)
				return true;
			
			feedimport.Rss.RssFeed dbRssFeed = GetRssFeed(rssFeed.Header.FeedID);
			
			using(SqliteConnection conn = GetConnection())
			{
				try
				{
					// Header anlegen
					if (dbRssFeed == null || dbRssFeed.Header == null)
					{
						using(DbCommand c = conn.CreateCommand())
						{
							c.CommandText = "INSERT INTO rssfeeds (FeedID, DateCreate, Title, Link, Description, LastBuildDate) VALUES (" + 
								rssFeed.Header.FeedID + ", " +
								"date('now'), " + 
								(!String.IsNullOrEmpty(rssFeed.Header.Title)? "'" + rssFeed.Header.Title + "'" : "'Kein Titel'") + ", " +
								(!String.IsNullOrEmpty(rssFeed.Header.Link)? "'" + rssFeed.Header.Link + "'" : "NULL") + ", " +
								(!String.IsNullOrEmpty(rssFeed.Header.Description)? "'" + rssFeed.Header.Description + "'" : "NULL") + ", " +
								((rssFeed.Header.LastBuildDate.HasValue)? Convert.ToInt64(rssFeed.Header.LastBuildDate.Value.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString() : "NULL") +
								");";
							c.CommandType = System.Data.CommandType.Text;
							conn.Open();
							c.ExecuteNonQuery();
							conn.Close();
						}
					}
					
					// Items anlegen
					if (rssFeed.Items != null && rssFeed.Items.Count > 0)
					{
						StringBuilder commands = new StringBuilder();
						
						foreach (feedimport.Rss.RssItem row in rssFeed.Items)
						{
							bool fInsertRow = true;
							
							if (dbRssFeed != null && dbRssFeed.Items != null && dbRssFeed.Items.Count > 0)
							{
								fInsertRow = (from r in dbRssFeed.Items
									where r.FeedID == row.FeedID && r.Link == row.Link
								    select r.ItemID).Count() == 0;
							}
							
							// Zeile in die DB eintragen
							if (fInsertRow)
							{
								commands.AppendLine("INSERT INTO rssfeeditem (FeedID, DateCreate, Title, Link, Description, Category, PubDate, IsRead) VALUES (" + 
									row.FeedID + ", " +
								    Convert.ToInt64(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString() + ", " +
								    (!String.IsNullOrEmpty(row.Title)? "'" + row.Title + "'" : "'Kein Titel'") + ", " +
								    (!String.IsNullOrEmpty(row.Link)? "'" + row.Link + "'" : "NULL") + ", " +
								    (!String.IsNullOrEmpty(row.Description)? "'" + row.Description + "'" : "NULL") + ", " +
								    (!String.IsNullOrEmpty(row.Category)? "'" + row.Category + "'" : "NULL") + ", " +
								    (!row.PubDate.HasValue? "'" + row.PubDate.ToString() + "'" : "NULL") + ", " +
								    "0 " +
								    ");");
							}
						}
						
						if (commands != null && commands.Length > 0)
						{
							using(DbCommand c = conn.CreateCommand())
							{
								c.CommandText = commands.ToString();
								c.CommandType = System.Data.CommandType.Text;
								conn.Open();
								c.ExecuteNonQuery();
								conn.Close();
							}
						}
					}
					
					ret = true;
						
					// TODO: Items die gelesen und älter als 365 Tage sind aus der DB entfernen
					// Die Daten erst spät entfernen, da sie sonst als neue nicht gelesene wieder angezeigt werden.
					
				}
				catch(SqliteException ex)
				{
					System.Diagnostics.Debug.WriteLine("SqlEx SetRssFeed - ex: " + ex.ToString());
				}
			}					
			
			return ret;
		}		

		public DateTime? GetLastBuildDate (int feedID)
		{
			DateTime? ret = null;

			feedimport.Rss.RssFeed rssFeed = GetRssFeed(feedID);
			
			if (rssFeed != null && rssFeed.Header != null && rssFeed.Header.LastBuildDate.HasValue)
				ret = rssFeed.Header.LastBuildDate.Value;
			
			return ret;
		}


		public List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem> GetActiveFeedItems (bool showOnlyUnReadFeedItems)
		{
			List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem> ret = new List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem>();

			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					using(DbCommand c = conn.CreateCommand())
					{
						c.CommandText = "SELECT ItemID, rssfeeditem.FeedID, Title, Link, Description, Category, PubDate, IsRead " +
							"FROM rssfeeditem " +
							"INNER JOIN feedconfig ON (rssfeeditem.FeedID = feedconfig.FeedID) " +
							"WHERE feedConfig.IsActive = 1 " +
							(showOnlyUnReadFeedItems? "rssfeeditem.IsRead = 0 " : "") +
							"ORDER BY PubDate;";
						c.CommandType = System.Data.CommandType.Text;
						conn.Open();
						
						using (DbDataReader reader = c.ExecuteReader())
						{
							while(reader.Read())
							{
								feedimport.Rss.RssItem rssItem = new de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem();
								
								if (reader.HasRows)
								{
									rssItem.ItemID = reader.GetInt32(0);
									rssItem.FeedID = reader.GetInt32(1);
									
									if (!reader.IsDBNull(2))
										rssItem.Title = reader.GetString(2);
									
									if (!reader.IsDBNull(3))
										rssItem.Link = reader.GetString(3);
									
									if (!reader.IsDBNull(4))
										rssItem.Description = reader.GetString(4);
									
									if (!reader.IsDBNull(5))
										rssItem.Category = reader.GetString(5);
									
									if (!reader.IsDBNull(6))
										rssItem.PubDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(reader.GetInt64(6)));

									rssItem.IsRead = reader.GetBoolean(7);
								}
								
								ret.Add(rssItem);
							}

							conn.Close();
						}
					}
				}
			}
			catch(SqliteException ex)
			{
				System.Diagnostics.Debug.WriteLine(this.GetType().Name + ".GetActiveFeedItems() - ex: " + ex.ToString());
			}

			return  ret;
		}

	}
}

