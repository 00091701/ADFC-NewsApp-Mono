using System;
using MonoTouch.UIKit;
using De.Dhoffmann.Mono.Adfcnewsapp.Touch;

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

			return indexPath;
		}
	}
}

