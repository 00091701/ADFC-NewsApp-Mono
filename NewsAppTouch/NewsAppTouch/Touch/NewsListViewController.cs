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

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using de.dhoffmann.mono.adfcnewsapp.buslog;
using De.Dhoffmann.Mono.Adfcnewsapp.IosHelper;

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	public partial class NewsListViewController : UITableViewController
	{
		private NewsListDataSource dsNewsList;
		public de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem SelectedFeedItem;

		public NewsListViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AppConfig appConfig = new Config(null).GetAppConfig();

			if (!appConfig.AppIsConfigured)
			{
				UpDateFeeds();

				EinstellungenViewController vcEinstellungen = Storyboard.InstantiateViewController("Einstellungen") as EinstellungenViewController;
				vcEinstellungen.ScrNewsListVC = this;
				NavigationController.PushViewController(vcEinstellungen, false);
			}
			else
			{
				UpDateFeeds();
				BindMyData();
			}
		}

		public void BindMyData ()
		{
			dsNewsList = new NewsListDataSource();
			dsNewsList.LoadData();
			tblNewsList.DataSource = dsNewsList;
			tblNewsList.Delegate = new NewsListDelegate(this);
			tblNewsList.ReloadData();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.DestinationViewController.GetType() == typeof(NewsDetailViewController))
			{
				NewsDetailViewController scrNewsDetailVC = (NewsDetailViewController)segue.DestinationViewController;
				scrNewsDetailVC.SelectedFeedItem = SelectedFeedItem;
			}
		}

		public void UpDateFeeds ()
		{
			new FeedHelper(this).UpdateBGFeeds();
		}

		static bool UserInterfaceIdiomIsPhone 
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}
	}
}
