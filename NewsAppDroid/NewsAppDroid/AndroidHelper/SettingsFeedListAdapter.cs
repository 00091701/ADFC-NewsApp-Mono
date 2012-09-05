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
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using de.dhoffmann.mono.adfcnewsapp.droid;
using de.dhoffmann.mono.adfcnewsapp.buslog.webservice;

namespace de.dhoffmann.mono.adfcnewsapp.androidhelper
{
	class SettingsFeedListAdapter : BaseAdapter
	{
		private Activity context;
		private List<WSFeedConfig.FeedConfig> entries;

		public SettingsFeedListAdapter (Activity context, List<WSFeedConfig.FeedConfig> entries)
		{
			this.context = context;
			this.entries = entries;
		}

		#region implemented abstract members of Android.Widget.BaseAdapter
		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}

		public override long GetItemId (int position)
		{
			if (entries == null)
				return -1;
			else
				return entries[position].FeedID;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (entries == null)
				return null;

			WSFeedConfig.FeedConfig entry = entries[position];

			LinearLayout view;
			CheckBox cbFeedItem;

			if (convertView == null)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.SettingsFeedListItem, parent, false) as LinearLayout;
				cbFeedItem = view.FindViewById<CheckBox>(Resource.Id.cbFeedItem);
				cbFeedItem.CheckedChange += ItemChecked;
			}
			else
			{
				view = convertView as LinearLayout;
				cbFeedItem = view.FindViewById<CheckBox>(Resource.Id.cbFeedItem);
			}

			cbFeedItem.Text = entry.Name;
			cbFeedItem.Tag = position;
			cbFeedItem.Checked = entry.IsActive;

			return view;
		}

		private void ItemChecked(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (entries == null)
				return;

			entries[(int)((CheckBox)(sender)).Tag].IsActive = e.IsChecked;
		}

		public override int Count 
		{
			get 
			{
				if (entries == null)
					return 0;
				else
					return entries.Count;
			}
		}
		#endregion


		public List<WSFeedConfig.FeedConfig> GetFeedConfig 
		{
			get 
			{
				return entries;
			}
		}
	}
}

