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
using Android.Provider;
using Android.Media;
using System.IO;
using Android.Graphics;

namespace BonusCards
{
    [Activity(Label = "My Activity")]
    public class EditCardActivity : Activity
    {
        //private CardData data;

        private StorageManager storage = new StorageManager();
        /*ImageView ivFore;
        ImageView ivBack;*/
        LinearLayout llNoImage;
        LinearLayout llImages;
        Button btnAddImage;
        Button btnRemoveImage;
        Button btnMoveLeft;
        Button btnMoveRight;
        EditText etCardName;
        EditText etShopName;

        List<ImageData> images;
        string _currentCardDir;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AddCard);

            Button btnApply = FindViewById<Button>(Resource.Id.btnApply);
            Button btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            etCardName = FindViewById<EditText>(Resource.Id.etCardName);
            etShopName = FindViewById<EditText>(Resource.Id.etShopName);

            btnCancel.Click += (s, e) => this.Finish();
            btnApply.Click += btnApply_Click;

            images = new List<ImageData>();

            llNoImage = FindViewById<LinearLayout>(Resource.Id.rlNoImage);
            var imgNoImage = FindViewById<ImageView>(Resource.Id.ivNoImage);
            imgNoImage.SetImageResource(Resource.Drawable.NoCardImage);
            imgNoImage.LayoutParameters = new LinearLayout.LayoutParams(
                Constants.PreviewCard.ImageWidth, Constants.PreviewCard.ImageHeight);
            var p = Constants.PreviewCard.BorderSize;
            llNoImage.SetPadding(p, p, p, p);
            llNoImage.SetBackgroundColor(Constants.PreviewCard.NoImageColor);

            llImages = FindViewById<LinearLayout>(Resource.Id.llCardImages);

            btnAddImage = FindViewById<Button>(Resource.Id.btnAddImage);
            btnRemoveImage = FindViewById<Button>(Resource.Id.btnRemoveImage);
            btnMoveLeft = FindViewById<Button>(Resource.Id.btnMoveLeft);
            btnMoveRight = FindViewById<Button>(Resource.Id.btnMoveRight);
            btnAddImage.Click += (s, e) => MakePictureAndAdd();

            btnRemoveImage.Click += btnRemoveImage_Click;
            btnMoveLeft.Click += btnMoveLeft_Click;
            btnMoveRight.Click += btnMoveRight_Click;

            CardData dt = null;
            if (this.Intent.HasExtra(Constants.EXTRA_CARD_NAME))
            {
                var cname = this.Intent.GetStringExtra(Constants.EXTRA_CARD_NAME);
                if (!string.IsNullOrEmpty(cname))
                    dt = CardManager.Instance.GetCard(cname);             
            }
            if (dt != null)
                ShowData(dt);
            else
                ShowData(null);
            SetButtonsEnabled();
            CheckShowNoImage();
        }

        private void ShowData(CardData data)
        {
            if (data == null)
            {
                _currentCardDir = storage.GetTempImageDir();
                etCardName.Text = this.GetString(Resource.String.addcard_cardName);
                etShopName.Text = this.GetString(Resource.String.addcard_shopName);
            }
            else
            {
                _currentCardDir = storage.GetDataFolderName(data);
                foreach (var item in data.Images)
                {
                    AddImage(item);
                }
                etCardName.Text = data.Name;
                etShopName.Text = data.ShopName;
            }
        }

        void btnMoveRight_Click(object sender, EventArgs e)
        {
            var imgIndex = images.FindIndex(i => i.Selected);
            if (imgIndex >= 0 && imgIndex < images.Count - 1)
            {
                var image = images[imgIndex];
                images.RemoveAt(imgIndex);
                images.Insert(imgIndex + 1, image);
            }
            ReshowOrderedItems();
            CheckShowNoImage();
        }

        void btnMoveLeft_Click(object sender, EventArgs e)
        {
            var imgIndex = images.FindIndex(i => i.Selected);
            if (imgIndex > 0)
            {
                var image = images[imgIndex];
                images.RemoveAt(imgIndex);
                images.Insert(imgIndex - 1, image);
            }
            ReshowOrderedItems();
            CheckShowNoImage();
        }

        private void ReshowOrderedItems()
        {
            llImages.RemoveAllViews();
            foreach (var image in images)
            {
                llImages.AddView(image.layoutWithImage);
            }
        }

        void btnRemoveImage_Click(object sender, EventArgs e)
        {
            var image = images.FirstOrDefault(i => i.Selected);
            if (image != null)
            {
                images.Remove(image);
                llImages.RemoveView(image.layoutWithImage);
            }
            CheckShowNoImage();
        }

        protected void SetButtonsEnabled()
        {
            //var imageSelected = images.Any(i => i.Selected);
            var imgIndex = images.FindIndex(i => i.Selected);
            btnRemoveImage.Enabled = imgIndex != -1;
            btnMoveRight.Enabled = imgIndex != -1 && imgIndex < images.Count - 1;
            btnMoveLeft.Enabled = imgIndex != -1 && imgIndex > 0;
        }

        protected void MakePictureAndAdd()
        {
            /*if (!PackageManager.HasSystemFeature(PackageManager.FeatureCamera))
            {
                Toast.MakeText(this, "No camera detected on device", ToastLength.Long).Show();
                return;
            }*/
            /*var photoIntent = new Intent(MediaStore.ActionImageCapture);
            _currentPictureFile = storage.GetTempImageFileName();
            photoIntent.PutExtra(MediaStore.ExtraOutput, _currentPictureFile);
            StartActivityForResult(photoIntent, Constants.TAKE_PICTURE);*/
            
            /*var dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory
                    (Android.OS.Environment.DirectoryDcim), "crime");*/

            var uri = ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri,
                                 new ContentValues());

            //var _file = new Java.IO.File(@"/mnt/.bonuscards", "mypic.jpg");

            var intent = new Intent(MediaStore.ActionImageCapture);
            //intent.PutExtra(MediaStore.ExtraOutput, uri);
            //_currentPictureFile = @"/mnt/.bonuscards/mypic.jpg";
            StartActivityForResult(intent, Constants.TAKE_PICTURE);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != Constants.TAKE_PICTURE)
            {
                base.OnActivityResult(requestCode, resultCode, data);
                return;
            }

            if (resultCode != Result.Ok)
                return;

            Android.Net.Uri imageUri = null;
            if (data != null && !string.IsNullOrEmpty(data.DataString))
            {
                imageUri = Android.Net.Uri.Parse(data.DataString);      //full uri to where the image is on the device.

                var loc = GetRealPathFromURI(imageUri);
                var fn = storage.GetNextFileName(_currentCardDir);
                File.Move(loc, fn);
                //var uri = Android.Net.Uri.Parse(new Java.IO.File(fn));                
                AddImage(Compress(fn));
            }
        }

        public string GetRealPathFromURI(Android.Net.Uri contentUri)
        {
            var mediaStoreImagesMediaData = "_data";
            string[] projection = { mediaStoreImagesMediaData };
            Android.Database.ICursor cursor = this.ManagedQuery(contentUri, projection,
                                                                null, null, null);
            int columnIndex = cursor.GetColumnIndexOrThrow(mediaStoreImagesMediaData);
            cursor.MoveToFirst();
            return cursor.GetString(columnIndex);
        }

        private string Compress(string uncompressedFile)
        {
            try
            {
                var bitmap = BitmapFactory.DecodeFile(uncompressedFile);
                var compr = Bitmap.CreateScaledBitmap(bitmap, Constants.BigImage.ImageWidth, Constants.BigImage.ImageHeight, false);
                var path = System.IO.Path.GetDirectoryName(uncompressedFile);
                var fn = System.IO.Path.GetFileNameWithoutExtension(uncompressedFile);
                var comprName = System.IO.Path.Combine(path, fn + "_c.JPG");                
                using (var stream = System.IO.File.Create(comprName))
                {
                    compr.Compress(Bitmap.CompressFormat.Png, Constants.BigImage.Quality, stream);
                    stream.Flush();
                }
                bitmap.Recycle();
                return comprName;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Can't compress image: " + ex.Message, ToastLength.Long).Show();
                return uncompressedFile;
            }
        }

        private void AddImage(string path)
        {
            LinearLayout llNewImage = new LinearLayout(this);
            llNewImage.LayoutParameters = new ViewGroup.LayoutParams(
                LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            var p = Constants.PreviewCard.BorderSize;
            llNewImage.SetPadding(p, p, p, p);
            var img = new ImageView(this);
            img.LayoutParameters = new LinearLayout.LayoutParams(
                Constants.PreviewCard.ImageWidth, Constants.PreviewCard.ImageHeight);
            try
            {
                img.SetImageURI(Android.Net.Uri.Parse(path));
            } catch(Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                img.SetImageResource(Resource.Drawable.ErrorImage);
            }
            llNewImage.AddView(img);
            llImages.AddView(llNewImage);

            ImageData id = new ImageData();
            id.ImageFile = path;
            id.layoutWithImage = llNewImage;
            id.Selected = false;
            images.Add(id);
            CheckShowNoImage();
            SetButtonsEnabled();

            img.Click += (o, s) => img_Click(id);
            /*switch (num)
            {
                case 0:
                    ivFore.SetImageURI(uri);
                    return;
                case 1:
                    ivBack.SetImageURI(uri);
                    return;
                default:
                    break;
            }*/
        }

        void img_Click(ImageData idata)
        {
            idata.Selected = !idata.Selected;
            foreach (ImageData item in images)
            {
                if (item != idata && item.Selected)
                    item.Selected = false;
            }
            SetButtonsEnabled();
        }

        private void CheckShowNoImage()
        {
            if (images.Count == 0)
                llNoImage.Visibility = ViewStates.Visible;
            else
                llNoImage.Visibility = ViewStates.Invisible;
        }


        void btnApply_Click(object sender, EventArgs e)
        {
            var data = new CardData();
            data.Name = etCardName.Text;
            data.ShopName = etShopName.Text;
            data.Images = new List<string>();
            foreach (var image in images)
            {
                var imf = image.ImageFile;
                if (imf.StartsWith(_currentCardDir))
                    imf = imf.Replace(_currentCardDir, storage.GetDataFolderName(data));
                data.Images.Add(imf);
            }
            CardManager.Instance.AddCard(data);
            storage.StoreDataToFolder(data, _currentCardDir);
            this.Finish();
        }
    }

    internal class ImageData
    {
        public LinearLayout layoutWithImage;
        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                layoutWithImage.SetBackgroundColor(
                    _selected 
                    ? Constants.PreviewCard.ActiveColor
                    : Constants.PreviewCard.InactiveColor);
            }
        }
        public string ImageFile;
    }
}