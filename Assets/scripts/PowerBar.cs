using UnityEngine;

namespace Assets.scripts
{
    public class PowerBar : MonoBehaviour
    {
        private int _progress;

        public GameObject Foreground;
        public GameObject Background;

        public int GetProgress()
        {
            return _progress;
        }

        public void SetProgress(int progress)
        {
            _progress = progress;
            var scale = Background.transform.localScale;
            scale.x = _progress / 100f;
            var moveX = (Foreground.GetComponent<SpriteRenderer>().size.x / 2) * (GetProgress() / 100f) - Foreground.GetComponent<SpriteRenderer>().size.x / 2;
            Foreground.transform.localPosition = new Vector3(moveX, Foreground.transform.localPosition.y, Foreground.transform.localPosition.z);
            Foreground.transform.localScale = scale;
        }
    }
}