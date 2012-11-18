using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;

namespace BonusCards
{
    [Activity(Label = "BonusCards", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ListView lvData;
        LocationTracker tracker;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            lvData = FindViewById<ListView>(Resource.Id.lvCards);

            ShowList();

            if (tracker == null)
            {
                tracker = new LocationTracker(this);
                tracker.LocationUpdated += tracker_LocationUpdated;
            }
        }

        void tracker_LocationUpdated(LocationUpdateEventArgs ev)
        {
            var tv = FindViewById<TextView>(Resource.Id.tvLocation);
            tv.Text = string.Format("Lat:{0}; Lon: {1}", ev.Location.Latitude, ev.Location.Longitude);
            var tva = FindViewById<TextView>(Resource.Id.tvLocationA);
            tva.Text = tracker.GetAddress(ev.Location);
        }

        public const int MenuItem_AddCard = 0;
        public const int MenuItem_Exit = 10;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var res = base.OnCreateOptionsMenu(menu);

            menu.Add(0, MenuItem_AddCard, 0, Resource.String.AddCard);
            menu.Add(0, MenuItem_Exit, 1, Resource.String.Exit);
                
            return res;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case MenuItem_AddCard:
                    StartActivity(typeof(EditCardActivity));
                    return true;
                case MenuItem_Exit:
                    System.Diagnostics.Debugger.Break();
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            ShowList();
        }

        public void ShowList()
        {
            try
            {
                var a = lvData.Adapter;
                lvData.Adapter = null;
                if (a != null)
                    a.Dispose();
                lvData.Adapter = new CardAdapter(CardManager.Instance.GetDistanceSorted(), this);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message, ToastLength.Long).Show();
            }
        }
    }
}

