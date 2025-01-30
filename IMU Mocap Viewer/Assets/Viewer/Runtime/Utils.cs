using UnityEngine;

namespace Viewer.Runtime
{
    static class Utils
    {
        public static float ClampAngle(float angle)
        {
            switch (angle)
            {
                case < 0f:
                    angle = 360f - -angle % 360f;
                    break;
                case > 360f:
                    angle %= 360f;
                    break;
            }

            return angle;
        }

        public static bool ConsumeFlag(ref bool flag)
        {
            if (flag == false) return false;

            flag = false;

            return true;
        }
        
        public static Bounds SmoothDamp(this Bounds current, Bounds target, ref Vector3 velocityMin, ref Vector3 velocityMax, float smoothTime)
        {
            Vector3 newMin = Vector3.SmoothDamp(current.min, target.min, ref velocityMin, smoothTime);
            Vector3 newMax = Vector3.SmoothDamp(current.max, target.max, ref velocityMax, smoothTime);

            return new Bounds
            {
                min = newMin,
                max = newMax
            };
        }
    }
}