using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit
{
    public class BezierMove
    {
        private BezierData data;
        private float[] distances = null;
        private int curCurve;
        private float movedDistance;
        private float totalDistance;

        public void Init(BezierData bd)
        {
            data = bd;
            curCurve = 0;
            movedDistance = 0;
            distances = data.distances;
        }

        public void Clear()
        {

        }

        public void Move(float deltaDist, Transform moveObj)
        {
            movedDistance += deltaDist;

            if (deltaDist > 0)
            {
                //往前
                if (movedDistance > totalDistance)
                {
                    if (curCurve == distances.Length - 1)
                    {
                        curCurve = 0; //运动完成
                        totalDistance = distances[0];
                        movedDistance = 0;
                    }
                    else
                    {
                        curCurve++; //切换到下一条路径上
                        movedDistance = movedDistance - totalDistance;
                        totalDistance = distances[curCurve];
                    }
                }
            }
            else
            {
                //往后
                if (movedDistance <= 0)
                {
                    if (curCurve == 0)
                    {
                        curCurve = distances.Length - 1; //运动完成
                        totalDistance = distances[distances.Length - 1];
                        movedDistance = totalDistance;
                    }
                    else
                    {
                        curCurve--; //切换到下一条路径上
                        totalDistance = distances[curCurve];
                        movedDistance = totalDistance - movedDistance;
                    }
                }
            }

            float t = movedDistance / totalDistance;
            var pos = BezierUtils.Bezier(data.points[curCurve], data.points[curCurve + 1], t);
            moveObj.localPosition = pos;
            moveObj.rotation = BezierUtils.BezierQuaternion(data.points[curCurve], data.points[curCurve + 1], t);
        }
    }
}