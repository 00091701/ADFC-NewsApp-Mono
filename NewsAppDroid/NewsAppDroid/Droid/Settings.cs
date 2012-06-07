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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;

namespace de.dhoffmann.mono.adfcnewsapp.droid
{
	[Activity (Label = "Settings", Theme = "@style/MainTheme")]			
	public class Settings : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			SetContentView(Resource.Layout.Settings);
			
			CheckBox cbDateIndicate = FindViewById<CheckBox>(Resource.Id.cbDateIndicate);
			CheckBox cbDataUpdate = FindViewById<CheckBox>(Resource.Id.cbDataUpdate);
			
			// Konfiguration laden
			AppConfig appConfig = new Config().GetConfig();
			cbDateIndicate.Checked = appConfig.DateIndicate;
			cbDataUpdate.Checked = appConfig.DataAutomaticUpdate;
		}
		
		protected override void OnStop ()
		{
			base.OnStop ();
			
			CheckBox cbDateIndicate = FindViewById<CheckBox>(Resource.Id.cbDateIndicate);
			CheckBox cbDataUpdate = FindViewById<CheckBox>(Resource.Id.cbDataUpdate);
			
			// Konfiguration speichern
			AppConfig appConfig = new AppConfig();
			appConfig.DateIndicate = cbDateIndicate.Checked;
			appConfig.DataAutomaticUpdate = cbDataUpdate.Checked;
			
			new Config().SetConfig(appConfig);

		}
	}
}

