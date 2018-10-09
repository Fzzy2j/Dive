using UnityEngine;

namespace Assets.scripts
{
    public class Launching : MonoBehaviour
    {

        public GameObject Tip;
        public GameObject Arms;

        private int _power;
        public bool launched { get; set; }
        public bool launching { get; set; }
        private float _armAngle;

        public DistanceJoint2D joint;

        public GameObject PowerBarObj;

        public GameObject Player;

        private void Start()
        {
            PowerBarObj = Instantiate(PowerBarObj, Player.transform);
            var powerBar = PowerBarObj.GetComponent<PowerBar>();
            powerBar.SetProgress(0);
            powerBar.gameObject.transform.localScale = new Vector3(2, 2, 2);
            powerBar.gameObject.transform.localPosition += new Vector3(0, 2, 0);
        }

        private void FixedUpdate()
        {
            if (launching)
            {
                if (_power < 200)
                {
                    _power += 2;
                }
                else
                {
                    _power = 0;
                }
            }

            if (_armAngle > -90 && launched)
            {
                _armAngle -= 20;
                Arms.transform.eulerAngles = new Vector3(0, 0, _armAngle);
            }
        }

        private void Update()
        {
            if (Manager.IsPlaying())
            {
                if (!launched)
                {
                    var powerBar = PowerBarObj.GetComponent<PowerBar>();
                    if (Input.GetMouseButton(0))
                    {
                        //Power Bar fill/deplete
                        launching = true;
                        var progress = _power < 100 ? _power : 200 - _power;
                        powerBar.SetProgress(progress);
                        _armAngle = (Mathf.Sqrt(progress / 20f + 0.2f) - Mathf.Sqrt(0.2f)) * 25;
                        Arms.transform.eulerAngles = new Vector3(0, 0, _armAngle);
                    }
                    else
                    {
                        if (launching)
                        {
                            // Launch Event
                            var args = new LaunchEventArgs();
                            var throwLevel = Manager.ThrowStrengthLevel + 1;
                            var hvelocity = Mathf.Max(Mathf.Sqrt(throwLevel * 2) / 2f, throwLevel / 10f) * 100;
                            var vvelocity = Mathf.Max(Mathf.Sqrt(throwLevel * 2) / 2f, throwLevel / 10f) * 400;

                            var power = _power < 100 ? _power : 200 - _power;

                            if (power > 98)
                                power = 150;
                            if (power < 30)
                                power = 30;

                            args.HorizontalVelocity = hvelocity * (power / 100f);
                            args.VerticleVelocity = vvelocity * (power / 100f);
                            args.Torque = throwLevel * 5 * (new System.Random().Next(2) == 0 ? -1 : 1);

                            Manager.I.OnLaunch(args);

                            Destroy(joint);
                            launching = false;
                            launched = true;
                            Destroy(PowerBarObj);
                        }
                    }
                }
            }
        }

    }
}
