using System;
using UnityEngine;

namespace Assets.scripts.util
{
    public class CrowBehavior : MonoBehaviour
    {

        public Sprite HurtSprite;

        private bool hurt = false;

        private void Update()
        {
            if (!hurt)
                this.transform.position -= new Vector3(5f, 0, 0) * Time.deltaTime;
            else
            {
                this.transform.position -= new Vector3(1f, 5f, 0) * Time.deltaTime;
                this.transform.eulerAngles += new Vector3(0, 0, 50f) * Time.deltaTime;
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            
            if (!hurt)
            {
                this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = HurtSprite;
                hurt = true;
                Time.timeScale = 0.02f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                Manager.I.ScheduleTaskToRun(() =>
                    {
                        Time.timeScale = 1f;
                        Time.fixedDeltaTime = Time.timeScale * 0.02f;
                        Manager.Player.GetComponentInChildren<Rigidbody2D>().velocity *= 1.4f;
                        return false;
                    }, 300);
            }
        }
        
    }
}