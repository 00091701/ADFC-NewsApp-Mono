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
		
		public AppConfig GetConfig()
		{
			AppConfig ret = new AppConfig();
			
			SqliteConnection conn = GetConnection();
			
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
			
			return ret;
		}
		
		public void SetConfig(AppConfig config)
		{
			if (config != null)
			{
				SqliteConnection conn = GetConnection();
			
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
		
	}
}

