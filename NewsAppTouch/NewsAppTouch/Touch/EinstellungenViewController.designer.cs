// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	[Register ("EinstellungenViewController")]
	partial class EinstellungenViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch swAutoDownload { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tblEinstellungen { get; set; }


		void ReleaseDesignerOutlets ()
		{
			if (swAutoDownload != null) {
				swAutoDownload.Dispose ();
				swAutoDownload = null;
			}

			if (tblEinstellungen != null) {
				tblEinstellungen.Dispose ();
				tblEinstellungen = null;
			}

		}
	}
}
