using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.util
{
    public class BoosterMenuSlice : MonoBehaviour
    {

        public int Level
        {
            get { return Level; }
            set
            {
                Level = value;
                Text.GetComponent<Text>().text = "Level " + value;
            }
        }

        public GameObject Sprite;
        public GameObject Text;
        public GameObject Button;

    }
}