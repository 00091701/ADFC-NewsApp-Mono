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
using System.Net;
using System.IO;
using System.Text;


namespace de.dhoffmann.mono.adfcnewsapp.buslog.webservice
{
	public class Download : WebServiceBase
	{
		public Download ()
		{
		}
		
		
		public string DownloadWebSource (string url)
		{
			string ret = null;
			
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.AllowAutoRedirect = true;
				request.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
				request.Headers.Add(HttpRequestHeader.AcceptLanguage, System.Threading.Thread.CurrentThread.CurrentUICulture.Name + "," + System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.Method = "GET";

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				Encoding encoding = Encoding.UTF8;

				try
				{
					if (!String.IsNullOrEmpty(response.CharacterSet))
						encoding = Encoding.GetEncoding(response.CharacterSet);
				}
				catch(Exception ex)
				{
					Logging.Log(this, Logging.LoggingTypeError, "Unbekanntes Encoding", ex);
				}

				using (Stream resStream = response.GetResponseStream())
				{
					StreamReader reader = new StreamReader(resStream, encoding);
					ret = reader.ReadToEnd();
				}
			}
			catch(WebException ex)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Fehler beim Abruf des Feeds: {0} - ex: ", url, ex.ToString()));
			}
			
			return ret;
		}
	}
}

