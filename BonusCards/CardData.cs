using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BonusCards
{
    [Serializable]
    public class CardData
    {        
        public string Name { get; set; }
        public string ShopName { get; set; }

        public List<string> Images { get; set; }

        [XmlIgnore]
        public string FSName
        {
            get { return Name.Replace(" ", "_").Replace(",", "_").Replace(".", ""); }
        }

        [XmlIgnore]
        public int? DistanceInMeters { get; set; }
    }

    public class CardDistanceComparer : IComparer<CardData>
    {
        public int Compare(CardData x, CardData y)
        {
            if (x.DistanceInMeters == null && y.DistanceInMeters == null)
                return x.Name.CompareTo(y.Name);
            if (x.DistanceInMeters == null)
                return -1;
            if (y.DistanceInMeters == null)
                return +1;

            return (x.DistanceInMeters.Value.CompareTo(y.DistanceInMeters.Value));
        }
    }
}