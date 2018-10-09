using UnityEngine;

namespace Assets.scripts.util
{
    public class Distance
    {
        public static Vector3 LookAt(Vector3 loc, Vector3 lookat)
        {
            float yaw = 0;

            var dx = lookat.x - loc.x;
            var dy = lookat.y - loc.y;
            var dz = lookat.z - loc.z;

            if (dx != 0)
            {
                if (dx < 0)
                {
                    yaw = 1.5f * Mathf.PI;
                }
                else
                {
                    yaw = 0.5f * Mathf.PI;
                }
                yaw -= Mathf.Atan(dz / dx);
            }
            else if (dz < 0)
            {
                yaw = Mathf.PI;
            }

            float dxz = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dz, 2));

            var pitch = -Mathf.Atan(dy / dxz);

            return new Vector3(-yaw * Mathf.Rad2Deg, pitch * Mathf.Rad2Deg, 0);
        }
    }
}