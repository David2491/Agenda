using Microsoft.Extensions.Hosting; //using the packagae
using Android.Net;
using Plugin.Maui.Audio;
using Android.Content.Res;

namespace Agenda.SQL_Classes
{
    public class BackgroundTask : IHostedService
    {
        private Timer timer;
        public Task StartAsync(CancellationToken token)//starts background task
        {
            timer = new Timer(TimeChecker, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));//repeats every minute
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken token)//stops background task when app is closed
        {
            timer.Dispose();
            return Task.CompletedTask;
        }
        private async void TimeChecker(object state)//main code for background task
        {
            SQLiteTransfer transfer = new SQLiteTransfer();
            var nextEvent = transfer.GetMostRecentEvent();
            if (nextEvent == null)
            {
                return; //code will break if the table is empty
            }

            long time;
            if(nextEvent.IsVariable == true) //if the event is variable
            {
                time = TimeFinder(nextEvent); //the variable time will be used
            }
            else
            {
                time = nextEvent.StartTime + nextEvent.Date; //otehrwise, the regular start time will be used
            }

            if (DateTime.Now.Ticks > time)
            {
                if (nextEvent.IsAlarm) // for alarms it will play the alarm sound file
                {
                    NotificationHelper.ShowNotification(nextEvent.Title, nextEvent.Description);
                    var audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("AlarmSound.mp3"));
                    audioPlayer.Play();
                }
                else // for notifications it will only show the notification
                {
                    NotificationHelper.ShowNotification(nextEvent.Title, nextEvent.Description);
                }
                switch (nextEvent.Recurring)
                {
                    case 0: //if never recurring, delete the event
                        transfer.DeleteRecord(nextEvent);
                        break;
                    case 1:
                        transfer.IncrementEventDate(1, nextEvent.Id);
                        break;
                    case 2:
                        transfer.IncrementEventDate(7, nextEvent.Id);
                        break;
                    case 3:
                        transfer.IncrementEventDate(30, nextEvent.Id);
                        break;
                    case 4:
                        transfer.IncrementEventDate(365, nextEvent.Id);
                        break;
                }
            }
        }
        private long TimeFinder(EventObject evnt)
        {
            string[] RouteData = evnt.RouteId.Split('|');
            int accurateJourneyTime = MapsHelper.GetRouteTime(evnt).Result;
            if (accurateJourneyTime != -1)
            {
                return (evnt.StartTime + evnt.Date) - Math.BigMul(accurateJourneyTime, 10000000);//expected start time subtract accurate duration in ticks
            }
            else
            {
                return (evnt.StartTime + evnt.Date) - Math.BigMul(Convert.ToInt32(RouteData[4]), 10000000);//expected start time subtract less accurate duration in ticks
            }
        }
    }
}
