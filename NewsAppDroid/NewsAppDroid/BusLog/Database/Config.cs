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
#if MONODROID
using Android.Database;
using Android.Database.Sqlite;
#endif



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
		//object context;
		public Config(object context)
		{
		//	this.context = context;
		}


		public class FeedSettings
		{
			public int FeedID { get; set; }
			public string Title { get; set; }
			public bool IsActive { get; set; }
		}


		public AppConfig GetAppConfig()
		{
			AppConfig ret = new AppConfig();
			string sqlCommand = "SELECT AppIsConfigured, DateIndicate, DataAutomaticUpdate FROM config Limit 1;";

#if MONODROID
			if ((int)Android.OS.Build.VERSION.SdkInt < 77)
			{
				/*
				 * http://pastebin.com/tNPmzXND
				 * http://www.c-sharpcorner.com/UploadFile/88b6e5/sqlitedatabase-connectivity/
				 * 
				try
				{
					DBHelper dbHelper = new DBHelper((Android.Content.Context)context);
					Android.Database.Sqlite.SQLiteDatabase sqlDB = dbHelper.ReadableDatabase;


					Cursor reader = sqlDB.RawQuery(sqlCommand, null);
					while(result.moveToNext())
					{
						ret.AppIsConfigured = reader.GetBoolean(0);
						ret.DateIndicate = reader.GetBoolean(1);
						ret.DataAutomaticUpdate = reader.GetBoolean(2);
					}

					dbHelper.Close();
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(this.GetType() + ".GetAppConfig() - ex: " + ex.ToString());
				}

				return ret;
				*/
			}
#endif

			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					using(DbCommand c = conn.CreateCommand())
					{
						using (SqliteCommand sqlCmd = new SqliteCommand(sqlCommand, conn))
						{
							conn.Open();
							
							using (DbDataReader reader = sqlCmd.ExecuteReader())
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
				}
			}
			catch(SqliteException ex)
			{
				System.Diagnostics.Debug.WriteLine(this.GetType() + ".GetAppConfig() - ex: " + ex.ToString());
			}

			return ret;
		}
		
		
		public void SetAppConfig(AppConfig config)
		{
			if (config == null)
				return;
			
			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					using(DbCommand c = conn.CreateCommand())
					{
						using (SqliteCommand sqlCmd = new SqliteCommand("UPDATE config SET AppIsConfigured=1, DateIndicate = @DateIndicate, DataAutomaticUpdate = @DataAutomaticUpdate;", conn))
						{
							sqlCmd.Parameters.AddWithValue("@DateIndicate", config.DateIndicate);
							sqlCmd.Parameters.AddWithValue("@DataAutomaticUpdate", config.DataAutomaticUpdate);

							conn.Open();
							sqlCmd.ExecuteNonQuery();
							conn.Close();
						}
					}
				}
			}
			catch(SqliteException ex)
			{
				System.Diagnostics.Debug.WriteLine(this.GetType() + ".SetAppConfig() - ex: " + ex.ToString());
			}
		}
		
		
		public List<WSFeedConfig.FeedConfig> GetWSConfig()
		{
			List<WSFeedConfig.FeedConfig> ret = new List<WSFeedConfig.FeedConfig>();
			
			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					using(DbCommand c = conn.CreateCommand())
					{
						using (SqliteCommand sqlCmd = new SqliteCommand("SELECT FeedID, IsActive, Name, FeedType, URL, URLType, CategoryFilter FROM feedconfig ORDER BY Name, FeedType, CategoryFilter;", conn))
						{
							conn.Open();

							using (DbDataReader reader = sqlCmd.ExecuteReader())
							{
								while (reader.Read())
								{
									if (reader.HasRows)
									{
										ret.Add(new WSFeedConfig.FeedConfig()
										{
											FeedID = reader.GetInt32(0),
											IsActive = reader.GetBoolean(1),
											Name = reader.GetString(2),
											FeedType = (WSFeedConfig.FeedTypes)reader.GetInt32(3),
											Url = reader.GetString(4),
											UrlType = (WSFeedConfig.UrlTypes)reader.GetInt32(5),
											CategoryFilter = (!reader.IsDBNull(6)? reader.GetString(6) : null)
										});
									}
								}
							}
								
							conn.Close();
						}
					}
				}
			}
			catch(SqliteException ex)
			{
				System.Diagnostics.Debug.WriteLine(this.GetType() + ".GetWSConfig() - ex: " + ex.ToString());
			}
			
			return ret;
		}
		
		
		public void SetWSConfig(List<WSFeedConfig.FeedConfig> feedsConfig)
		{
			if (feedsConfig == null)
				return;
			
			List<WSFeedConfig.FeedConfig> dbFeedsConfig = GetWSConfig();

			List<SqliteCommand> sqlCmds = new List<SqliteCommand>();

			try
			{
				using(SqliteConnection conn = GetConnection())
				{
					// Feeds die noch nicht in der DB sind hinzufügen
					foreach(WSFeedConfig.FeedConfig feedConfig in feedsConfig)
					{
						if (!dbFeedsConfig.Exists(p => p.Url == feedConfig.Url && p.CategoryFilter == feedConfig.CategoryFilter))
						{
							SqliteCommand sqlCmd = new SqliteCommand("INSERT INTO feedconfig (IsActive, Name, FeedType, URL, URLType, CategoryFilter) VALUES (0, @Name, @FeedType, @Url, @UrlType, @CategoryFilter);", conn);
							sqlCmd.Parameters.AddWithValue("@Name", feedConfig.Name);
							sqlCmd.Parameters.AddWithValue("@FeedType", (int)feedConfig.FeedType);
							sqlCmd.Parameters.AddWithValue("@Url", feedConfig.Url);
							sqlCmd.Parameters.AddWithValue("@UrlType", (int)feedConfig.UrlType);
							sqlCmd.Parameters.AddWithValue("@CategoryFilter", feedConfig.CategoryFilter);

							sqlCmds.Add(sqlCmd);
						}
						else
						{
							SqliteCommand sqlCmd = new SqliteCommand("UPDATE feedconfig SET IsActive=@IsActive WHERE FeedID=@FeedID;", conn);
							sqlCmd.Parameters.AddWithValue("@IsActive", feedConfig.IsActive);
							sqlCmd.Parameters.AddWithValue("@FeedID", feedConfig.FeedID);

							sqlCmds.Add(sqlCmd);
						}
					}
					
					// Einträge die es nicht mehr gibt entfernen.
					foreach(WSFeedConfig.FeedConfig dbFeed in dbFeedsConfig)
					{
						if (!feedsConfig.Exists(p => p.Url == dbFeed.Url && p.CategoryFilter == dbFeed.CategoryFilter))
						{
							SqliteCommand sqlCmd = new SqliteCommand("DELETE FROM feedconfig WHERE URL=@Url AND CategoryFilter=@CategoryFilter;", conn);
							sqlCmd.Parameters.AddWithValue("@Url", dbFeed.Url);
							sqlCmd.Parameters.AddWithValue("@CategoryFilter", dbFeed.CategoryFilter);

							sqlCmds.Add(sqlCmd);
						}
					}
					
					if (sqlCmds.Count > 0)
					{
						conn.Open();

						foreach(SqliteCommand sqlCmd in sqlCmds)
							sqlCmd.ExecuteNonQuery();

						conn.Close();
					}
				}
			}
			catch(SqliteException ex)
			{
				System.Diagnostics.Debug.WriteLine(this.GetType() + ".SetWSConfig() - ex: " + ex.ToString());
			}	

		}
	}
}

