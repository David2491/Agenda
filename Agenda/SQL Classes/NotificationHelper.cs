using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace Agenda.SQL_Classes
{
    public static class NotificationHelper
    {
        private static readonly string channelId = "Agenda.Notification.channel";

        public static void ShowNotification(string title, string message)
        {
            int notificationId = 1; //Notification ID

            //An intent is an operation that will be performed
            Intent notificationIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));
            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);


            PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, 0, notificationIntent, PendingIntentFlags.Immutable);

            long[] pattern = new long[] {500, 500};
            // Creates the notification with the passed value of the title and message
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
                .SetSmallIcon(Resource.Drawable.dotnet_bot)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetVibrate(pattern)
                .SetContentIntent(pendingIntent);

            NotificationManager notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

            // Shows the notification
            notificationManager.Notify(notificationId, builder.Build());
        }
    }
}
