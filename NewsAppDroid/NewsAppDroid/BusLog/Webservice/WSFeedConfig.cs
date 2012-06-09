using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Json;


namespace de.dhoffmann.mono.adfcnewsapp.buslog.webservice
{
	public class WSFeedConfig : WebServiceBase
	{
		public enum FeedTypes : int
		{
			News = 0,
			Dates
		}
		
		public enum UrlTypes : int
		{
			RSS = 0,
			ICS
		}
		
		public class FeedConfig
		{
			string Name { get; set; }
			FeedTypes Type { get; set; }
			string Url { get; set; }
			UrlTypes UrlType { get; set; }
		}
		
		
		public WSFeedConfig ()
		{
		}
		
		
		public List<FeedConfig> GetFeedConfig()
		{
			List<FeedConfig> ret = new List<FeedConfig>();
			
			string url = "https://raw.github.com/00091701/ADFC-NewsApp-Mono/master/FeedConfig.json";
			
			WebRequest webRequest = (WebRequest)HttpWebRequest.Create(url);
			WebResponse webResponse = webRequest.GetResponse();
			
			JsonValue jsonValue = (JsonObject)JsonObject.Load(webResponse.GetResponseStream());
			
			if (jsonValue != null)
			{
				
			}
			
			return ret;
		}
	}
}

