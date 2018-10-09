
using System;

namespace Assets.scripts
{
    public class LaunchEventArgs : EventArgs
    {
        public float HorizontalVelocity { get; set; }
        public float VerticleVelocity { get; set; }
        public float Torque { get; set; }
    }
}
