using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BonusCards
{
    public static class Constants
    {
        public const string APP_DIR = ".bonuscards";
        public const string CARD_DATA_FILE = "carddata.xml";

        public const string EXTRA_CARD_NAME = "cardname";
        public const string EXTRA_IMAGE_NUMBER = "imagenum";

        public const int TAKE_PICTURE = 125;

        public static class PreviewCard
        {
            public const int ImageWidth = 255;
            public const int ImageHeight = 165;
            public const int BorderSize = 5;

            public static Color NoImageColor = Color.LightGray;
            public static Color InactiveColor = Color.Gray;
            public static Color ActiveColor = Color.Green;
        }

        public static class BigImage
        {
            public const int ImageWidth = 800;
            public const int ImageHeight = 480;
            public const int Quality = 50;
        }
    }
}