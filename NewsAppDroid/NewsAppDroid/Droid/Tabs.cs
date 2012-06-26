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
using de.dhoffmann.mono.adfcnewsapp.buslog;

namespace de.dhoffmann.mono.adfcnewsapp.droid
{
	[Activity (Label = "ADFC-News", Icon="@drawable/Icon", Theme = "@style/MainTheme", MainLauncher = true)]			
	public class Tabs : TabActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			SetContentView (Resource.Layout.Tabs);
			
			// Datenbank initialisieren
			new DBSchema().UpdateDBSchema();
			
			TabHost.TabSpec spec;
			Intent intent;
			
			// -- 
			/*
			intent = new Intent(this, typeof(Dates));
			intent.AddFlags(ActivityFlags.NewTask);
			
			spec = TabHost.NewTabSpec("TabDates");
			spec.SetIndicator("Termine");
			spec.SetContent(intent);
			
			TabHost.AddTab(spec);
			*/
			// -- 
			
			intent = new Intent (this, typeof(News));
			intent.AddFlags (ActivityFlags.NewTask);
			
			spec = TabHost.NewTabSpec ("TabNews");
			spec.SetIndicator ("Neuigkeiten");
			spec.SetContent (intent);
			
			TabHost.AddTab(spec);
			
			// -- 
			
			// Wenn die App noch nicht konfiguriert wurde, die Einstellungen anzeigen.
			if (!new Config().GetAppConfig ().AppIsConfigured) 
			{
				Intent setIntent = new Android.Content.Intent(this, typeof(Settings));
				setIntent.PutExtra("FirstRun", true);
				StartActivity(setIntent);
			}
		}
		
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater menuInflater = new Android.Views.MenuInflater(this);
			menuInflater.Inflate(Resource.Layout.Menu, menu);
			
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch(item.ItemId)
			{
				case Resource.Id.menuSettings:
					StartActivity(typeof(Settings));
					break;
				case Resource.Id.menuGetDataNow:
					new FeedHelper() .UpdateBGFeeds();
					break;
			}
			
			return true;
		}
	}
}

