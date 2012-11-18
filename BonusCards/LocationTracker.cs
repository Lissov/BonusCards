using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BonusCards
{
    class LocationTracker : Java.Lang.Object, ILocationListener
    {
        LocationManager _locManager;
        Geocoder _geocoder;
        Activity _activity;

        public LocationTracker(Activity activity)
        {
            _activity = activity;
            Initialise();
        }

        public bool Initialise()
        {
            if (_locManager != null)
                return true;

            try
            {
                _locManager = _activity.GetSystemService(Context.LocationService) as LocationManager;
                _geocoder = new Geocoder(_activity);

                var criteria = new Criteria() { Accuracy = Accuracy.NoRequirement };
                string bestProvider = _locManager.GetBestProvider(criteria, true);

                var lastKnownLocation = _locManager.GetLastKnownLocation(bestProvider);

                if (lastKnownLocation != null)
                {
                    OnLocationUpdated(lastKnownLocation);
                }

                _locManager.RequestLocationUpdates(bestProvider, 5000, 2, this);
                _locManager.RequestLocationUpdates("gps", 5000, 2, this);

                return true;
            }
            catch (Exception)
            {
                Toast.MakeText(_activity, Resource.String.error_LocationUnavailable, ToastLength.Long).Show();
                return false;
            }
        }

        private Location _oldLocation = null;

        //public int LocationChangeDeltaM = 50;

        public event Action<LocationUpdateEventArgs> LocationUpdated;
        protected void OnLocationUpdated(Location newLocation)
        {
            //if (_oldLocation != null /*&& _oldLocation.DistanceTo(newLocation) <= LocationChangeDeltaM*/)
            //    return;

            _oldLocation = newLocation;
            var e = LocationUpdated;
            if (e != null)
                e(new LocationUpdateEventArgs() { 
                    Location = newLocation, 
                    Timestamp = DateTime.Now
                });
        }

        public string GetAddress(Location location)
        {
            try
            {
                var addr = _geocoder.GetFromLocation(location.Latitude, location.Longitude, 1).FirstOrDefault();
                if (addr == null)
                    return "Unavailable";

                var res = string.Format("{0}, {1}, {2}", addr.CountryName ?? "NoCtry", addr.PostalCode ?? "0000", addr.Locality ?? "--");
                res += " - " + location.Accuracy;
                return res;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }


        public void OnLocationChanged(Location location)
        {
            OnLocationUpdated(location);
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
    }

    public class LocationUpdateEventArgs
    {
        public Location Location { get; set; }
        public DateTime Timestamp { get; set; }
    }
}