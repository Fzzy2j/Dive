using System;
using UnityEngine;

namespace Assets.scripts.stage
{
    public class RocketBooster : Stage {
        private int _startTime;

        public override GameObject Instantiate() {
            var booster = Instantiate(Manager.I.getStagePrefab(StageName));
            var stage = booster.GetComponent<RocketBooster>();
            stage.Duration = Duration;
            return booster;
        }

        private void Start()
        {
            _startTime = Environment.TickCount;
        }

        private void FixedUpdate ()
        {
            Manager.Player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -40));
            if (Environment.TickCount - _startTime <= Duration) return;
            base.StageFinished();
            Destroy(this.gameObject);
        }

        private void Update()
        {
            this.transform.position = Manager.Player.transform.position;
        }

    }
}
