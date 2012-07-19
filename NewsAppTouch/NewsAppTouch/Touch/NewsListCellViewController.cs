
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	public partial class NewsListCellViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public NewsListCellViewController ()
		{
			MonoTouch.Foundation.NSBundle.MainBundle.LoadNib (UserInterfaceIdiomIsPhone ? "NewsListCellViewController_iPhone" : "NewsListCellViewController_iPad", this, null);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone) {
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else {
				return true;
			}
		}


		public UITableViewCell Cell
		{
			get { return this.cellMain; }
		}

		public UILabel LbHeadline
		{
			get { return lbHeadline; }
			set { lbHeadline = value; }
		}

		public UILabel LbDate
		{
			get { return lbDate; }
			set { lbDate = value; }
		}

	}
}

