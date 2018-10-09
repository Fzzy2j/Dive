using UnityEngine;

namespace Assets.scripts.stage
{
    public abstract class Stage : MonoBehaviour {
        
        public int Duration { get; set; }
        public string StageName;

        public abstract GameObject Instantiate();

        public delegate void StageFinish(Stage stage);
        public static event StageFinish StageFinishEvent;

        protected void StageFinished()
        {
            if (StageFinishEvent != null) StageFinishEvent(this);
        }

    }
}
