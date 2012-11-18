using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BonusCards
{
    public class CardAdapter : BaseAdapter
    {
        List<CardData> _data;
        Activity _context;

        public CardAdapter(List<CardData> data, Activity context)
        {
            _data = data;
            _context = context;
        }

        public override int Count
        {
            get { return _data.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.CardItem, null);

            var iv = view.FindViewById<ImageView>(Resource.Id.ivCardThumbnail);
            var dt = _data[position];
            if (dt.Images != null && dt.Images.Count > 0)
            {
                try
                {
                    iv.SetImageURI(Android.Net.Uri.Parse(dt.Images[0]));
                } catch
                {
                    iv.SetImageResource(Resource.Drawable.NoCardImage);
                }
            }
            else
                iv.SetImageResource(Resource.Drawable.NoCardImage);
            
            var tvName = view.FindViewById<TextView>(Resource.Id.tvCardName);
            tvName.Text = dt.Name;
            var tvShopName = view.FindViewById<TextView>(Resource.Id.tvShopName);
            tvShopName.Text = dt.ShopName;
            var tvDist = view.FindViewById<TextView>(Resource.Id.tvDistance);
            tvDist.Text = GetDistanceString(dt.DistanceInMeters);

            view.Click += (s, e) => ShowData(dt);
            return view;
        }

        private void ShowData(CardData data)
        {
            if (data.Images.Count > 0)
            {
                var intent = new Intent(_context, typeof(ViewCardActivity));
                intent.PutExtra(Constants.EXTRA_CARD_NAME, data.Name);
                intent.PutExtra(Constants.EXTRA_IMAGE_NUMBER, 0);
                _context.StartActivity(intent);
            }
            else
            {
                var intent = new Intent(_context, typeof(EditCardActivity));
                intent.PutExtra(Constants.EXTRA_CARD_NAME, data.Name);
                _context.StartActivity(intent);
            }
        }

        private string GetDistanceString(int? meters)
        {
            if (meters == null)
                return _context.Resources.GetString(Resource.String.distance_Unknown);
            if (meters <= 20)
                return _context.Resources.GetString(Resource.String.distance_Here);

            if (meters <= 200)
                return (meters / 10) * 10 + " " + _context.Resources.GetString(Resource.String.distance_M);
            if (meters <= 200)
                return (meters / 50) * 50 + " " + _context.Resources.GetString(Resource.String.distance_M);
            if (meters < 10000)
                return Math.Round((decimal)meters / 1000, 1) + " " + _context.Resources.GetString(Resource.String.distance_KM);

            return Math.Round((decimal)meters / 1000) + " " + _context.Resources.GetString(Resource.String.distance_KM);
        }
    }
}