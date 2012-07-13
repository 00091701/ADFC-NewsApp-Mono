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
		private bool showOnlyUnreadNews = true;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			Logging.Log(this, Logging.LoggingTypeDebug, "OnCreate");

			SetContentView(Resource.Layout.News);

			if (Intent.GetBooleanExtra("RELOADFEEDS", false))
				new FeedHelper().UpdateBGFeeds(this);


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

		protected override void OnResume()
		{
			base.OnResume ();

			// Feeds aktualisieren
			if (Intent.GetBooleanExtra("RELOADFEEDS", false))
				new FeedHelper().UpdateBGFeeds(this);

			LoadNews();
		}


		public void LoadNews()
		{
			List<Rss.RssItem> items = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetActiveFeedItems(showOnlyUnreadNews);
			adapter = new NewsListItemAdapter(this, items);

			ListView lvNews = FindViewById<ListView>(Resource.Id.lvNews);
			lvNews.Adapter = adapter;
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			var item = menu.FindItem(Resource.Id.menuShowReadNews);

			if (!showOnlyUnreadNews)
				item.SetTitle("Ungelesene News");
			else
				item.SetTitle("Alle News");

			return base.OnPrepareOptionsMenu (menu);
		}


		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch(item.ItemId)
			{
				case Resource.Id.menuSettings:
					Intent setIntent = new Intent(this, typeof(Settings));
					StartActivityForResult(setIntent, 0);
					break;

				case Resource.Id.menuGetDataNow:
					new FeedHelper().UpdateBGFeeds(this);
					break;

				case Resource.Id.menuShowReadNews:
					showOnlyUnreadNews = !showOnlyUnreadNews;

					LoadNews();

					if (!showOnlyUnreadNews)
						item.SetTitle("Ungelesene News");
					else
						item.SetTitle("Alle News");
					break;
		
				case Resource.Id.menuMarkAllRead:
					new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().MarkItemsAsRead(null, true);
					LoadNews();
					break;
			}
			
			return true;
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Android.Content.Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode == Result.Canceled)
				new FeedHelper().UpdateBGFeeds(this);
		}
	}
}

