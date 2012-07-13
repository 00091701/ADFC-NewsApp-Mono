using System;
using Android.Content;
using Android.Widget;
using de.dhoffmann.mono.adfcnewsapp.buslog;
using de.dhoffmann.mono.adfcnewsapp.buslog.database;

namespace de.dhoffmann.mono.adfcnewsapp.AndroidService
{
	[BroadcastReceiver]
	public class NewsAppAlarmService : BroadcastReceiver
	{
		public NewsAppAlarmService ()
		{
		}

		#region implemented abstract members of Android.Content.BroadcastReceiver
		public override void OnReceive (Context context, Intent intent)
		{
			Logging.Log(this, Logging.LoggingTypeDebug, "OnReceive()");

			AppConfig appConfig = new Config(this).GetAppConfig();

			if (appConfig.DataAutomaticUpdate)
				new FeedHelper().UpdateBGFeeds(null);
		}
		#endregion

	}
}

