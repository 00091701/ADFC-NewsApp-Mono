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
using System.ComponentModel;
using de.dhoffmann.mono.adfcnewsapp.buslog.webservice;
using System.Collections.Generic;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using de.dhoffmann.mono.adfcnewsapp.buslog;


namespace de.dhoffmann.mono.adfcnewsapp.buslog
{
	public class FeedHelper
	{
#if MONODROID
		Android.App.Activity activity;
#endif
		private static Object lockObject = new Object();
#if MONOTOUCH
		private De.Dhoffmann.Mono.Adfcnewsapp.Touch.NewsListViewController parentController;

		public FeedHelper (De.Dhoffmann.Mono.Adfcnewsapp.Touch.NewsListViewController controller)
		{
			this.parentController = controller;
		}
#endif

#if MONODROID
		public FeedHelper ()
		{}

		public void UpdateBGFeeds(Android.App.Activity activity)
		{
			this.activity = activity;
#endif

#if MONOTOUCH
		public void UpdateBGFeeds()
		{
#endif
			Logging.Log(this, Logging.LoggingTypeDebug, "UpdateBGFeeds()");
			BackgroundWorker bgWorker = new BackgroundWorker();
			
			bgWorker.DoWork += delegate(object sender, DoWorkEventArgs e) 
			{
#if MONODROID
				// Wartekringel anzeigen
				de.dhoffmann.mono.adfcnewsapp.droid.Tabs tabs = this.activity.Parent as de.dhoffmann.mono.adfcnewsapp.droid.Tabs;
				
				if (tabs != null)
					tabs.SetLoadingIcon(true);
#endif

				UpdateFeeds();
			};

			bgWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) 
			{
				Logging.Log(this, Logging.LoggingTypeDebug, "UpdateBGFeeds() - RunWorkerCompleted");
#if MONODROID
				
				activity.RunOnUiThread(delegate() 
				{
					if (this.activity.GetType() == typeof(de.dhoffmann.mono.adfcnewsapp.droid.News))
					{
						de.dhoffmann.mono.adfcnewsapp.droid.News aNews = (de.dhoffmann.mono.adfcnewsapp.droid.News)this.activity;
						aNews.LoadNews();
						Android.Widget.Toast.MakeText(this.activity, "Die Newsfeeds sind aktualisiert.", Android.Widget.ToastLength.Short).Show();
					}

					// Wartekringel entfernen
					de.dhoffmann.mono.adfcnewsapp.droid.Tabs tabs = this.activity.Parent as de.dhoffmann.mono.adfcnewsapp.droid.Tabs;
					
					if (tabs != null)
						tabs.SetLoadingIcon(false);
				});
#endif
#if MONOTOUCH
				parentController.InvokeOnMainThread(delegate 
				{
					// TODO: Wartekringel entfernen
					this.parentController.BindMyData();
				});
#endif
			};
			
			bgWorker.RunWorkerAsync();
		}

		public void UpdateFeeds()
		{		
			Logging.Log(this, Logging.LoggingTypeDebug, "UpdateFeeds() - before lock");
			lock(lockObject)
			{
				Logging.Log(this, Logging.LoggingTypeDebug, "UpdateFeeds() - in lock");

				// Konfiguration vom Webserver laden
				Logging.Log(this, Logging.LoggingTypeDebug, "Konfiguration vom Webserver laden");
				List<WSFeedConfig.FeedConfig> feedsConfig = new WSFeedConfig().GetFeedConfig();
				
				Logging.Log(this, Logging.LoggingTypeDebug, "Konfiguration in der Datenbank speichern");
				
				Config config = new Config(this);
				
				// Konfiguration in der Datenbank speichern
				config.SetWSConfig(feedsConfig);
				
				// Neu aus der DB laden um auch die FeedID zu bekommen.
				feedsConfig = config.GetWSConfig();

				Logging.Log(this, Logging.LoggingTypeInfo, "Feedimport start.");
				
				foreach(WSFeedConfig.FeedConfig feed in feedsConfig)
				{
					// Nur aktive feeds laden
					if (!feed.IsActive)
						continue;
					
					string webSource = new Download().DownloadWebSource(feed.Url);
					
					if (!string.IsNullOrEmpty(webSource))
					{
						switch(feed.FeedType)
						{
							case WSFeedConfig.FeedTypes.News:
								new feedimport.Rss().ImportRss(feed, webSource);
								break;
						}
					}
				}

				Logging.Log(this, Logging.LoggingTypeInfo, "Feedimport abgeschlossen.");
			}
		}
	}
}

