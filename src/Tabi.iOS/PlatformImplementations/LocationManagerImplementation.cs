using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using CoreMotion;
using Foundation;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.iOS.Helpers;
using Tabi.iOS.PlatformImplementations;
using Tabi.Logging;
using Tabi.Model;
using Tabi.Shared.Collection;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LocationManagerImplementation))]
namespace Tabi.iOS.PlatformImplementations
{
    public class LocationManagerImplementation : ILocationManager
    {
        CLLocationManager clManager;
        ProfileiOS collectionProfile;

        bool lowPriority = false;
        Repository<Position> posRepo;
        IMotionEntryRepository motionEntryRepo;
        IPositionEntryRepository positionEntryRepo;

        PositionEntry previousPosition;

        public bool IsListening { get; private set; }

        public LocationManagerImplementation()
        {
            collectionProfile = App.CollectionProfile.iOSProfile;

            clManager = new CLLocationManager();
            clManager.LocationsUpdated += ClManager_LocationsUpdated;

            clManager.AuthorizationChanged += ClManager_AuthorizationChanged;

            positionEntryRepo = App.RepoManager.PositionEntryRepository;
            motionEntryRepo = App.RepoManager.MotionEntryRepository;

        }

        void ClManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            if (e.Status == CLAuthorizationStatus.AuthorizedAlways)
            {
                StartLocationUpdates();
            }
        }

        private void ClManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            //Log.Debug("ClManager_LocationsUpdated: " + e.Locations.Length);
            ValidateWiFi();
            List<PositionEntry> positions = new List<PositionEntry>();

            foreach (CLLocation l in e.Locations)
            {
                PositionEntry p = l.ToPosition();
                p.DesiredAccuracy = clManager.DesiredAccuracy;
                if (previousPosition != null)
                {
                    p.DistanceBetweenPreviousPosition = p.DistanceTo(previousPosition);

                }
                positions.Add(p);
                ProcessLocation(p);
                previousPosition = p;
            }

            LocationsUpdated?.Invoke(this, new LocationEventArgs() { Positions = positions });
        }



        private void ProcessLocation(PositionEntry p)
        {
            List<PositionEntry> entries = positionEntryRepo.TakeLast(5);

            TimeSpan timeSinceLastPosition = TimeSpan.FromSeconds(0);
            if (entries.Count > 0)
            {
                timeSinceLastPosition = entries.Last().Timestamp - DateTimeOffset.Now;
            }

            if (timeSinceLastPosition > TimeSpan.FromMinutes(5))
            {
                Log.Warn($"{timeSinceLastPosition.TotalMinutes} minutes between position updates");
            }

            bool movedSign = false;
            foreach (PositionEntry oP in entries)
            {
                //Debug.WriteLine("Distance: " + p.DistanceTo(oP) + " " + arg.Result.Count);
                if (p.DistanceTo(oP) > collectionProfile.DistanceDeltaLimitLowTracking && p.Accuracy < 100)
                {
                    movedSign = true;
                    break;
                }
            }
            if (movedSign && lowPriority)
            {
                lowPriority = false;
                HighAccuracy();
            }
            else if (!movedSign && !lowPriority)
            {
                lowPriority = true;
                LowAccuracy();
            }

            positionEntryRepo.Add(p);
        }


        public event EventHandler<LocationEventArgs> LocationsUpdated;

        private void ValidateWiFi()
        {

        }


        private void LowAccuracy()
        {
            Log.Debug("LowAccuracy");
            // Set to 3000, should actually get locationupdates around ~70m
            // accuracy if wifi is enabled with accesspoints nearby.
            ApplyCoreLocationProfile(collectionProfile.LowTrackingProfile);
        }

        private void HighAccuracy()
        {
            Log.Debug("HighAccuracy");
            ApplyCoreLocationProfile(collectionProfile.HighTrackingProfile);

        }

        private void Authorize()
        {
            clManager.RequestAlwaysAuthorization();
        }

        public void StartLocationUpdates()
        {
            Authorize();
            bool invalidPermissions = CLLocationManager.Status != CLAuthorizationStatus.AuthorizedAlways;
            bool trackingAllowed = Settings.Current.Tracking;
            if (invalidPermissions || !trackingAllowed)
            {
                return;
            }

            HighAccuracy();
            clManager.StartUpdatingLocation();

            // Also listen to significant changes which are sent even if the
            // app is not in memory (/app switcher).
            IsListening = true;
        }

        public void StopLocationUpdates()
        {
            clManager.StopUpdatingLocation();
            clManager.StopMonitoringSignificantLocationChanges();
            IsListening = false;
        }


        private void ApplyCoreLocationProfile(CoreLocationProfile profile)
        {
            clManager.DesiredAccuracy = profile.DesiredAccuracy;
            clManager.DistanceFilter = profile.DistanceFilter;
            clManager.AllowsBackgroundLocationUpdates = profile.AllowBackgroundLocationUpdates;
            if (profile.MonitorSignificantChanges)
            {
                clManager.StartMonitoringSignificantLocationChanges();
            }
            else
            {
                clManager.StopMonitoringSignificantLocationChanges();
            }
            clManager.PausesLocationUpdatesAutomatically = clManager.PausesLocationUpdatesAutomatically;

            if (profile.AllowDeferredLocationUpdates)
            {
                clManager.AllowDeferredLocationUpdatesUntil(profile.DeferredLocationUpdatesDistance, profile.DeferredLocationUpdatesTimeout);
            }
            else
            {
                clManager.DisallowDeferredLocationUpdates();
            }
        }
    }
}
