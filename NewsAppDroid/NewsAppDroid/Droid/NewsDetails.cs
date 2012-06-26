
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

			SetContentView (Resource.Layout.NewsDetails);

			int feedID = Intent.GetIntExtra ("FeedID", -1);
			int feedItemID = Intent.GetIntExtra ("FeedItemID", -1);

			if (feedItemID >= 0) 
			{
				buslog.feedimport.Rss.RssFeed rssfeed = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetRssFeed(feedID, feedItemID);

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
					intent.PutExtra(Android.Content.Intent.ExtraText, description + "\n\n\n" + url + "\n\npowered by: APP");

					StartActivity(Intent.CreateChooser(intent, "Share via"));
				};
			}
		}
	}
}

