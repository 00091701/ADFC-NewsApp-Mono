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
	public partial class WebViewController : UIViewController
	{
		public string GotoUrl {
			get;
			set;
		}

		public WebViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIWebView webView = View.ViewWithTag(221) as UIWebView;
			webView.LoadRequest(new NSUrlRequest(new NSUrl(GotoUrl)));

		}
	}
}
