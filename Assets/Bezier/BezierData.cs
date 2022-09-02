using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit
{
    [CreateAssetMenu(fileName = "BezierData", menuName = "VStar/Bezier/Asset", order = 1)]
    public class BezierData : ScriptableObject
    {   //两个一组，分别是位置和旋转。012,234,456这样是表示一个曲线，中间那个是控制点
        public List<BezierPointData> points = new List<BezierPointData>();
        public float totalDistance; //n个顶点，n-1段。第i段是i和i+1顶点
        public float[] distances;

        public void Set(List<BezierPointData> list)
        {
            points.Clear();
            points.AddRange(list);
            CalculateDisance();
        }

        public List<BezierPointData> GetClonedPoints()
        {
            List<BezierPointData> list = new List<BezierPointData>();
            for(int i = 0; i < points.Count; i++)
            {
                list.Add(points[i].Clone());
            }

            return list;
        }

        public void CalculateDisance()
        {
            distances = BezierUtils.InitDistances(points, out totalDistance);
        }
    }

    //为了计算快，只用一个控制点。第i个点上的控制点是[i,i+1]段曲线上的控制点
    //因此最后一个点的控制点没有用
    [Serializable]
    public class BezierPointData
    {
        public Vector3 position;
        public Quaternion quaternion;
        public Vector3 controller;

        public BezierPointData()
        {
            quaternion = Quaternion.identity;
        }

        public BezierPointData Clone()
        {
            var data = new BezierPointData();
            data.position = position;
            data.quaternion = quaternion;
            data.controller = controller;
            return data;
        }
    }
}