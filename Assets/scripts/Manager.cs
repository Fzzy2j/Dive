using System;
using System.Collections.Generic;
using Assets.scripts.stage;
using Assets.scripts.util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.scripts
{
    public delegate void LaunchDelegate(LaunchEventArgs e);
    public class Manager : MonoBehaviour
    {

        public static Manager I;
        public int PlayId;
        public int ShopId;

        public static Player Player;

        public CurvedText CurvedTextPrefab;

        public List<GameObject> StageTypes;
        public List<GameObject> AerialTypes;
        public List<GameObject> FishTypes;

        public GameObject getStagePrefab(string name)
        {
            foreach (var pre in StageTypes)
            {
                if (pre.GetComponent<Stage>().StageName == name)
                    return pre;
            }
            return null;
        }

        public GameObject getFishPrefab(string type)
        {
            foreach (var pre in FishTypes)
            {
                if (pre.GetComponent<FishBehavior>().Type == type)
                    return pre;
            }
            return null;
        }

        private List<Stage> _stages;

        private void Awake()
        {
            _stages = new List<Stage>();
            

            var s = StageSerial;
            if (s.Length > 0)
            {
                foreach (var stagesSerial in s.Split(';'))
                {
                    var name = stagesSerial.Split(',')[0];
                    var level = int.Parse(stagesSerial.Split(',')[1]);
                }
            }

            if (I == null)
            {
                DontDestroyOnLoad(gameObject);
                I = this;
            }
            else if (I != this)
            {
                Destroy(gameObject);
            }
        }

        public static System.Random Random = new System.Random();

        //Save game values
        public static int ThrowStrengthLevel
        {
            get
            {
                return PlayerPrefs.GetInt("ThrowStrengthLevel");
            }
        }

        public static int Money
        {
            get
            {
                return PlayerPrefs.GetInt("Money");
            }
            set
            {
                if (value < 0) return;
                PlayerPrefs.SetInt("Money", value);
                PlayerPrefs.Save();
            }
        }

        private static string StageSerial
        {
            get
            {
                return PlayerPrefs.GetString("StageSerial");
            }
            set
            {
                PlayerPrefs.SetString("StageSerial", value);
                PlayerPrefs.Save();
            }
        }

        private List<ScheduledTask> scheduled = new List<ScheduledTask>();

        public class ScheduledTask
        {
            public ScheduledTask(Func<bool> run, int delay, int timeStamp)
            {
                this.run = run;
                this.delay = delay;
                this.timeStamp = timeStamp;
            }
            public Func<bool> run;
            public int delay;
            public int timeStamp;
            public bool repeat = true;
        }

        public ScheduledTask ScheduleTaskToRun(Func<bool> run, int delay)
        {
            var task = new ScheduledTask(run, delay, Environment.TickCount);
            scheduled.Add(task);
            return task;
        }

        private void Update()
        {
            for (int i = scheduled.Count - 1; i >= 0; i--)
            {
                var task = scheduled[i];
                if (Environment.TickCount - task.timeStamp > task.delay)
                {
                    bool repeat = task.run.Invoke();
                    if (repeat && task.repeat)
                        task.timeStamp = Environment.TickCount;
                    else
                        scheduled.RemoveAt(i);
                }
            }
        }

        public static bool IsPlaying()
        {
            return SceneManager.GetActiveScene().name == SceneManager.GetSceneByBuildIndex(I.PlayId).name;
        }

        public void StartNewPlaySession()
        {
            SceneManager.LoadScene(PlayId);
        }

        public void GotoShop()
        {
            SceneManager.LoadScene(ShopId);
        }

        public List<Stage> GetStages()
        {
            return _stages;
        }

        public virtual void OnLaunch(LaunchEventArgs e)
        {
            _stages.Clear();

            foreach (var stageObject in StageTypes)
            {
                var stage = stageObject.GetComponent<Stage>();
                stage.Duration = 5000;
                _stages.Add(stage);
            }

            LaunchEvent(e);
        }

        public event LaunchDelegate LaunchEvent;
    }
}
