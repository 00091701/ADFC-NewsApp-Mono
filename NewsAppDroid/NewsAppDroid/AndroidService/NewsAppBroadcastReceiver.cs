using System;
using Android.App;
using Android.Content;
using de.dhoffmann.mono.adfcnewsapp.buslog;
using Android.OS;

namespace de.dhoffmann.mono.adfcnewsapp.AndroidService
{
	[BroadcastReceiver]
	[IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted },
	 Categories = new[] { Android.Content.Intent.CategoryDefault } )]
	public class NewsAppBroadcastReceiver : BroadcastReceiver
	{
		public NewsAppBroadcastReceiver ()
		{
		}

		public override void OnReceive (Context context, Intent intent)
		{
			if (intent.Action != null && intent.Action.Equals(Intent.ActionBootCompleted)) 
			{
				Logging.Log(this, Logging.LoggingTypeDebug, "OnReceive()");

				AlarmManager alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);
				PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, new Intent(context, typeof(NewsAppAlarmService)), 0);
				alarmManager.SetInexactRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + 60, AlarmManager.IntervalHalfDay, pendingIntent);
			}
		}
	}
}

