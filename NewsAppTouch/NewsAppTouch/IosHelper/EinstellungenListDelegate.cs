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

namespace De.Dhoffmann.Mono.Adfcnewsapp.IosHelper
{
	public class EinstellungenListDelegate : UITableViewDelegate
	{
		public EinstellungenListDelegate ()
		{
		}

		public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{

			UITableViewCell cell = tableView.DequeueReusableCell("EinstellungenListCell") as UITableViewCell;
			((UISwitch)((UIView)cell.Subviews[0]).Subviews[1]).On = !((UISwitch)((UIView)cell.Subviews[0]).Subviews[1]).On;

			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
		}
	}
}

