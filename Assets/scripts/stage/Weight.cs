using System;
using UnityEngine;

namespace Assets.scripts.stage
{
    public class Weight : Stage
    {
        private int _startTime;

        public override GameObject Instantiate()
        {
            var weight = Instantiate(Manager.I.getStagePrefab(StageName));
            var stage = weight.GetComponent<Weight>();
            stage.Duration = Duration;
            return weight;
        }

        private void Start()
        {
            _startTime = Environment.TickCount;
        }

        private void FixedUpdate()
        {
            Manager.Player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -20));
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