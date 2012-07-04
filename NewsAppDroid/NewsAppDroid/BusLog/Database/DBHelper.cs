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

#if MONODROID
using Android.Database.Sqlite;
#endif

namespace de.dhoffmann.mono.adfcnewsapp.buslog.database
{
#if MONODROID

	public class DBHelper : SQLiteOpenHelper
	{
		public const string DATABASENAME = "News.db";
		private const int DATABASEVERSION = 1;

		public DBHelper(Android.Content.Context context) : base(context, DATABASENAME, null, DATABASEVERSION)
		{
			;
		}		


		#region implemented abstract members of Android.Database.Sqlite.SQLiteOpenHelper
		public override void OnCreate (SQLiteDatabase db)
		{
			;
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			;
		}
		#endregion
	}

#else
	// Wrapper für Nicht-Android
	public class DBHelper
	{
		public const string DATABASENAME = "News.db";

		public DBHelper(object context)
		{
		}
	}
#endif
}
