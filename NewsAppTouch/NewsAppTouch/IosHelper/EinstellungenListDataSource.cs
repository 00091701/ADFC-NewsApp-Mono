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
using de.dhoffmann.mono.adfcnewsapp.buslog.webservice;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using System.Linq;

namespace De.Dhoffmann.Mono.Adfcnewsapp.IosHelper
{
	public class EinstellungenListDataSource : UITableViewDataSource
	{
		private List<UISwitch> uiSwitches = new List<UISwitch>();
		public List<WSFeedConfig.FeedConfig> ViewData;

		public EinstellungenListDataSource ()
		{
		}

		public void LoadData ()
		{
			ViewData = new Config(this).GetWSConfig();
		}

		public void SaveData ()
		{
			new Config(this).SetWSConfig(ViewData);
		}

		#region implemented abstract members of MonoTouch.UIKit.UITableViewDataSource
		public override int RowsInSection (UITableView tableView, int section)
		{
			if (ViewData == null)
				return 0;
			else
				return ViewData.Count;
		}



		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell("EinstellungenListCell") as UITableViewCell;

			WSFeedConfig.FeedConfig entry = ViewData[indexPath.Row];

			UILabel lbWS = cell.ViewWithTag(202) as UILabel;
			lbWS.Text = entry.Name;

			UISwitch tblSwitch = cell.ViewWithTag(203) as UISwitch;
			tblSwitch.On = entry.IsActive;

			UILabel lbHidden = cell.ViewWithTag(204) as UILabel;
			lbHidden.Hidden = true;
			lbHidden.Text = entry.FeedID.ToString();

			uiSwitches.Add(tblSwitch);
			tblSwitch.ValueChanged += SwitchValueChanged;

			return cell;
		}
		#endregion


		public void SwitchValueChanged(object sender, EventArgs e)
		{
			UISwitch uiSwitch = (UISwitch)sender;
			UITableViewCell uiTableCell = (UITableViewCell)((UIView)uiSwitch.Superview).Superview;
			
			UILabel lbH = uiTableCell.ViewWithTag(204) as UILabel;
			if (lbH != null && !String.IsNullOrEmpty(lbH.Text))
			{
				UITableView uiTableView = (UITableView)uiTableCell.Superview;
				EinstellungenListDataSource ds = uiTableView.DataSource as EinstellungenListDataSource;
				
				int feedID = int.Parse(lbH.Text);
				
				WSFeedConfig.FeedConfig feedConfig = ds.ViewData.Where(p => p.FeedID == feedID).FirstOrDefault();
				if (feedConfig != null)
					feedConfig.IsActive = uiSwitch.On;
			}
		}

	}
}

