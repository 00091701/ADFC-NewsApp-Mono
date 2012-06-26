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


namespace de.dhoffmann.mono.adfcnewsapp.buslog
{
	public class FeedHelper
	{
		public FeedHelper ()
		{
		}

		public void UpdateBGFeeds()
		{
			BackgroundWorker bgWorker = new BackgroundWorker();
			
			bgWorker.DoWork += delegate(object sender, DoWorkEventArgs e) 
			{
				UpdateFeeds();
			};
			
			bgWorker.RunWorkerAsync();
		}

		public void UpdateFeeds()
		{		
			// Konfiguration vom Webserver laden
			System.Diagnostics.Debug.WriteLine("Konfiguration vom Webserver laden");
			List<WSFeedConfig.FeedConfig> feedsConfig = new WSFeedConfig().GetFeedConfig();
			
			System.Diagnostics.Debug.WriteLine("Konfiguration in der Datenbank speichern");
			
			Config config = new Config();
			
			// Konfiguration in der Datenbank speichern
			config.SetWSConfig(feedsConfig);
			
			// Neu aus der DB laden um auch die FeedID zu bekommen.
			feedsConfig = config.GetWSConfig();
			
			System.Diagnostics.Debug.WriteLine("Feeds importieren");
			
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
			
			System.Diagnostics.Debug.WriteLine("Feedimport abgeschlossen.");
		}

		
		
	}
}

