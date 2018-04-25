using System;
using CoreLocation;
using Foundation;
using Tabi.DataObjects;
using Tabi.DataObjects.CollectionProfile;
using Tabi.DataStorage;
using Tabi.iOS.Helpers;
using Tabi.iOS.PlatformImplementations;
using Tabi.Shared.Collection;
using Tabi.Shared.Helpers;
using Xamarin.Forms;

namespace Tabi.iOS.PlatformImplementations
{
    public class LocationManagerImplementation : ILocationManager
    {
        private readonly CLLocationManager _clManager;
        private readonly IRepoManager _repoManager;
        private readonly BatteryHelper _batteryHelper;

        private CoreLocationProfile _currentCoreLocationProfile;
        private ProfileiOS _currentProfile;

        private PositionCache _positionCache = new PositionCache();
        private bool _locationsDeferred = false;

        IMotionEntryRepository _motionEntryRepo;
        IPositionEntryRepository _positionEntryRepo;

        public bool IsListening { get; private set; }

        public LocationManagerImplementation(CLLocationManager cLLocationManager, IRepoManager repoManager, ProfileiOS profile, BatteryHelper batteryHelper)
        {
            _currentProfile = profile ?? throw new ArgumentNullException(nameof(profile));
            _clManager = cLLocationManager ?? throw new ArgumentNullException(nameof(cLLocationManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _batteryHelper = batteryHelper ?? throw new ArgumentNullException(nameof(batteryHelper));

            _clManager.LocationsUpdated += ClManager_LocationsUpdated;
            _clManager.DeferredUpdatesFinished += ClManagerOnDeferredUpdatesFinished;
            _clManager.AuthorizationChanged += ClManager_AuthorizationChanged;

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
            _batteryHelper.CheckStoreBatteryLevel(TimeSpan.FromMinutes(10));

            if (!_locationsDeferred && _currentCoreLocationProfile.AllowDeferredLocationUpdates)
            {
                _clManager.AllowDeferredLocationUpdatesUntil(_currentCoreLocationProfile.DeferredLocationUpdatesDistance,
                    _currentCoreLocationProfile.DeferredLocationUpdatesTimeout);
            }

            Log.Debug($"LocationsUpdated called with {e.Locations.Length} locationUpdates");
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

                _positionEntryRepo.Add(p);
            }
        }


        private void Authorize()
        {
            _clManager.RequestAlwaysAuthorization();
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


            if (profile.MonitorSignificantChanges)
            {
                _clManager.StartMonitoringSignificantLocationChanges();
            }
            else
            {
                _clManager.StopMonitoringSignificantLocationChanges();
            }

            if (_locationsDeferred && !profile.AllowDeferredLocationUpdates)
            {
                _clManager.DisallowDeferredLocationUpdates();
                _locationsDeferred = false;
            }
        }
    }
}