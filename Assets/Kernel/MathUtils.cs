using UnityEngine;

namespace GameKit.Kernel
{
    public class MathUtils
    {
        public static Vector2 Bezier(Vector2 p00, Vector2 p10, Vector2 p20, float t)
        {
            var p11 = p00 * (1 - t) + p10 * t;
            var p21 = p10 * (1 - t) + p20 * t;
            var p22 = p11 * (1 - t) + p21 * t;

            return p22;
        }

        public static Vector3 Bezier(Vector3 p00, Vector3 p10, Vector3 p20, float t)
        {
            var p11 = p00 * (1 - t) + p10 * t;
            var p21 = p10 * (1 - t) + p20 * t;
            var p22 = p11 * (1 - t) + p21 * t;

            return p22;
        }
    }
}