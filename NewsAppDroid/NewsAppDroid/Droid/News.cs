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
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using de.dhoffmann.mono.adfcnewsapp.AndroidService;

namespace de.dhoffmann.mono.adfcnewsapp.droid
{
	[Activity (Label = "News", Theme = "@style/MainTheme")]			
	public class News : Activity
	{
		private NewsListItemAdapter adapter;
		private bool showOnlyUnreadNews = false;
		private bool isMenuVisible = false;
		ProgressBar progressLoading;
		ImageButton btnRefresh;


		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);

			Logging.Log(this, Logging.LoggingTypeDebug, "OnCreate");

			// News anzeigen
			SetContentView(Resource.Layout.News);

			// Wenn die App noch nicht konfiguriert wurde, die Einstellungen anzeigen.
			if (!new Config(this).GetAppConfig().AppIsConfigured) 
			{
				Intent intentSettings = new Intent(this, typeof(Settings));
				StartActivityForResult(intentSettings, 0);
			}

			progressLoading = FindViewById<ProgressBar> (Resource.Id.progressLoading);
			btnRefresh = FindViewById<ImageButton> (Resource.Id.btnRefresh);

			btnRefresh.Click += BtnRefresh_Click;

			SetLoadingIcon(false);

			// add event to menu button
			ImageButton ibMenu = FindViewById<ImageButton> (Resource.Id.ibMenu);
			if (ibMenu != null)
				ibMenu.Click += IbMenu_Click;

			if (Intent.GetBooleanExtra("RELOADFEEDS", false))
				new FeedHelper().UpdateBGFeeds(this);


			ListView lvNews = FindViewById<ListView>(Resource.Id.lvNews);
			if (lvNews != null)
			{
				lvNews.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e) 
				{
					if (adapter == null || adapter.GetEntries == null)
						return;

					de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem entry = adapter.GetEntries[e.Position];
					Intent intent = new Intent(this, typeof(NewsDetails));
					intent.PutExtra("FeedID", entry.FeedID);
					intent.PutExtra("FeedItemID", entry.ItemID);

					StartActivity(intent);
				};
			}

			AlarmManager alarmManager = (AlarmManager) this.GetSystemService(Context.AlarmService);
			PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, new Intent(this, typeof(NewsAppAlarmService)), 0);
			alarmManager.SetRepeating(AlarmType.Rtc, 0, AlarmManager.IntervalHalfDay, pendingIntent);
		}

		void BtnRefresh_Click (object sender, EventArgs e)
		{
			new FeedHelper().UpdateBGFeeds(this);
		}

		void IbMenu_Click (object sender, EventArgs e)
		{
			// hide menu
			if (isMenuVisible)
				this.CloseOptionsMenu ();
			else
			// show menu
				this.OpenOptionsMenu ();

			isMenuVisible = !isMenuVisible;
		}

		protected override void OnResume()
		{
			base.OnResume ();

			Logging.Log(this, Logging.LoggingTypeDebug, "OnResume");

			// Feeds aktualisieren
			if (Intent.GetBooleanExtra("RELOADFEEDS", false))
				new FeedHelper().UpdateBGFeeds(this);

			LoadNews();
		}


		public void LoadNews()
		{
			List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem> items = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetActiveFeedItems(showOnlyUnreadNews);
			adapter = new NewsListItemAdapter(this, items);

			ListView lvNews = FindViewById<ListView>(Resource.Id.lvNews);
			if (lvNews != null)
				lvNews.Adapter = adapter;
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			Logging.Log(this, Logging.LoggingTypeDebug, "OnPrepareOptionsMenu");

			var item = menu.FindItem(Resource.Id.menuShowReadNews);

			if (!showOnlyUnreadNews)
				item.SetTitle("Ungelesene News");
			else
				item.SetTitle("Alle News");

			return base.OnPrepareOptionsMenu (menu);
		}


		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			Logging.Log(this, Logging.LoggingTypeDebug, "OnOptionsItemSelected");

			switch(item.ItemId)
			{
				case Resource.Id.menuSettings:
					Intent setIntent = new Intent(this, typeof(Settings));
					StartActivityForResult(setIntent, 0);
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
			Logging.Log(this, Logging.LoggingTypeDebug, "OnActivityResult");

			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode == Result.Canceled)
			{
				Toast.MakeText(this, "Die Newsfeeds werden aktualisiert.", Android.Widget.ToastLength.Short).Show();
				new FeedHelper().UpdateBGFeeds(this);
			}
		}


		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater menuInflater = new Android.Views.MenuInflater(this);
			menuInflater.Inflate(Resource.Layout.MenuNews, menu);

			return true;
		}


		/// <summary>
		/// Steuert die Anzeige des Wartekringels
		/// </summary>
		/// <param name='isActive'>
		/// If set to <c>true</c> is active.
		/// </param>
		public void SetLoadingIcon(bool isActive)
		{
			RunOnUiThread(delegate() {
				Logging.Log(this, Logging.LoggingTypeDebug, "SetLoadingIcon: " + (isActive? "Visible" : "Invisible"));
				progressLoading.Visibility = (isActive? ViewStates.Visible : ViewStates.Gone);
				btnRefresh.Visibility = (!isActive? ViewStates.Visible : ViewStates.Gone);
			});
		}

	}
}

