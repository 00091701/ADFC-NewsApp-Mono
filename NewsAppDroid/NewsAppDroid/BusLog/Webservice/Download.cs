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
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.Method = "GET";
				
				StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
				ret = sr.ReadToEnd();
			}
			catch(WebException ex)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Fehler beim Abruf des Feeds: {0} - ex: ", url, ex.ToString()));
			}
			
			return ret;
		}
	}
}

