using UnityEngine;
using System.Collections;

namespace GameKit.Kernel
{
    public class FastMath
    {
        public const float CIRCLE = 360;
        public const float HALF_CIRCLE = 180;

        private const int ASIN_PRECISION = 360; //即：分成的段数
        private const int ACOS_PRECISION = 360; //即：分成的段数

        public static float[] sinValues = new float[361];
        public static float[] cosValues = new float[361];
        public static float[] asinValues = new float[ASIN_PRECISION + 2];
        public static float[] acosValues = new float[ASIN_PRECISION + 2]; //最后多存一个cos(1)，使250插值不溢出
        public static bool isInited = false;

        public static void Init()
        {
            if (isInited) return;

            float arg = 0f;
            for (int i = 0; i < 361; i++)
            {
                arg = Mathf.Deg2Rad * i;
                sinValues[i] = Mathf.Sin(arg);
                cosValues[i] = Mathf.Cos(arg);
            }

            for (int i = 0; i < ASIN_PRECISION + 1; i++)
            {
                arg = (float) i / ASIN_PRECISION;
                asinValues[i] = Mathf.Asin(arg) * Mathf.Rad2Deg;
                acosValues[i] = Mathf.Acos(arg) * Mathf.Rad2Deg;
            }

            asinValues[ASIN_PRECISION + 1] = asinValues[ASIN_PRECISION];
            acosValues[ASIN_PRECISION + 1] = acosValues[ASIN_PRECISION];

            isInited = true;
        }

        public static float fsin(float angle)
        {
            float f = angle;
            if (angle < 0) angle = -angle;

            int n = (int) angle;
            float d = angle - n;
            if (n >= 360) n = n % 360;
            float a = sinValues[n];
            float b = sinValues[n + 1];

            if (f >= 0)
                return a + (b - a) * d;
            else
                return -(a + (b - a) * d);
        }

        public static float fcos(float angle)
        {
            if (angle < 0) angle = -angle;

            int n = (int) angle;
            float d = angle - n;
            if (n >= 360) n = n % 360;
            float a = cosValues[n];
            float b = cosValues[n + 1];
            return a + (b - a) * d;
        }

        public static float fasin(float value)
        {
            if (value < 0)
            {
                value *= -ASIN_PRECISION;
                int n = (int) value;
                float d = value - n;
                float a = asinValues[n];
                float b = asinValues[n + 1];
                return -(a + (b - a) * d);
            }
            else
            {
                value *= ASIN_PRECISION;
                int n = (int) value;
                float d = value - n;
                float a = asinValues[n];
                float b = asinValues[n + 1];
                return (a + (b - a) * d);
            }
        }

        public static float facos(float value)
        {
            //return Mathf.PI*0.5f - (value + value*value*value*0.1666667f + value*value*value*value*value*0.075f);
            //*
            if (value < 0)
            {
                value *= -ASIN_PRECISION;
                int n = (int) value;
                float d = value - n;
                float a = acosValues[n];
                float b = acosValues[n + 1];
                return HALF_CIRCLE - (a + (b - a) * d);
            }
            else
            {
                value *= ASIN_PRECISION;
                int n = (int) value;
                float d = value - n;
                float a = acosValues[n];
                float b = acosValues[n + 1];
                return (a + (b - a) * d);
            }

            //*/
        }
    }
}