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
using de.dhoffmann.mono.adfcnewsapp.buslog;
using de.dhoffmann.mono.adfcnewsapp.buslog.feedimport;
using de.dhoffmann.mono.adfcnewsapp.androidhelper;

namespace de.dhoffmann.mono.adfcnewsapp.droid
{
	[Activity (Label = "News")]			
	public class News : Activity
	{
		private NewsListItemAdapter adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.News);

			ListView lvNews = FindViewById<ListView>(Resource.Id.lvNews);
			lvNews.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) 
			{
				if (adapter == null || adapter.GetEntries == null)
					return;

				Rss.RssItem entry = adapter.GetEntries[e.Position];
				Intent intent = new Intent(this, typeof(NewsDetails));
				intent.PutExtra("FeedID", entry.FeedID);
				intent.PutExtra("FeedItemID", entry.ItemID);

				StartActivity(intent);
			};
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			List<Rss.RssItem> items = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetActiveFeedItems(false);
			adapter = new NewsListItemAdapter(this, items);

			ListView lvNews = FindViewById<ListView>(Resource.Id.lvNews);
			lvNews.Adapter = adapter;

		}
	}
}

