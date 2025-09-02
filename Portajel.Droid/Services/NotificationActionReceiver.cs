using Android.App;
using Android.Content;
using Android.OS;
using Portajel.Droid;
using Portajel.Droid.Services;

namespace Portajel.Services;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(new[] { NotificationActions.ACTION_NOTIFICATION_TAPPED, NotificationActions.ACTION_NOTIFICATION_DELETED })]
public class NotificationActionReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent == null || intent.Action == null)
            return;

        switch (intent.Action)
        {
            case NotificationActions.ACTION_NOTIFICATION_TAPPED:
                var openIntent = new Intent(context, typeof(MainActivity));
                openIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);
                context.StartActivity(openIntent);
                break;
            case NotificationActions.ACTION_NOTIFICATION_DELETED:
                // Handle dismiss: stop your service or cancel work
                var stopIntent = new Intent(context, typeof(DroidService));
                stopIntent.SetAction("ACTION_NOTIFICATION_DISMISSED");
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    context.StartForegroundService(stopIntent);
                else
                    context.StartService(stopIntent);
                break;
        }
    }
}

public static class NotificationActions
{
    public const string ACTION_NOTIFICATION_TAPPED = "com.portajel.ACTION_NOTIFICATION_TAPPED";
    public const string ACTION_NOTIFICATION_DELETED = "com.portajel.ACTION_NOTIFICATION_DELETED";
}