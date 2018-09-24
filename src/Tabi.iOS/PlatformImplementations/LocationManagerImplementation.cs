using System;
using CoreLocation;
using Foundation;
using Tabi.Collection;
using Tabi.DataObjects;
using Tabi.DataObjects.CollectionProfile;
using Tabi.DataStorage;
using Tabi.Helpers;
using Tabi.iOS.Helpers;
using Tabi.iOS.PlatformImplementations;
using Tabi.Logging;
using Tabi.Resx;
using UserNotifications;
using Xamarin.Forms;

namespace Tabi.iOS.PlatformImplementations
{
    public class LocationManagerImplementation : ILocationManager
    {
        private readonly IRepoManager _repoManager;
        private readonly BatteryHelper _batteryHelper;

        private readonly CLLocationManager _clManager;
        private readonly CLLocationManager _listenLocationManager;
        private readonly CLLocationManager _significantLocationManager;

        private CoreLocationProfile _currentCoreLocationProfile;
        private ProfileiOS _currentProfile;

        private PositionEntry previousPositionEntry;
        private DateTime _lastLocation;

        private PositionCache _positionCache = new PositionCache();
        private bool _locationsDeferred = false;

        IMotionEntryRepository _motionEntryRepo;
        IPositionEntryRepository _positionEntryRepo;

        public bool IsListening { get; private set; }

        public LocationManagerImplementation(IRepoManager repoManager, ProfileiOS profile, BatteryHelper batteryHelper)
        {
            _currentProfile = profile ?? throw new ArgumentNullException(nameof(profile));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _batteryHelper = batteryHelper ?? throw new ArgumentNullException(nameof(batteryHelper));

            _significantLocationManager = new CLLocationManager();
            _significantLocationManager.LocationsUpdated += _significantLocationManager_LocationsUpdated;
            _significantLocationManager.AllowsBackgroundLocationUpdates = true;

            _clManager = new CLLocationManager();
            _clManager.LocationsUpdated += ClManager_LocationsUpdated;
            _clManager.RegionLeft += _clManager_RegionLeft;
            _clManager.DeferredUpdatesFinished += ClManagerOnDeferredUpdatesFinished;
            _clManager.AuthorizationChanged += ClManager_AuthorizationChanged;

            _listenLocationManager = new CLLocationManager();
            _clManager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;
            _clManager.ActivityType = CLActivityType.Fitness;

            _positionEntryRepo = _repoManager.PositionEntryRepository;
            _motionEntryRepo = _repoManager.MotionEntryRepository;
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
            _lastLocation = DateTime.Now;

            _batteryHelper.CheckStoreBatteryLevel(TimeSpan.FromMinutes(10));

            if (!_locationsDeferred && _currentCoreLocationProfile.AllowDeferredLocationUpdates)
            {
                _clManager.AllowDeferredLocationUpdatesUntil(_currentCoreLocationProfile.DeferredLocationUpdatesDistance,
                    _currentCoreLocationProfile.DeferredLocationUpdatesTimeout);
            }

            foreach (CLLocation l in e.Locations)
            {
                PositionEntry p = l.ToPosition();

                p.DesiredAccuracy = _clManager.DesiredAccuracy;

                _positionCache.Distance = _currentProfile.DistanceDeltaLowTracking;
                _positionCache.Add(p);

                p.DistanceBetweenPreviousPosition = _positionCache.PreviousDistance;

                // Check if new position is around the same location using distance and time period
                if (_positionCache.Check(TimeSpan.FromSeconds(_currentProfile.TimePeriodLowTracking)))
                {
                    // Enable Low Tracking if not already enabled
                    if (_currentCoreLocationProfile != _currentProfile.LowTrackingProfile)
                    {
                        Log.Info("LowTracking");
                        MonitorCurrentRegion(p.Latitude, p.Longitude, 100, "StopRegion");
                        _currentCoreLocationProfile = _currentProfile.LowTrackingProfile;
                        ApplyCoreLocationProfile(_currentCoreLocationProfile);
                    }
                }
                else if (_currentCoreLocationProfile != _currentProfile.HighTrackingProfile)
                {
                    Log.Info("HighTracking");
                    _currentCoreLocationProfile = _currentProfile.HighTrackingProfile;
                    ApplyCoreLocationProfile(_currentCoreLocationProfile);
                }


                // Do not store location twice if it has exact same properties.
                if (!p.Equals(previousPositionEntry))
                {
                    _positionEntryRepo.Add(p);
                }

                previousPositionEntry = p;
            }
        }

        void _clManager_RegionLeft(object sender, CLRegionEventArgs e)
        {
            _currentCoreLocationProfile = _currentProfile.HighTrackingProfile;
            ApplyCoreLocationProfile(_currentCoreLocationProfile);
        }


        void MonitorCurrentRegion(double lat, double lon, double radius, string identifier)
        {
            CLRegion region = new CLRegion(new CLLocationCoordinate2D(lat, lon), radius, identifier);
            region.NotifyOnExit = true;
            region.NotifyOnEntry = false;

            _clManager.StartMonitoring(region);
        }

        void RemoveRegionMonitoring()
        {
            foreach (CLRegion region in _clManager.MonitoredRegions)
            {
                _clManager.StopMonitoring(region);
            }
        }


        void _significantLocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            if (_lastLocation < DateTime.Now - TimeSpan.FromMinutes(15))
            {
                SendReminderNotification();
            }

        }

        private void Authorize()
        {
            _clManager.RequestAlwaysAuthorization();
        }

        private void SendReminderNotification()
        {
            var content = new UNMutableNotificationContent();
            content.Title = AppResources.StartAppReminderTitle;
            content.Body = AppResources.StartAppReminderText;

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(10, false);

            var requestID = "startapp";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {

                }
            });

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

            _currentCoreLocationProfile = _currentProfile.HighTrackingProfile;
            _clManager.StartUpdatingLocation();

            //_significantLocationManager.StartMonitoringSignificantLocationChanges();

            IsListening = true;
        }

        public void StopLocationUpdates()
        {
            _clManager.StopUpdatingLocation();
            _clManager.StopMonitoringSignificantLocationChanges();
            IsListening = false;
        }

        public event EventHandler<LocationEventArgs> LocationsUpdated;


        private void ApplyCoreLocationProfile(CoreLocationProfile profile)
        {
            _clManager.DesiredAccuracy = profile.DesiredAccuracy;
            _clManager.DistanceFilter = profile.DistanceFilter;
            _clManager.AllowsBackgroundLocationUpdates = profile.AllowBackgroundLocationUpdates;
            _clManager.PausesLocationUpdatesAutomatically = profile.PauseLocationUpdatesAutomatically;


            //if (profile.MonitorSignificantChanges)
            //{
            //    _clManager.StartMonitoringSignificantLocationChanges();
            //}
            //else
            //{
            //    _clManager.StopMonitoringSignificantLocationChanges();
            //}

            if (_locationsDeferred && !profile.AllowDeferredLocationUpdates)
            {
                _clManager.DisallowDeferredLocationUpdates();
                _locationsDeferred = false;
            }
        }
    }
}