using System;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Droid.CollectionService;
using Tabi.Shared.Collection;
using Debug = System.Diagnostics.Debug;
using ILocationListener = Android.Gms.Location.ILocationListener;
using Object = Java.Lang.Object;

namespace Tabi.Droid
{
    public class GoogleLocationServicesAPI : Object, IAndroidLocation, ILocationListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private GoogleApiClient client;
        private readonly IPositionEntryRepository positionEntryRepository = App.RepoManager.PositionEntryRepository;
        private readonly ProfileAndroid profile;
        private LocationRequest request;


        private Location previousLocation;

        public GoogleLocationServicesAPI()
        {
            profile = App.CollectionProfile.AndroidProfile;
        }

        public void RequestLocationUpdates()
        {
            Log.Info("GoogleApi: Requested location updates");

            client = GetGoogleAPIClient();
            client.Connect();
        }


        public void StopLocationUpdates()
        {
            LocationServices.FusedLocationApi.RemoveLocationUpdates(client, this);
        }

        void GoogleApiClient.IConnectionCallbacks.OnConnected(Bundle connectionHint)
        {
            Log.Info("GoogleApi: Connected!");
            LowTracking();
        }

        void GoogleApiClient.IConnectionCallbacks.OnConnectionSuspended(int cause)
        {
        }

        public void OnLocationChanged(Location location)
        {
            Debug.WriteLine(location);
            Log.Debug("GoogleApi: Location was changed!!");

            if (previousLocation?.DistanceTo(location) > 40)
            {
                
            }
            
            positionEntryRepository.Add(new PositionEntry
            {
                Accuracy = location.Accuracy,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Timestamp = DateTimeOffset.Now
            });
        }

        void GoogleApiClient.IOnConnectionFailedListener.OnConnectionFailed(ConnectionResult result)
        {
            Log.Info($"GoogleApi: Failed! {result.ErrorMessage}");
        }

        private void HighTracking()
        {
            Log.Info("HighTracking");
            StopLocationUpdates();
            request = CreateLocationRequest(profile.HighTrackingProfile);
            StartLocationUpdates();
        }

        private void LowTracking()
        {
            Log.Info("LowTracking");
            StopLocationUpdates();
            request = CreateLocationRequest(profile.LowTrackingProfile);
            StartLocationUpdates();
        }
        
        

        public void StartLocationUpdates()
        {
            LocationServices.FusedLocationApi.RequestLocationUpdates(client, request, this);
        }

        private GoogleApiClient GetGoogleAPIClient()
        {
            return new GoogleApiClient.Builder(Application.Context)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();
        }

        private LocationRequest CreateLocationRequest(LocationRequestProfile profile)
        {
            var request = new LocationRequest();
            request.SetPriority(profile.Priority);
            request.SetInterval(profile.Interval);

            return request;
        }
    }
}