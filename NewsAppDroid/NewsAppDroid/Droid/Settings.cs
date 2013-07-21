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
using de.dhoffmann.mono.adfcnewsapp.androidhelper;
using de.dhoffmann.mono.adfcnewsapp.buslog.webservice;
using System.ComponentModel;
using de.dhoffmann.mono.adfcnewsapp.buslog;

namespace de.dhoffmann.mono.adfcnewsapp.droid
{
	[Activity (Label = "Settings", Theme = "@style/MainTheme")]			
	public class Settings : Activity
	{
		private SettingsFeedListAdapter settingsFeedListAdapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Logging.Log(this, Logging.LoggingTypeDebug, "OnCreate");
			
			SetContentView(Resource.Layout.Settings);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			bool firstRun = Intent.GetBooleanExtra("FirstRun", false);

			if (!firstRun)
				BindMyData();
			else 
			{
				BackgroundWorker bgWorker = new BackgroundWorker();

				bgWorker.DoWork += delegate(object sender, DoWorkEventArgs e) 
				{
					RunOnUiThread(delegate() 
					{
						AlertDialog dlgInfo = new AlertDialog.Builder(this).Create();
	     				dlgInfo.SetTitle("Hinweis");
	     				dlgInfo.SetMessage("In diesem Moment wird die Basiskonfiguration von einem Webserver geladen.\n\n" +
						                   "Warten Sie daher bitte einen kleinen Moment.\nAnschließend können Sie " +
						                   "Termine und Neuigkeiten von verschiedenen Ortgruppen abonnieren.\n\n" +
						                   "Die Einstellungen werden automatisch beim verlassen der Konfiguration gespeichert." + 
						                   "\n\nBitte drücken Sie jetzt die 'Zurück-Taste' und warten einen Moment." +
						                   "\n\nNachdem Sie die gewünschten Einstellungen vorgenommen haben, drücken Sie bitte " +
						                   "nochmal auf die 'Zurück-Taste' die gewünschten Nachrichten werden dann aktualisiert und " +
						                   "stehen kurz danach zur Verfügung."
						                   );
	     				dlgInfo.Show();
					});

					new FeedHelper().UpdateFeeds();
				};

				bgWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) 
				{
					RunOnUiThread(delegate() 
					{
						BindMyData();
					});
				};

				bgWorker.RunWorkerAsync();
			}
		}

		private void BindMyData()
		{
			CheckBox cbDateIndicate = FindViewById<CheckBox>(Resource.Id.cbDateIndicate);
			CheckBox cbDataUpdate = FindViewById<CheckBox>(Resource.Id.cbDataUpdate);
			
			// Konfiguration laden
			Config config = new Config(this);
			AppConfig appConfig = config.GetAppConfig();
			cbDateIndicate.Checked = appConfig.DateIndicate;
			cbDataUpdate.Checked = appConfig.DataAutomaticUpdate;

			List<WSFeedConfig.FeedConfig> feedConfig = config.GetWSConfig();

			ListView lvDataSubscription = FindViewById<ListView>(Resource.Id.lvDataSubscription);
			settingsFeedListAdapter = new SettingsFeedListAdapter(this, feedConfig);
			lvDataSubscription.Adapter = settingsFeedListAdapter;
		}


		protected override void OnPause ()
		{
			base.OnPause ();

			CheckBox cbDateIndicate = FindViewById<CheckBox>(Resource.Id.cbDateIndicate);
			CheckBox cbDataUpdate = FindViewById<CheckBox>(Resource.Id.cbDataUpdate);
			
			// Konfiguration speichern
			AppConfig appConfig = new AppConfig();
			appConfig.DateIndicate = cbDateIndicate.Checked;
			appConfig.DataAutomaticUpdate = cbDataUpdate.Checked;

			Config config = new Config(this);
			config.SetAppConfig(appConfig);

			if (settingsFeedListAdapter != null)
				config.SetWSConfig(settingsFeedListAdapter.GetFeedConfig);
		}
		
		protected override void OnStop ()
		{
			base.OnStop ();		
		}
	}
}

