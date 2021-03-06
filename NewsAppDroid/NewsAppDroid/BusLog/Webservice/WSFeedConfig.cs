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
using System.Net;
using System.IO;
using System.Json;
using System.Text;


namespace de.dhoffmann.mono.adfcnewsapp.buslog.webservice
{
	public class WSFeedConfig : WebServiceBase
	{
		public enum FeedTypes : int
		{
			UNSET = 0,
			News,
			Dates
		}
		
		public enum UrlTypes : int
		{
			UNSET = 0,
			RSS,
			ICS
		}
		
		public class FeedConfig
		{
			public int FeedID { get; set; }
			public bool IsActive { get; set; }
			public string Name { get; set; }
			public FeedTypes FeedType { get; set; }
			public string Url { get; set; }
			public UrlTypes UrlType { get; set; }
			public string CategoryFilter { get; set; }
			public string UseEncoding { get; set; }
		}
		
		
		public WSFeedConfig ()
		{
		}
		
		
		public List<FeedConfig> GetFeedConfig()
		{
			List<FeedConfig> ret = null;
			
			// Zentrale Konfigurationsdatei
			string url = "https://raw.github.com/00091701/ADFC-NewsApp-Mono/master/FeedConfig.json";
			JsonValue jsonValue = null;
			
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

#if MONODROID
				string proxyHost = Android.Net.Proxy.DefaultHost;
				int proxyPort = Android.Net.Proxy.DefaultPort;
#endif				
#if MONOTOUCH
				//TODO
				string proxyHost = null;
				int proxyAddr = 0;
#endif
				//Wenn ein Proxy im System eingestellt ist, diesen auch nutzen
				if(!String.IsNullOrEmpty(proxyHost))
					request.Proxy = new WebProxy(proxyHost, proxyPort);

				request.AllowAutoRedirect = true;
				request.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
				request.Headers.Add(HttpRequestHeader.AcceptLanguage, System.Threading.Thread.CurrentThread.CurrentUICulture.Name + "," + System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.Method = "GET";
				
				jsonValue = (JsonObject)JsonObject.Load(request.GetResponse().GetResponseStream());
			}
			catch(WebException ex)
			{
				Logging.Log(this, Logging.LoggingTypeError, string.Format("Fehler beim Abruf des Feeds: {0} - ex: ", url), ex);
			}
			
			if (jsonValue != null && jsonValue.Count > 0)
			{
				ret = new List<FeedConfig>();
				
				foreach (JsonValue feed in (jsonValue["Feeds"] as JsonArray))
				{
					FeedConfig feedConfig = new FeedConfig();
					feedConfig.Name = (string)feed["Name"];
					feedConfig.Url = (string)feed["URL"];
					
					if (feed.ContainsKey("CategoryFilter") && !string.IsNullOrEmpty((string)feed["CategoryFilter"]))
						feedConfig.CategoryFilter = (string)feed["CategoryFilter"];
					
					switch((string)feed["FeedType"])
					{
						case "News":
							feedConfig.FeedType = FeedTypes.News;
							break;
						case "Dates":
							feedConfig.FeedType = FeedTypes.Dates;
							break;
						default:
							feedConfig.FeedType = FeedTypes.UNSET;
							break;
					}
					
					switch((string)feed["URLType"])
					{
						case "RSS":
							feedConfig.UrlType = UrlTypes.RSS;
							break;
						case "ICS":
							feedConfig.UrlType = UrlTypes.ICS;
							break;
						default:
							feedConfig.UrlType = UrlTypes.UNSET;
							break;
					}
				
					if (feed.ContainsKey("UseEncoding") && !string.IsNullOrEmpty((string)feed["UseEncoding"]))
						feedConfig.UseEncoding = (string)feed["UseEncoding"];

					if (!string.IsNullOrEmpty(feedConfig.Name) &&
					    !string.IsNullOrEmpty(feedConfig.Url) &&
					    feedConfig.FeedType != FeedTypes.UNSET &&
					    feedConfig.UrlType != UrlTypes.UNSET)
						ret.Add(feedConfig);
				}
			}
			
			
			return ret;
		}
	}
}

