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
using de.dhoffmann.mono.adfcnewsapp.droid.buslog;


namespace de.dhoffmann.mono.adfcnewsapp.buslog.database
{
	public class Rss : DataBase
	{
		public Rss ()
		{
		}
		
		
		public feedimport.Rss.RssFeed GetRssFeed(int feedID, int? feedItemID)
		{
			feedimport.Rss.RssFeed ret = new de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssFeed();
			
			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					using(SqliteCommand sqlCmd = new SqliteCommand("SELECT FeedID, Title, Link, Description, LastBuildDate FROM rssfeeds WHERE FeedID=@FeedID LIMIT 1;", conn))
					{
						sqlCmd.Parameters.AddWithValue("@FeedID", feedID);

						conn.Open();
						
						using (DbDataReader reader = sqlCmd.ExecuteReader())
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
									rssHeader.LastBuildDate = reader.GetDateTime(4);
								
								ret.Header = rssHeader;
							}
						}
						
						conn.Close();
					}

					
					if (ret.Header != null)
					{
						using(SqliteCommand sqlCmd2 = new SqliteCommand("SELECT ItemID, FeedID, Title, Link, Description, Category, PubDate, IsRead FROM rssfeeditem WHERE FeedID=@FeedID " +
						                                                (feedItemID.HasValue? "AND ItemID=@FeedItemID " : "") +
						                                                "ORDER BY PubDate DESC;", conn))
						{
							sqlCmd2.Parameters.AddWithValue("@FeedID", feedID);

							if (feedItemID.HasValue)
								sqlCmd2.Parameters.AddWithValue("@FeedItemID", feedItemID);

							conn.Open();
							
							using (DbDataReader reader = sqlCmd2.ExecuteReader())
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
											rssItem.PubDate = reader.GetDateTime(6);

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
			
			feedimport.Rss.RssFeed dbRssFeed = GetRssFeed(rssFeed.Header.FeedID, null);
			
			using(SqliteConnection conn = GetConnection())
			{
				try
				{
					// Header anlegen
					if (dbRssFeed == null || dbRssFeed.Header == null)
					{
						using(SqliteCommand sqlCmd = new SqliteCommand("INSERT INTO rssfeeds (FeedID, DateCreate, Title, Link, Description, LastBuildDate) VALUES (" + 
						                                         "@FeedID, date('now'), @Title, @Link, @Description, @LastBuildDate);", conn))
						{
							sqlCmd.Parameters.AddWithValue("@FeedID", rssFeed.Header.FeedID);
							sqlCmd.Parameters.AddWithValue("@Title", (!String.IsNullOrEmpty(rssFeed.Header.Title)? rssFeed.Header.Title : "Kein Titel"));
							sqlCmd.Parameters.AddWithValue("@Link", (!String.IsNullOrEmpty(rssFeed.Header.Link)? rssFeed.Header.Link : null));
							sqlCmd.Parameters.AddWithValue("@Description", (!String.IsNullOrEmpty(rssFeed.Header.Description)? rssFeed.Header.Description : null));

							if (rssFeed.Header.LastBuildDate.HasValue)
								sqlCmd.Parameters.AddWithValue("@LastBuildDate", rssFeed.Header.LastBuildDate.Value);

							conn.Open();
							sqlCmd.ExecuteNonQuery();
							conn.Close();
						}
					}
					
					// Items anlegen
					if (rssFeed.Items != null && rssFeed.Items.Count > 0)
					{
						List<SqliteCommand> sqlCmds = new List<SqliteCommand>();

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
								SqliteCommand sqlCmd = new SqliteCommand("INSERT INTO rssfeeditem (FeedID, DateCreate, Title, Link, Description, Category, PubDate, IsRead) VALUES (" + 
								                                         "@FeedID, @DateCreate, @Title, @Link, @Description, @Category, @PubDate, @IsRead)", conn);
								sqlCmd.Parameters.AddWithValue("@FeedID", row.FeedID);
								sqlCmd.Parameters.AddWithValue("@DateCreate", DateTime.Now);
								sqlCmd.Parameters.AddWithValue("@Title", (!String.IsNullOrEmpty(row.Title)? row.Title : "Kein Titel"));
								sqlCmd.Parameters.AddWithValue("@Link", (!String.IsNullOrEmpty(row.Link)? row.Link : null));
								sqlCmd.Parameters.AddWithValue("@Description", (!String.IsNullOrEmpty(row.Description)? row.Description : null));
								sqlCmd.Parameters.AddWithValue("@Category", (!String.IsNullOrEmpty(row.Category)? row.Category : null));

								if (row.PubDate.HasValue)
									sqlCmd.Parameters.AddWithValue("@PubDate", row.PubDate.Value);

								sqlCmd.Parameters.AddWithValue("@IsRead", 0);

								sqlCmds.Add(sqlCmd);
							}
						}
						
						if (sqlCmds != null && sqlCmds.Count > 0)
						{
							conn.Open();
							foreach(SqliteCommand cmd in sqlCmds)
								cmd.ExecuteNonQuery();
							conn.Close();
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

			feedimport.Rss.RssFeed rssFeed = GetRssFeed(feedID, null);
			
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
							(showOnlyUnReadFeedItems? "AND rssfeeditem.IsRead = 0 " : "") +
							"ORDER BY PubDate DESC;";
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
									{
										// TODO prüfen
										try
										{
											rssItem.PubDate = reader.GetDateTime(6);
										}
										catch(Exception)
										{}
									}
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
				Logging.Log(this, Logging.LoggingTypeError, "SQLException", ex);
			}

			return  ret;
		}


		public void MarkItemsAsRead (int? feedItemID, bool markAsRead)
		{
			try 
			{
				using (SqliteConnection conn = GetConnection()) 
				{
					using (SqliteCommand sqlCmd = new SqliteCommand("UPDATE rssfeeditem SET IsRead=@IsRead " +
					                                                (feedItemID.HasValue? "WHERE ItemID=@ItemID;" : ";"), conn))
					{
						if (feedItemID.HasValue)
							sqlCmd.Parameters.AddWithValue ("@ItemID", feedItemID);

						sqlCmd.Parameters.AddWithValue ("@IsRead", markAsRead);

						conn.Open ();
						sqlCmd.ExecuteNonQuery();
						conn.Close();
					}
				}
			} 
			catch (SqliteException ex) 
			{
				System.Diagnostics.Debug.WriteLine(this.GetType().Name + ".MarkItemAsRead() - ex: " + ex.ToString());
			}
		}
	}
}

