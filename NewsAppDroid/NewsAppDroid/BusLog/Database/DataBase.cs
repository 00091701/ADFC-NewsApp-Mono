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
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;
using System.Collections.Generic;


namespace de.dhoffmann.mono.adfcnewsapp.buslog.database
{
	public abstract class DataBase
	{
		public DataBase ()
		{
		}
		
		
		protected SqliteConnection GetConnection()
		{
			string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string dbFilename = Path.Combine(docPath, database.DBHelper.DATABASENAME);
			
			bool dbExists = File.Exists (dbFilename);
			
			// Existiert die DB schon?
			if (!dbExists)
				SqliteConnection.CreateFile(dbFilename);
			
			SqliteConnection conn = new SqliteConnection("Data Source=" + dbFilename);
			
			// Wenn es eine neue Datenbank ist.. 
			// Eine neue Tabelle für die Versionsverwaltung anlegen.
			if (!dbExists)
			{
				List<SqliteCommand> sqlCmds = new List<SqliteCommand>();

				SqliteCommand sqlCmd = new SqliteCommand("CREATE TABLE version (VersionID INTEGER PRIMARY KEY AUTOINCREMENT, DateCreate DATETIME NOT NULL);", conn);
				sqlCmds.Add(sqlCmd);

				SqliteCommand sqlCmd2 = new SqliteCommand("INSERT INTO version (VersionID, DateCreate) VALUES (0, @DateCreate);", conn);
				sqlCmd2.Parameters.AddWithValue("@DateCreate", DateTime.Now);
				sqlCmds.Add(sqlCmd2);
				
				conn.Open();

				foreach(SqliteCommand cmd in sqlCmds)
					cmd.ExecuteNonQuery();
				
				conn.Close();
			}
			
			return conn;
		}
	}
}

