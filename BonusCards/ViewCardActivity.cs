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
    [Activity(Label = "My Activity")]
    public class ViewCardActivity : Activity
    {
        private ImageView _image;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ViewCard);

            var cardName = this.Intent.GetStringExtra(Constants.EXTRA_CARD_NAME);
            _imageNum = this.Intent.GetIntExtra(Constants.EXTRA_IMAGE_NUMBER, 0);
            _data = CardManager.Instance.GetCard(cardName);
            if (_data == null)
            {
                Finish();
                return;
            }

            _image = FindViewById<ImageView>(Resource.Id.cardImageView);
            ShowImage();
        }

        private CardData _data;
        private int _imageNum;

        private void ShowImage()
        {
            try
            {
                _image.SetImageURI(Android.Net.Uri.Parse(_data.Images[_imageNum]));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var b = base.OnCreateOptionsMenu(menu);
            menu.Add(1, 0, 1, Resource.String.viewcard_Left);
            menu.Add(1, 1, 1, Resource.String.viewcard_Edit);
            menu.Add(1, 2, 1, Resource.String.viewcard_Right);
            return b;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.GetItem(0).SetEnabled(_imageNum > 0);
            menu.GetItem(2).SetEnabled(_imageNum < _data.Images.Count - 1);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    if (_imageNum > 0)
                        _imageNum--;
                    ShowImage();
                    return true;
                case 1:
                    var intent = new Intent(this, typeof(EditCardActivity));
                    intent.PutExtra(Constants.EXTRA_CARD_NAME, _data.Name);
                    this.StartActivity(intent);
                    return true;
                case 2:
                    if (_imageNum < _data.Images.Count - 1)
                        _imageNum++;
                    ShowImage();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}