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
using Mono.Data.Sqlite;
using System.Data.Common;
using System.Collections.Generic;
using de.dhoffmann.mono.adfcnewsapp.buslog.webservice;
using System.Text;

namespace de.dhoffmann.mono.adfcnewsapp.buslog.database
{
	public class AppConfig
	{
		public bool AppIsConfigured { get; set; }
		public bool DateIndicate { get; set; }
		public bool DataAutomaticUpdate { get; set; }
	}
	
	public class Config : DataBase
	{
		public Config ()
		{
		}
		
		public AppConfig GetAppConfig()
		{
			AppConfig ret = new AppConfig();
			
			using(SqliteConnection conn = GetConnection())
			{
				using(DbCommand c = conn.CreateCommand())
				{
					c.CommandText = "SELECT AppIsConfigured, DateIndicate, DataAutomaticUpdate FROM config Limit 1;";
					c.CommandType = System.Data.CommandType.Text;
					conn.Open();
					
					using (DbDataReader reader = c.ExecuteReader())
					{
						// Es gibt nur eine letzte Version
						reader.Read();
						
						if (reader.HasRows)
						{
							ret.AppIsConfigured = reader.GetBoolean(0);
							ret.DateIndicate = reader.GetBoolean(1);
							ret.DataAutomaticUpdate = reader.GetBoolean(2);
						}
					}
					
					conn.Close();
				}
			}
			
			return ret;
		}
		
		
		public void SetAppConfig(AppConfig config)
		{
			if (config == null)
				return;
			
			using(SqliteConnection conn = GetConnection())
			{
				using(DbCommand c = conn.CreateCommand())
				{
					c.CommandText = "UPDATE config SET AppIsConfigured=1, DateIndicate=" + (config.DateIndicate? "1" : "0") + ", DataAutomaticUpdate=" + (config.DataAutomaticUpdate? "1" : "0") + ";";
					c.CommandType = System.Data.CommandType.Text;
					conn.Open();
					c.ExecuteNonQuery();
					
					conn.Close();
				}
			}
		}
		
		
		public List<WSFeedConfig.FeedConfig> GetWSConfig()
		{
			List<WSFeedConfig.FeedConfig> ret = new List<WSFeedConfig.FeedConfig>();
			
			using(SqliteConnection conn = GetConnection())
			{
				using(DbCommand c = conn.CreateCommand())
				{
					conn.Open();
					
					c.CommandText = "SELECT Name, FeedType, URL, URLType, ShowCategory FROM feedconfig;";
					c.CommandType = System.Data.CommandType.Text;
					
					using (DbDataReader reader = c.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader.HasRows)
							{
								try {
								ret.Add(new WSFeedConfig.FeedConfig()
								{
									Name = reader.GetString(0),
									FeedType = (WSFeedConfig.FeedTypes)reader.GetInt32(1),
									Url = reader.GetString(2),
									UrlType = (WSFeedConfig.UrlTypes)reader.GetInt32(3),
									ShowCategory = (!reader.IsDBNull(4)? reader.GetString(4) : null)
								});
								}catch(Exception e) {
									int i = 0;i++;}
							}
						}
					}
						
					conn.Close();
				}
			}
			
			return ret;
		}
		
		
		public void SetWSConfig(List<WSFeedConfig.FeedConfig> feedsConfig)
		{
			if (feedsConfig == null)
				return;
			
			List<WSFeedConfig.FeedConfig> dbFeedsConfig = GetWSConfig();
			
			StringBuilder commands = new StringBuilder();
			
			// Feeds die noch nicht in der DB sind hinzufügen
			foreach(WSFeedConfig.FeedConfig feedConfig in feedsConfig)
			{
				if (!dbFeedsConfig.Exists(p => p.Url == feedConfig.Url && p.ShowCategory == feedConfig.ShowCategory))
					commands.AppendLine("INSERT INTO feedconfig (Name, FeedType, URL, URLType, ShowCategory) VALUES ('" + feedConfig.Name + "', " + (int)feedConfig.FeedType + ", '" + feedConfig.Url + "', " + (int)feedConfig.UrlType + ", " + (!String.IsNullOrEmpty(feedConfig.ShowCategory)? "'" + feedConfig.ShowCategory + "'" : "NULL") + ");");
			}
			
			// Einträge die es nicht mehr gibt entfernen.
			foreach(WSFeedConfig.FeedConfig dbFeed in dbFeedsConfig)
			{
				if (!feedsConfig.Exists(p => p.Url == dbFeed.Url && p.ShowCategory == dbFeed.ShowCategory))
					commands.AppendLine("DELETE FROM feedconfig WHERE URL='" + dbFeed.Url + "' AND ShowCategory='" + dbFeed.ShowCategory + "';");
			}
			
			if (commands.Length > 0)
			{
				using(SqliteConnection conn = GetConnection())
				{
					conn.Open();
					
					using(DbCommand c = conn.CreateCommand())
					{
						c.CommandText = commands.ToString();
						c.CommandType = System.Data.CommandType.Text;
						c.ExecuteNonQuery();
					}
					
					conn.Close();
				}
			}
		}
	}
}

