using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tabi.Core;
using Tabi.DataObjects;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class AssignSensorDataToTrackService : Service
    {
        private AssignSensorDataToTrackServiceBinder _binder;
        
        public override IBinder OnBind(Intent intent)
        {
            _binder = new AssignSensorDataToTrackServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // service for resolve data per 5 minutes
            Timer timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
            timer.Start();

            return StartCommandResult.Sticky;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            DataResolver dataResolver = new DataResolver();
            dataResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);

            List<TrackEntry> trackEntries = App.RepoManager.TrackEntryRepository.GetAll().ToList();
            List<StopVisit> stopVisits = App.RepoManager.StopVisitRepository.GetAll().ToList();
            List<Stop> stops = App.RepoManager.StopRepository.GetAll().ToList();

            Console.WriteLine("Trackentries: " + trackEntries.Count);
            Console.WriteLine("stopvisits: " + stopVisits.Count);
            Console.WriteLine("stops: " + stops.Count);
        }
    }
}