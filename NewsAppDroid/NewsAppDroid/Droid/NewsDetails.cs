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
using de.dhoffmann.mono.adfcnewsapp;
using de.dhoffmann.mono.adfcnewsapp.buslog;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using de.dhoffmann.mono.adfcnewsapp.buslog.feedimport;

namespace de.dhoffmann.mono.adfcnewsapp.droid
{
	[Activity (Label = "NewsDetails", Theme = "@style/MainTheme")]		
	public class NewsDetails : Activity
	{
		private string url = null;
		private string description = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Logging.Log(this, Logging.LoggingTypeDebug, "OnCreate");

			SetContentView (Resource.Layout.NewsDetails);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			int feedID = Intent.GetIntExtra ("FeedID", -1);
			int feedItemID = Intent.GetIntExtra ("FeedItemID", -1);

			if (feedItemID >= 0) 
			{
				Button btnNextNewsEntry = FindViewById<Button>(Resource.Id.btnNextNewsEntry);

				List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem> rssItems = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetActiveFeedItems(false);
				btnNextNewsEntry.Visibility = ViewStates.Gone;

				int? nextFeedID = null;
				int? nextFeedItemID = null;

				if (rssItems != null && rssItems.Count > 1)
				{
					bool ready = false;
					for (int nIndex=0; nIndex<rssItems.Count; nIndex++)
					{
						if (!ready)
						{
							if (rssItems[nIndex].ItemID == feedItemID)
							{
								ready = true;
							}

							continue;
						}

						if (!rssItems[nIndex].IsRead)
						{
							nextFeedID = rssItems[nIndex].FeedID;
							nextFeedItemID = rssItems[nIndex].ItemID;
							btnNextNewsEntry.Visibility = ViewStates.Visible;
							break;
						}
					}
				}

				new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().MarkItemsAsRead(feedItemID, true);
				de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssFeed rssfeed = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetRssFeed(feedID, feedItemID);

				if (rssfeed.Items.First().PubDate.HasValue)
				{
					TextView tvNewsDetailDate = FindViewById<TextView>(Resource.Id.tvNewsDetailDate);
					tvNewsDetailDate.Text = rssfeed.Items.First().PubDate.ToString();
				}

				TextView tvNewsDetailFeedTitle = FindViewById<TextView>(Resource.Id.tvNewsDetailFeedTitle);
				tvNewsDetailFeedTitle.Text = rssfeed.Header.Title;

				TextView tvNewsItemHeadline = FindViewById<TextView>(Resource.Id.tvNewsItemHeadline);
				tvNewsItemHeadline.Text = rssfeed.Items.First().Title;

				TextView tvNewsDetail = FindViewById<TextView>(Resource.Id.tvNewsDetail);
				tvNewsDetail.Text = rssfeed.Items.First().Description;

				url = rssfeed.Items.First().Link;
				description = rssfeed.Items.First().Description;

				Button btnWebsite = FindViewById<Button>(Resource.Id.btnWebsite);
				btnWebsite.Click += delegate(object sender, EventArgs e) 
				{
					Intent intent = new Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse(url));

					StartActivity(intent);
				};

				Button btnShare = FindViewById<Button>(Resource.Id.btnShare);
				btnShare.Click += delegate(object sender, EventArgs e) 
				{
					Intent intent = new Intent(Android.Content.Intent.ActionSend);
					intent.SetType("text/plain");

					intent.PutExtra(Android.Content.Intent.ExtraSubject, "Neues vom ADFC");
					intent.PutExtra(Android.Content.Intent.ExtraText, description + "\n\n\n" + url + "\n\npowered by: ADFC-News für Android\nhttps://play.google.com/store/apps/details?id=de.dhoffmann.mono.adfcnewsapp");

					StartActivity(Intent.CreateChooser(intent, "Share via"));
				};

				if (nextFeedID.HasValue && nextFeedItemID.HasValue)
				{
					btnNextNewsEntry.Click += delegate(object sender, EventArgs e) 
					{
						Intent i = new Android.Content.Intent(this, typeof(NewsDetails));
						i.PutExtra("FeedID", nextFeedID.Value);
						i.PutExtra("FeedItemID", nextFeedItemID.Value);
						
						StartActivity(i);
						this.Finish();
					};
				}
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater menuInflater = new Android.Views.MenuInflater(this);
			menuInflater.Inflate(Resource.Layout.NewsDetailsMenu, menu);
			
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch(item.ItemId)
			{
				case Resource.Id.menuMarkAsUnread:
					int feedItemID = Intent.GetIntExtra ("FeedItemID", -1);
					
					if (feedItemID >= 0) 
						new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().MarkItemsAsRead(feedItemID, false);

					break;
			}
			
			return true;
		}
	}
}

