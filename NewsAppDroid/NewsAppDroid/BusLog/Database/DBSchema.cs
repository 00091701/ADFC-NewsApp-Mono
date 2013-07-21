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
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;

namespace de.dhoffmann.mono.adfcnewsapp.buslog.database
{
	public class DBSchema : DataBase
	{
		public DBSchema()
		{
		}
		
		
		/// <summary>
		/// Gets the DB version.
		/// </summary>
		/// <returns>
		/// The DB version.
		/// </returns>
		public int GetDBVersion()
		{
			int retVersion = -1;
			
			using (SqliteConnection conn = GetConnection())
			{
				using(DbCommand c = conn.CreateCommand())
				{
					c.CommandText = "SELECT VersionID, DateCreate FROM version ORDER BY VersionID DESC Limit 1;";
					c.CommandType = System.Data.CommandType.Text;
					conn.Open();
					
					using (DbDataReader reader = c.ExecuteReader())
					{
						// Es gibt nur eine letzte Version
						reader.Read();
						
						if (reader.HasRows)
							retVersion = reader.GetInt32(0);
						else
							retVersion = -1;
					}
					
					conn.Close();
				}
			}
			
			if (retVersion == -1)
				throw new Exception("Fehler beim Zugriff auf die Datenbank!");
			
			return retVersion;
		}
		
		
		/// <summary>
		/// Aktualisiert das Datenbankenschema
		/// </summary>
		public void UpdateDBSchema()
		{
			StringBuilder commands = new StringBuilder();
			
			int currentVersion = GetDBVersion();
			
			// Befehle für die Schemaaktualisierung zusmmen sammeln.
			if (currentVersion <= 0)
			{
				commands.AppendLine("CREATE TABLE config (AppIsConfigured BOOLEAN NOT NULL, DateIndicate BOOLEAN NOT NULL, DataAutomaticUpdate BOOLEAN NOT NULL);");
				commands.AppendLine("INSERT INTO config (AppIsConfigured, DateIndicate, DataAutomaticUpdate) VALUES (0, 1, 1);");
				commands.AppendLine("CREATE TABLE feedconfig (FeedID INTEGER PRIMARY KEY AUTOINCREMENT, IsActive BOOLEAN NOT NULL, Name VARCHAR(100) NOT NULL, FeedType INTEGER NOT NULL, URL VARCHAR(250) NOT NULL, URLType INTEGER NOT NULL, CategoryFilter VARCHAR(100));");
				commands.AppendLine("CREATE TABLE rssfeeds (FeedID INTEGER PRIMARY KEY AUTOINCREMENT, DateCreate DATETIME NOT NULL, Title VARCHAR(200), Link VARCHAR(250), Description VARCHAR(500), LastBuildDate DATETIME);");
				commands.AppendLine("CREATE TABLE rssfeeditem (ItemID INTEGER PRIMARY KEY AUTOINCREMENT, FeedID INTEGER NOT NULL, DateCreate DATETIME NOT NULL, Title VARCHAR(200), Link VARCHAR(250), Description VARCHAR(1000), Category VARCHAR(100), PubDate DATETIME, IsRead BOOLEAN NOT NULL);");
				commands.AppendLine("INSERT INTO version (DateCreate) VALUES (date('now'));");
			}

			if (currentVersion <= 1) 
			{
				commands.AppendLine ("ALTER TABLE feedconfig ADD UseEncoding VARCHAR(30);");
				commands.AppendLine("INSERT INTO version (DateCreate) VALUES (date('now'));");
			}
			
			// Befehle an die Datenbank schicken
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
