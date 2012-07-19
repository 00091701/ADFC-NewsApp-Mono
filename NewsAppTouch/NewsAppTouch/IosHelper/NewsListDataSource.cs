using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using De.Dhoffmann.Mono.Adfcnewsapp.Touch;
using de.dhoffmann.mono.adfcnewsapp.buslog.feedimport;

namespace De.Dhoffmann.Mono.Adfcnewsapp.IosHelper
{
	public class NewsListDataSource : UITableViewDataSource
	{
		Dictionary<int, NewsListCellViewController> cellControllers = new Dictionary<int, NewsListCellViewController>();
        List<KeyValuePair<string, Rss.RssItem>> viewData;

		public NewsListDataSource ()
		{
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
			UITableViewCell cell = tableView.DequeueReusableCell("CellID");
            NewsListCellViewController cellController = null;

            if (cell == null)
            {
                cellController = new NewsListCellViewController();
                cell = cellController.Cell;
                cell.Tag = Environment.TickCount;
                cellControllers[cell.Tag] = cellController;
            }
            else
            {
                cellController = cellControllers[cell.Tag];
            }

			cellController.LbHeadline.Text = viewData[indexPath.Row].Value.Title;

			if (viewData[indexPath.Row].Value.PubDate.HasValue)
				cellController.LbDate.Text = viewData[indexPath.Row].Value.PubDate.Value.ToString();

            return cell;
		}
		#endregion

	}
}

