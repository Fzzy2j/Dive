using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    public class Shop : MonoBehaviour {

        public Text Money;
        public Text CurrentStrengthLevel;

        private void Update () {
            Money.text = "Money: " + Manager.Money;
            CurrentStrengthLevel.text = "Current Level: " + Manager.ThrowStrengthLevel;
        }

        public void UpgradeThrowStrength()
        {
            var price = Mathf.Max(1, Manager.ThrowStrengthLevel + 1);
            if (Manager.Money < price) return;
            Manager.Money -= price;
            PlayerPrefs.SetInt("ThrowStrengthLevel", Manager.ThrowStrengthLevel + 1);
            PlayerPrefs.Save();
        }

        public void Play()
        {
            Manager.I.StartNewPlaySession();
        }
    }
}
