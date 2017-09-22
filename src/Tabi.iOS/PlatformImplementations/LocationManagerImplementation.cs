using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.iOS.Helpers;
using Tabi.iOS.PlatformImplementations;
using Tabi.Shared.Collection;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocationManagerImplementation))]

namespace Tabi.iOS.PlatformImplementations
{
    public class LocationManagerImplementation : ILocationManager
    {
        CLLocationManager clManager;
        private CoreLocationProfile currentCoreLocationProfile;
        private ProfileiOS currentProfile;

        private PositionCache positionCache = new PositionCache();
        private bool locationsDeferred = false;

        IMotionEntryRepository motionEntryRepo;
        IPositionEntryRepository positionEntryRepo;


        public bool IsListening { get; private set; }

        public LocationManagerImplementation()
        {
            currentProfile = App.CollectionProfile.iOSProfile;

            clManager = new CLLocationManager();
            clManager.LocationsUpdated += ClManager_LocationsUpdated;
            clManager.DeferredUpdatesFinished += ClManagerOnDeferredUpdatesFinished;
            clManager.AuthorizationChanged += ClManager_AuthorizationChanged;

            positionEntryRepo = App.RepoManager.PositionEntryRepository;
            motionEntryRepo = App.RepoManager.MotionEntryRepository;
        }

        private void ClManagerOnDeferredUpdatesFinished(object sender, NSErrorEventArgs nsErrorEventArgs)
        {
            Log.Debug("ClManagerOnDeferredUpdatesFinished");
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
            if (!locationsDeferred && currentCoreLocationProfile.AllowDeferredLocationUpdates)
            {
                clManager.AllowDeferredLocationUpdatesUntil(currentCoreLocationProfile.DeferredLocationUpdatesDistance,
                    currentCoreLocationProfile.DeferredLocationUpdatesTimeout);
            }
            
            Log.Debug($"LocationsUpdated called with {e.Locations.Length} locationUpdates");
            foreach (CLLocation l in e.Locations)
            {
                PositionEntry p = l.ToPosition();
                p.DesiredAccuracy = clManager.DesiredAccuracy;
                
                positionCache.Distance = currentProfile.DistanceDeltaLowTracking;
                positionCache.Add(p);
                
                p.DistanceBetweenPreviousPosition = positionCache.PreviousDistance;

                // Check if new position is around the same location using distance and time period
                if (positionCache.Check(TimeSpan.FromSeconds(currentProfile.TimePeriodLowTracking)))
                {
                    // Enable Low Tracking if not already enabled
                    if (currentCoreLocationProfile != currentProfile.LowTrackingProfile)
                    {
                        Log.Info("LowTracking");
                        currentCoreLocationProfile = currentProfile.LowTrackingProfile;
                        ApplyCoreLocationProfile(currentCoreLocationProfile);
                    }
                }
                else if(currentCoreLocationProfile != currentProfile.HighTrackingProfile)
                {
                    Log.Info("HighTracking");
                    currentCoreLocationProfile = currentProfile.HighTrackingProfile;
                    ApplyCoreLocationProfile(currentCoreLocationProfile);

                }
 
                positionEntryRepo.Add(p);
            }
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

            currentCoreLocationProfile = currentProfile.HighTrackingProfile;
            clManager.StartUpdatingLocation();

            IsListening = true;
        }

        public void StopLocationUpdates()
        {
            clManager.StopUpdatingLocation();
            clManager.StopMonitoringSignificantLocationChanges();
            IsListening = false;
        }

        public event EventHandler<LocationEventArgs> LocationsUpdated;


        private void ApplyCoreLocationProfile(CoreLocationProfile profile)
        {
            clManager.DesiredAccuracy = profile.DesiredAccuracy;
            clManager.DistanceFilter = profile.DistanceFilter;
            clManager.AllowsBackgroundLocationUpdates = profile.AllowBackgroundLocationUpdates;
            clManager.PausesLocationUpdatesAutomatically = profile.PauseLocationUpdatesAutomatically;

            
            if (profile.MonitorSignificantChanges)
            {
                clManager.StartMonitoringSignificantLocationChanges();
            }
            else
            {
                clManager.StopMonitoringSignificantLocationChanges();
            }

            if (locationsDeferred && !profile.AllowDeferredLocationUpdates)
            {
                clManager.DisallowDeferredLocationUpdates();
                locationsDeferred = false;
            }
        }
    }
}