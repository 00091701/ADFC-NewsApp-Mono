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
using MonoTouch.UIKit;
using System.Collections.Generic;
using De.Dhoffmann.Mono.Adfcnewsapp.Touch;
using de.dhoffmann.mono.adfcnewsapp.buslog.feedimport;
using System.Linq;

namespace De.Dhoffmann.Mono.Adfcnewsapp.IosHelper
{
	public class NewsListDataSource : UITableViewDataSource
	{
        List<KeyValuePair<string, Rss.RssItem>> viewData;

		public NewsListDataSource ()
		{
		}

		public void LoadData ()
		{
			List<de.dhoffmann.mono.adfcnewsapp.buslog.feedimport.Rss.RssItem> rssItems = new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().GetActiveFeedItems(false);

			viewData = new List<KeyValuePair<string, Rss.RssItem>>();
			foreach (Rss.RssItem item in rssItems)
				viewData.Add(new KeyValuePair<string, Rss.RssItem>(item.ItemID.ToString(), item));
		}

		#region implemented abstract members of MonoTouch.UIKit.UITableViewDataSource
		public override int RowsInSection (UITableView tableView, int section)
		{
			if (viewData == null)
				return 0;
			else
				return viewData.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{

			UITableViewCell cell = tableView.DequeueReusableCell("NewsListCell");

			UILabel lbHeadLine = cell.ViewWithTag(100) as UILabel;
			lbHeadLine.Text = viewData[indexPath.Row].Value.Title;

			if (viewData[indexPath.Row].Value.PubDate.HasValue)
			{
				UILabel lbDate = cell.ViewWithTag(101) as UILabel;
				lbDate.Text = viewData[indexPath.Row].Value.PubDate.Value.ToString();
			}

            return cell;
		}
		#endregion

		public Rss.RssItem GetRow(int row)
		{
			return viewData[row].Value;
		}

	}
}

