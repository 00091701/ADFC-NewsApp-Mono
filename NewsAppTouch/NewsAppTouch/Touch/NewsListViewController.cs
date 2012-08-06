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

		public NewsListViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AppConfig appConfig = new Config(null).GetAppConfig();

			if (!appConfig.AppIsConfigured)
			{
				new FeedHelper().UpdateBGFeeds();

				EinstellungenViewController vcEinstellungen = Storyboard.InstantiateViewController("Einstellungen") as EinstellungenViewController;
				NavigationController.PushViewController(vcEinstellungen, false);
			}
			else
			{
				BindMyData();
			}
		}

		public void BindMyData ()
		{
			new FeedHelper().UpdateBGFeeds();

			dsNewsList = new NewsListDataSource();
			dsNewsList.LoadData();
			tblNewsList.DataSource = dsNewsList;
		}

		static bool UserInterfaceIdiomIsPhone 
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}
	}
}
