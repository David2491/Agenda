using Agenda.SQL_Classes;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Support.V4.App;
using Java.Lang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AndroidX.Core.App;

namespace Agenda;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private IHostedService backgroundTask;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        // creates and starts the background task
        base.OnCreate(savedInstanceState);
        backgroundTask = new BackgroundTask();
        backgroundTask.StartAsync(CancellationToken.None);

        //For Android 8.0 and up
        string channelId = "Agenda.Notification.channel";

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            NotificationChannel channel = new NotificationChannel(channelId, "Channel Name", NotificationImportance.Default);
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //stops the background task when the app is destoryed
        backgroundTask.StopAsync(CancellationToken.None);
    }

}
