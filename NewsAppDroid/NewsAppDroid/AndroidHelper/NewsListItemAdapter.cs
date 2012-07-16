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
using de.dhoffmann.mono.adfcnewsapp.buslog.feedimport;
using de.dhoffmann.mono.adfcnewsapp.droid;
using Android.Graphics;

namespace de.dhoffmann.mono.adfcnewsapp.androidhelper
{
	public class NewsListItemAdapter : BaseAdapter
	{
		private Activity context;
		private List<Rss.RssItem> entries;

		public NewsListItemAdapter (Activity context, List<Rss.RssItem> entries)
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
				return entries[position].ItemID;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			if (entries == null)
				return null;

			Rss.RssItem entry = entries[position];

			var view = (convertView ?? context.LayoutInflater.Inflate(Resource.Layout.NewsListItem, parent, false)) as LinearLayout;

			TextView tvName = view.FindViewById<TextView>(Resource.Id.tvName);

			if (!entry.IsRead)
				tvName.SetTypeface(Typeface.DefaultBold, TypefaceStyle.Bold);

			tvName.Text = entry.Title;

			TextView tvDate = view.FindViewById<TextView>(Resource.Id.tvDate);
			if (entry.PubDate.HasValue)
				tvDate.Text = entry.PubDate.ToString();
			else
				tvDate.Visibility = ViewStates.Gone;

	        return view;
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

		public List<Rss.RssItem> GetEntries 
		{
			get
			{
				return entries;
			}
		}
	}
}

