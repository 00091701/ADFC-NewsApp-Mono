// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	[Register ("NewsListCellViewController")]
	partial class NewsListCellViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableViewCell cellMain { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lbHeadline { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lbDate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (cellMain != null) {
				cellMain.Dispose ();
				cellMain = null;
			}

			if (lbHeadline != null) {
				lbHeadline.Dispose ();
				lbHeadline = null;
			}

			if (lbDate != null) {
				lbDate.Dispose ();
				lbDate = null;
			}
		}
	}
}
