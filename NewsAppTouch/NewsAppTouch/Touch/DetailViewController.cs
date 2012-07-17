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
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	public partial class DetailViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		UIPopoverController popoverController;
		string detailItem;
		
		[Export("detailItem")]
		public string DetailItem {
			get {
				return detailItem;
			}
			set {
				SetDetailItem (value);
			}
		}
		
		public DetailViewController (IntPtr handle) : base (handle)
		{
		}
		
		public void SetDetailItem (string newDetailItem)
		{
			if (detailItem != newDetailItem) {
				detailItem = newDetailItem;
				
				// Update the view
				ConfigureView ();
			}
			
			if (this.popoverController != null)
				this.popoverController.Dismiss (true);
		}
		
		void ConfigureView ()
		{
			// Update the user interface for the detail item
			if (DetailItem != null)
				this.detailDescriptionLabel.Text = DetailItem.ToString ();
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		#region View lifecycle
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			ConfigureView ();
			
			if (!UserInterfaceIdiomIsPhone)
				SplitViewController.Delegate = new SplitViewControllerDelegate ();
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}
		
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		
		#endregion
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone) {
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else {
				return true;
			}
		}
		
		#region Split View
		
		class SplitViewControllerDelegate : UISplitViewControllerDelegate
		{
			public override void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc)
			{
				var dv = svc.ViewControllers [1] as DetailViewController;
				barButtonItem.Title = "Master";
				var items = new List<UIBarButtonItem> ();
				items.Add (barButtonItem);
				items.AddRange (dv.toolbar.Items);
				dv.toolbar.SetItems (items.ToArray (), true);
				dv.popoverController = pc;
			}
			
			public override void WillShowViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem button)
			{
				var dv = svc.ViewControllers [1] as DetailViewController;
				var items = new List<UIBarButtonItem> (dv.toolbar.Items);
				items.RemoveAt (0);
				dv.toolbar.SetItems (items.ToArray (), true);
				dv.popoverController = null;
			}
		}
		
		#endregion
	}
}

