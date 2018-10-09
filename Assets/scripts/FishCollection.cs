using Assets.scripts.util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts
{
    public class FishCollection
    {
        public List<FishContainer> TotalCollected { get; private set; }

        public GameObject LastCollected;

        public FishCollection()
        {
            TotalCollected = new List<FishContainer>();
        }

        public void FishCollected(string type, GameObject obj)
        {
            if (LastCollected != null)
                Manager.Destroy(LastCollected);
            LastCollected = obj;
            foreach (var cont in TotalCollected)
            {
                if (cont.Type == type)
                {
                    cont.Amount++;
                    return;
                }
            }
            TotalCollected.Add(new FishContainer(type, 1));
        }
    }

    public class FishContainer
    {
        public FishContainer(string Type, int Amount)
        {
            this.Type = Type;
            this.Amount = Amount;
        }

        public string Type;
        public int Amount;
    }
}