using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameKit
{
    [CustomEditor(typeof(BezierData))]
    public class BezierDataInspector : Editor
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            BezierData data = target as BezierData;
            if(GUILayout.Button("copy to clipboard"))
            {

            }
        }

        private void CopyToClipboard()
        {
            //StringBuilder pts = new StringBuilder("{\n");
            //StringBuilder rts = new StringBuilder("{\n");
            //StringBuilder dists = new StringBuilder("{\n");
            //for(var i = 0; i < points.Count; i++)
            //{
            //    var point = points[i];
            //    var rotation = rotations[i];
            //    pts.AppendFormat("    Vector3({0:f4}, {1:f4}, {2:f4}),\n", point.x, point.y, point.z);
            //    rts.AppendFormat("    Quaternion({0:f4}, {1:f4}, {2:f4}, {3:f4}),\n", rotation.x, rotation.y, rotation.z, rotation.w);
            //}

            float total = 0;
            //IteratorCurves((p0, p1, p2) =>
            //{
            //float d = Distance(p0, p1, p2);
            //total += d;
            //dists.AppendFormat("    {0:f4}, \n", d);
            //});

            //pts.Append("}\n\n");
            //rts.Append("}\n\n");
            //dists.Append("}\n\n");
            //string totalStr = string.Format("{{\n    {0:f4}\n}}\n\n", total);
            //GUIUtility.systemCopyBuffer = pts.ToString() + rts.ToString() + dists.ToString() + totalStr;
            //Debug.Log("copy to clipboard");
        }
    }
}

