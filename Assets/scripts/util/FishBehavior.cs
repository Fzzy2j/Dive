using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.util
{
    public class FishBehavior : MonoBehaviour
    {

        public string Type;

        public bool moveRight = false;

        public int MinimumDepth;
        public int MaximumDepth;
        public int chance;

        public int freezeTime;
        public bool zoom;
        public float zoomAmount;

        public List<string> lines;

        private bool _caught = false;
        private CurvedText curve = null;

        private void Update()
        {
            if (!_caught)
            {
                if (moveRight)
                    this.transform.position -= new Vector3(-3f, 0, 0) * Time.deltaTime;
                else
                    this.transform.position -= new Vector3(3f, 0, 0) * Time.deltaTime;
            }
            else
            {
                this.transform.position = Manager.Player.transform.position;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_caught) return;
            GetComponentInChildren<Animator>().Play("caught");

            var collection = Manager.Player.GetComponent<Player>().FishCollection;

            collection.FishCollected(Type, gameObject);

            var angleChange = Manager.Random.Next(20) - 10;
            if (moveRight)
                gameObject.transform.eulerAngles = new Vector3(0, 0, 90 + angleChange);
            else
                gameObject.transform.eulerAngles = new Vector3(0, 0, -90 + angleChange);

            Manager.Player.GetComponent<Player>().UpdateGUI();

            _caught = true;
            var player = Manager.Player.GetComponent<Player>();
            float size = player.Camera.GetComponent<Camera>().orthographicSize;

            if (zoom)
                Manager.Player.GetComponent<Player>().ZoomTo = zoomAmount;

            if (freezeTime > 0)
            {
                Manager.ScheduledTask task = null;
                if (lines.Count > 0)
                {
                    var line = lines[Manager.Random.Next(lines.Count)];
                    curve = Instantiate(Manager.I.CurvedTextPrefab, Manager.Player.Canvas.transform);
                    curve.text = line;
                    curve.fontSize = 80;
                    task = Manager.I.ScheduleTaskToRun(() =>
                    {
                        if (curve != null)
                        {
                            curve.fontSize += 1;
                            curve.transform.position += Vector3.up * 5;
                        }
                        return true;
                    }, 5);
                }
                Time.timeScale = 0f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                Manager.I.ScheduleTaskToRun(() =>
                {
                    if (task != null)
                        task.repeat = false;
                    if (curve != null)
                        Destroy(curve);
                    if (zoom)
                        Manager.Player.GetComponent<Player>().ZoomTo = Manager.Player.GetComponent<Player>().DefaultZoom;
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    return false;
                }, freezeTime);
            }
        }
    }
}
