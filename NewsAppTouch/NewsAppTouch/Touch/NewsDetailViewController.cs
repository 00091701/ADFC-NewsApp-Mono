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

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	public partial class NewsDetailViewController : UIViewController
	{
		private string website;

		public de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem SelectedFeedItem { get; set; }

		public NewsDetailViewController (IntPtr handle) : base (handle)
		{
		}



		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (SelectedFeedItem != null)
			{
				de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssFeed rssFeed = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetRssFeed(SelectedFeedItem.FeedID, SelectedFeedItem.ItemID);

				UITextView txtContent = View.ViewWithTag(202) as UITextView;
				txtContent.Text = rssFeed.Items[0].Description;

				website = rssFeed.Items[0].Link;
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.DestinationViewController.GetType() == typeof(WebViewController))
			{
				((WebViewController)segue.DestinationViewController).GotoUrl = website;
			}
		}
	}
}
