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
using System.IO;
using System.Xml.Serialization;

namespace BonusCards
{
    public class StorageManager
    {
        private string FullAppFolder
        {
            get { return Android.OS.Environment.ExternalStorageDirectory + @"/" + Constants.APP_DIR; }
        }

        public string GetTempImageDir()
        {
            if (!Directory.Exists(FullAppFolder))
                Directory.CreateDirectory(FullAppFolder);
            var tempfoldername = Path.Combine(FullAppFolder, "Temp");
            if (!Directory.Exists(tempfoldername))
                Directory.CreateDirectory(tempfoldername);
            return tempfoldername;
        }
        public string GetTempImageFileName()
        {
            var tfn = Path.Combine(GetTempImageDir(), "IMG_"/* + DateTime.Now.ToString("yyyyMMdd_HHmmss")*/ + ".JPG");
            while (File.Exists(tfn))
                tfn += "1";
            return tfn;
        }
        public void RecreateTempFolder()
        {
            var tempfoldername = Path.Combine(FullAppFolder, "Temp");
            if (Directory.Exists(tempfoldername))
                Directory.Delete(tempfoldername, true);
            Directory.CreateDirectory(tempfoldername);
        }

        public string GetNextFileName(string folder)
        {
            var fn = "img_";
            var i = 1;
            while (File.Exists(Path.Combine(folder, fn + i + ".jpg")))
                i++;
            return Path.Combine(folder, fn + i + ".jpg");
        }
        public string GetDataFolderName(CardData data)
        {
            return Path.Combine(FullAppFolder, data.FSName);
        }
        public void StoreDataToFolder(CardData data, string prevDirName)
        {
            if (string.IsNullOrEmpty(data.FSName))
                throw new Exception("Empty card Name.");

            var dn = GetDataFolderName(data);

            if (prevDirName != dn)
            {
                if (Directory.Exists(dn))
                    throw new Exception("Card name can't be used.");
                Directory.Move(prevDirName, dn);
            }

            var descFile = Path.Combine(dn, Constants.CARD_DATA_FILE);
            if (File.Exists(descFile))
                File.Delete(descFile);

            var cd = data; // new CardDataDt(data);
            XmlSerializer xs = new XmlSerializer(cd.GetType());
            using (FileStream fs = new FileStream(descFile, FileMode.CreateNew)){
                xs.Serialize(fs, cd);
            }
        }

/*        [Serializable]
        public class CardDataDt
        {
            public string Name { get; set; }
            public string ShopName { get; set; }

            public CardDataDt() { }
            public CardDataDt(CardData data)
            {
                Name = data.Name;
                ShopName = data.ShopName;
            }
        }*/

        public List<CardData> LoadFromFileSystem()
        {
            var res = new List<CardData>();
            var folders = Directory.GetDirectories(FullAppFolder);
            XmlSerializer xs = new XmlSerializer(typeof(CardData));
            foreach (var folder in folders)
            {
                var descFile = Path.Combine(folder, Constants.CARD_DATA_FILE);
                if (File.Exists(descFile))
                {
                    using (FileStream fs = new FileStream(descFile, FileMode.Open))
                    {
                        var d = xs.Deserialize(fs) as CardData;
                        if (d != null)
                            res.Add(d);
                    }
                }
            }

            return res;
        }
    }
}