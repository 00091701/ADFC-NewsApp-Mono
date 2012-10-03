using System;
using MonoTouch.UIKit;
using De.Dhoffmann.Mono.Adfcnewsapp.Touch;
using System.Linq;

namespace De.Dhoffmann.Mono.Adfcnewsapp.IosHelper
{
	public class NewsListDelegate : UITableViewDelegate
	{
		NewsListViewController parentController;

		public NewsListDelegate (NewsListViewController controller)
		{
			this.parentController = controller;
		}

		public override MonoTouch.Foundation.NSIndexPath WillSelectRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			NewsListDataSource ds = tableView.DataSource as NewsListDataSource;
			this.parentController.SelectedFeedItem = ds.GetRow(indexPath.Row);


			new de.dhoffmann.mono.adfcnewsapp.buslog.database.Rss().MarkItemsAsRead(this.parentController.SelectedFeedItem.ItemID, true);
			((NewsListDataSource)tableView.DataSource).ViewData.FirstOrDefault(p => p.Value.ItemID == this.parentController.SelectedFeedItem.ItemID).Value.IsRead = true;

			tableView.ReloadRows(new MonoTouch.Foundation.NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);

			return indexPath;
		}
	}
}

