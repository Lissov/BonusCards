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
    public class CardManager
    {
        StorageManager storage = new StorageManager();

        private CardManager() 
        {
            _cards = storage.LoadFromFileSystem();
        }

        private static CardManager _instance = new CardManager();

        public static CardManager Instance
        {
            get { return _instance; }
        }


        private List<CardData> _cards = new List<CardData>();

        public void AddCard(string cardName)
        {
            _cards.Add(new CardData() { Name = cardName });
        }

        public void AddCard(CardData card)
        {
            _cards.Add(card);
        }

        public List<CardData> GetDistanceSorted()
        {
            var l = new List<CardData>();
            l.AddRange(_cards);
            l.Sort(new CardDistanceComparer());
            return l;
        }

        public CardData GetCard(string name)
        {
            return _cards.FirstOrDefault(card => card.Name == name);
        }
    }
}