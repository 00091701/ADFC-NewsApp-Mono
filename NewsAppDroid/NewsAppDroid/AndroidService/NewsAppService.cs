
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
using de.dhoffmann.mono.adfcnewsapp.droid.buslog;

namespace de.dhoffmann.mono.adfcnewsapp.AndroidService
{
	[Service]
	public class NewsAppService : Service
	{
		private IBinder binder;

		public NewsAppService ()
		{
			binder = new NewsAppBinder (this);
		}

		// how's this work?
		public class NewsAppBinder : Binder 
		{
			NewsAppService self;

			public NewsAppBinder (NewsAppService self)
			{
				this.self = self;
			}

			public NewsAppService Service 
			{
				get 
				{
					return self;
				}
			}
		}

		#region implemented abstract members of Android.App.Service
		public override IBinder OnBind (Intent intent)
		{
			return binder;
		}
		#endregion


		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			/*
			while (true) 
			{
				System.Threading.Thread.Sleep(10*1000);
				System.Diagnostics.Debug.WriteLine("####################### Service - PING ###");
			}
*/

			Logging.Log(this, Logging.LoggingTypeDebug, "LocalService _Received start id " + startId + ": " + intent);

			return StartCommandResult.Sticky;
		}


		public override void OnStart (Intent intent, int startId)
		{
			base.OnStart (intent, startId);
			System.Diagnostics.Debug.WriteLine("####################### Service - OnStart ###");

		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			System.Diagnostics.Debug.WriteLine("####################### Service - OnStop ###");
		}
	}
}

