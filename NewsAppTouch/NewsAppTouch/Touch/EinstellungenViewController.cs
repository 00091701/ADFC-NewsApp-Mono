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
using De.Dhoffmann.Mono.Adfcnewsapp.IosHelper;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;
using System.ComponentModel;
using de.dhoffmann.mono.adfcnewsapp.buslog;

namespace De.Dhoffmann.Mono.Adfcnewsapp.Touch
{
	public partial class EinstellungenViewController : UIViewController
	{
		public NewsListViewController ScrNewsListVC;
		private EinstellungenListDataSource dsEinstellungenList;
		public EinstellungenViewController (IntPtr handle) : base (handle)
		{
		}		


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (new Config(this).GetAppConfig().AppIsConfigured)
				BindMyData();
			else
			{
				BackgroundWorker bgWorker = new BackgroundWorker();

                bgWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
                {
					new FeedHelper(ScrNewsListVC).UpdateFeeds();
                };

                bgWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                {
					BindMyData();
				};

				BindMyData();
    			bgWorker.RunWorkerAsync();

				string msg = "In diesem Moment wird die Basiskonfiguration von einem Webserver geladen.\n\n" +
                             	"Warten Sie daher bitte einen kleinen Moment.\nAnschließend können Sie " +
                                "Termine und Neuigkeiten von verschiedenen Ortgruppen abonnieren.\n\n" +
                                "Die Einstellungen werden automatisch beim verlassen der Konfiguration gespeichert." +
                                "\n\nBitte drücken Sie jetzt die 'Zurück-Taste' und warten einen Moment." +
                                "\n\nNachdem Sie die gewünschten Einstellungen vorgenommen haben, drücken Sie bitte " +
                                "nochmal auf die 'Zurück-Taste' die gewünschten Nachrichten werden dann aktualisiert und " +
								"stehen kurz danach zur Verfügung.";
				using (UIAlertView alert = new UIAlertView("Hinweis", msg, null, "Ok", null))
				{
					alert.Show();
				}
			}
		}



		private void BindMyData()
		{
			dsEinstellungenList = new EinstellungenListDataSource();
			dsEinstellungenList.LoadData();
			tblEinstellungen.DataSource = dsEinstellungenList;
			tblEinstellungen.ReloadData();

			AppConfig appConfig = new Config(this).GetAppConfig();

			swAutoDownload.On = appConfig.DataAutomaticUpdate;
		}


		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			dsEinstellungenList.SaveData();

			AppConfig appConfig = new AppConfig() 
			{
				AppIsConfigured = true,
				DataAutomaticUpdate = swAutoDownload.On,
				DateIndicate = false
			};

			new Config(this).SetAppConfig(appConfig);

			if (NavigationController.TopViewController.GetType() == typeof(NewsListViewController))
			{
				((NewsListViewController)NavigationController.TopViewController).UpDateFeeds();
			}
		}
	}
}
