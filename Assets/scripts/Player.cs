using System;
using System.Collections.Generic;
using Assets.scripts.stage;
using Assets.scripts.util;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.scripts
{
    public class Player : MonoBehaviour
    {
        public GameObject OceanTile;
        public GameObject Launcher;
        public GameObject FishCounterPrefab;
        public GameObject CollectionParent;
        public Canvas Canvas;
        public SpriteRenderer Splash;

        public GameObject String;

        // How many tiles to render
        public int RenderDistance;
        public static List<GameObject> Deletables;

        private int _maxDepth;
        public Text DepthText;

        public float DefaultZoom;

        public Camera Camera;

        private List<GameObject> _oceanTiles;

        public FishCollection FishCollection { get; set; }
        private List<GameObject> _fishCounters;

        public int OceanSize = 5;

        private float _boyancy = 20;
        public float Boyancy
        {
            get { return _boyancy; }
            set { _boyancy = value; }
        }

        private int _currentStageIndex = -1;

        public float ZoomTo { get; set; }

        private void Start()
        {
            ZoomTo = DefaultZoom;
            _fishCounters = new List<GameObject>();
            FishCollection = new FishCollection();
            Manager.Player = this;
            Manager.I.LaunchEvent += OnLaunch;
            Deletables = new List<GameObject>();
            _currentStageIndex = -1;

            Stage.StageFinishEvent += OnStageFinish;

            _oceanTiles = new List<GameObject>();
        }

        private void OnDestroy()
        {
            Manager.I.LaunchEvent -= OnLaunch;
        }

        public void OnLaunch(LaunchEventArgs e)
        {
            var rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.AddForce(new Vector2(e.HorizontalVelocity, e.VerticleVelocity));
            rigidBody.AddTorque(e.Torque);
        }
        
        private bool _enteredWater = false;
        private void Update()
        {
            var zoom = Camera.orthographicSize;
            if (Mathf.Abs(zoom - ZoomTo) > 0.2)
            {
                if (zoom < ZoomTo)
                {
                    Camera.orthographicSize += 0.2f;
                }
                else if (zoom > ZoomTo)
                {
                    Camera.orthographicSize -= 0.2f;
                }
            }
            UpdateBackground();
            var launch = Launcher.GetComponent<Launching>();
            if (!launch.launched)
            {
                //this.gameObject.transform.position = launch.Tip.transform.position;
            }

            // Handle splash
            if (!_enteredWater && IsInWater())
            {
                _enteredWater = true;
                var splash = Instantiate(Splash);
                splash.transform.position = gameObject.transform.position;
                int t = 0;
                Manager.I.ScheduleTaskToRun(() =>
                {
                    t++;
                    splash.transform.localScale += new Vector3(1f, 1f, 0);
                    if (t > 20)
                    {
                        Destroy(splash.gameObject);
                        return false;
                    }
                    return true;
                }, 5);
            }

            Camera.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Camera.transform.position.z);
        }
        private void FixedUpdate()
        {
            var launch = Launcher.GetComponent<Launching>();
            // Handle Render Distance
            for (var i = Deletables.Count - 1; i >= 0; i--)
            {
                var obj = Deletables[i];
                if (obj == null)
                    Deletables.RemoveAt(i);
                else if (Vector3.Distance(obj.transform.position, gameObject.transform.position) > RenderDistance)
                {
                    Deletables.RemoveAt(i);
                    Destroy(obj);
                }
            }

            // Spawning Obstacles
            if (launch.launched && !IsInWater())
            {
                // Aerial Types
                Vector3 vec = this.transform.position + new Vector3(Manager.Random.Next(60) - 30, Manager.Random.Next(60) - 30, 0);
                if (Vector3.Distance(vec, this.transform.position) > 25 && vec.y > 5)
                {
                    List<GameObject> AerialTypes = Manager.I.AerialTypes;
                    Deletables.Add(Instantiate(AerialTypes[Manager.Random.Next(AerialTypes.Count)], vec, new Quaternion()));
                }
            }
            else if (launch.launched)
            {
                // Fish
                for (int i = 0; i < 4; i++)
                {
                    List<GameObject> FishTypes = Manager.I.FishTypes;
                    List<FishBehavior> available = new List<FishBehavior>();
                    var total = 0;
                    foreach (var fish in FishTypes)
                    {
                        var type = fish.GetComponent<FishBehavior>();
                        if (-gameObject.transform.position.y > type.MinimumDepth && -gameObject.transform.position.y < type.MaximumDepth)
                        {
                            total += type.chance;
                            available.Add(type);
                        }
                    }
                    int r = Manager.Random.Next(total) + 1;
                    int calc = 0;
                    foreach (var fish in available)
                    {
                        calc += fish.chance;
                        if (r <= calc)
                        {
                            Vector3 vec = this.transform.position + new Vector3(Manager.Random.Next(60) - 30, Manager.Random.Next(30) - 30, 0);
                            if (Vector3.Distance(vec, this.transform.position) > 25 && vec.y < 5)
                            {
                                Deletables.Add(Instantiate(fish.gameObject, vec, new Quaternion()));
                            }
                            break;
                        }
                    }
                }
            }

            if (Mathf.Max(0, Mathf.RoundToInt(transform.position.y) * -1) > _maxDepth)
                _maxDepth = Mathf.Max(0, Mathf.RoundToInt(transform.position.y) * -1);
            DepthText.text = "DEPTH: " + (Mathf.Max(0, Mathf.RoundToInt(transform.position.y) * -1) + "m");

            if (IsInWater())
            {
                var rigidBody = GetComponent<Rigidbody2D>();
                rigidBody.AddForce(new Vector2(0, 30f));
                if (_currentStageIndex == -1 && Manager.I.GetStages().Count > 0)
                {
                    //Start new stage
                    _currentStageIndex++;
                    Manager.I.GetStages()[_currentStageIndex].Instantiate();
                }
            }

            // Trigger end of run
            if (_maxDepth - Mathf.Max(0, Mathf.RoundToInt(transform.position.y) * -1) <= 50 &&
                (_maxDepth == 0 || !(transform.position.y > 0))) return;
            Invoke("EndRun", 0.4f);
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        private void OnStageFinish(Stage stage)
        {
            if (_currentStageIndex + 1 == Manager.I.GetStages().Count) return;
            _currentStageIndex++;
            Manager.I.GetStages()[_currentStageIndex].Instantiate();
        }

        public void EndRun()
        {
            Manager.Money += _maxDepth / 4;
            Manager.I.GotoShop();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        public bool IsInWater()
        {
            return this.transform.position.y < 2;
        }

        private int y;
        public void UpdateGUI()
        {
            foreach (var collected in FishCollection.TotalCollected)
            {
                bool exists = false;
                foreach (var ui in _fishCounters)
                {
                    if (ui.GetComponent<FishCounter>().Type == collected.Type)
                    {
                        ui.GetComponent<FishCounter>().Amount = collected.Amount;
                        exists = true;
                    }
                }
                if (exists) continue;

                var addUi = Instantiate(FishCounterPrefab, CollectionParent.transform);
                addUi.transform.position += new Vector3(0, y, 0);
                y -= 100;
                var counter = addUi.GetComponent<FishCounter>();
                counter.Type = collected.Type;
                counter.Image.GetComponent<Image>().sprite = Manager.I.getFishPrefab(collected.Type).GetComponentInChildren<SpriteRenderer>().sprite;
                counter.Image.GetComponent<Image>().SetNativeSize();
                counter.Amount = collected.Amount;
                _fishCounters.Add(addUi);
            }
        }

        private void UpdateBackground()
        {
            var relative = Mathf.RoundToInt(this.transform.position.y);
            var level = relative - (relative % OceanSize);

            for (var i = -RenderDistance; i < RenderDistance; i += OceanSize)
            {
                var placeY = Mathf.RoundToInt(OceanTile.transform.localPosition.y + i + level);
                if (placeY >= 0) continue;
                var exists = false;

                for (int c = _oceanTiles.Count - 1; c >= 0; c--)
                {
                    var tile = _oceanTiles[c];
                    if (tile == null)
                    {
                        _oceanTiles.RemoveAt(c);
                        continue;
                    }

                    if (Mathf.RoundToInt(tile.transform.position.y) != placeY) continue;
                    exists = true;
                    break;
                }

                if (exists) continue;

                var sprite = Instantiate(OceanTile,
                    new Vector3(this.transform.position.x, placeY, this.transform.position.z),
                    new Quaternion(0, 0, 0, 1));
                var factor = Mathf.Max(0, Mathf.Min(1, (this.transform.position.y / 10994) + 1));
                sprite.GetComponent<SpriteRenderer>().color = new Color(factor, factor, factor);
                _oceanTiles.Add(sprite);
                Deletables.Add(sprite);
            }
        }

        private static float DistanceSquared(Vector3 a, Vector3 b)
        {
            return Mathf.Pow(Mathf.Abs(a.x - b.x), 2) + Mathf.Pow(Mathf.Abs(a.y - b.y), 2) +
                   Mathf.Pow(Mathf.Abs(a.z - b.z), 2);
        }
    }
}