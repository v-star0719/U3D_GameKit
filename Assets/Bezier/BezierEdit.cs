using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameKit
{

//直接挂到GameObjective上，然后加子节点作为曲线的节点
//两个节点中间的点是控制点。节点列表为：节点，控制点，节点，控制点，节点。。。以此类推
    [ExecuteInEditMode]
    public class BezierEdit : MonoBehaviour
    {
        public BezierData bezierData;
        public bool isLoaded;

        public Transform moveObj;
        public bool isTesting;
        public float autoMoveSpeed = 1;

        private BezierMove bezierMove = new BezierMove();

        private BezierData workingBezierData;

        private List<GameObject> childGameObjects = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            workingBezierData = ScriptableObject.CreateInstance<BezierData>();
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                int index = GameUtilities.GetIntFromString(t.name);
                if (index < workingBezierData.points.Count)
                {
                    var bp = workingBezierData.points[index];
                    if (t.name.EndsWith("_c"))
                    {
                        bp.controller = t.localPosition;
                    }
                    else
                    {
                        bp.position = t.localPosition;
                    }
                }
            }

            if (isTesting)
            {
                bezierMove.Move(Time.deltaTime * autoMoveSpeed, moveObj);
            }

        }

        public void Load()
        {
            isLoaded = true;
            workingBezierData.Set(bezierData.GetClonedPoints());
            RefreshPointGameObjects();
        }

        public void StartTest()
        {
            if (isTesting || moveObj == null)
            {
                if (moveObj == null)
                {
                    EditorUtility.DisplayDialog("错误", "没有设置moveObject", "我知道了");
                }

                return;
            }

            isTesting = true;
            bezierMove.Init(workingBezierData);
        }

        public void StopTest()
        {
            if (!isTesting)
            {
                return;
            }

            isTesting = false;
            bezierMove.Clear();
        }


        public void RefreshPointGameObjects()
        {
            for (int i = 0; i < workingBezierData.points.Count; i++)
            {
                var pt = workingBezierData.points[i];
                GameObject go;
                int m = i * 2;
                if (m < childGameObjects.Count)
                {
                    go = childGameObjects[m];
                }
                else
                {
                    go = new GameObject();
                    go.transform.parent = transform;
                    go.transform.localScale = Vector3.one;
                    childGameObjects.Add(go);
                }

                go.name = i.ToString();
                go.transform.localPosition = pt.position;
                go.transform.localRotation = pt.quaternion;
                go.SetActive(true);

                if (i < workingBezierData.points.Count - 1)
                {
                    m++;
                    if (m < childGameObjects.Count)
                    {
                        go = childGameObjects[m];
                    }
                    else
                    {
                        go = new GameObject(i.ToString());
                        go.transform.parent = transform;
                        go.transform.localScale = Vector3.one;
                        childGameObjects.Add(go);
                    }

                    go.name = i + "_c";
                    go.transform.localPosition = pt.controller;
                    go.SetActive(true);
                }
            }

            if (workingBezierData.points.Count > 0)
            {
                int max = workingBezierData.points.Count * 2 - 1;
                for (int i = max; i < childGameObjects.Count; i++)
                {
                    childGameObjects[i].SetActive(false);
                }
            }
        }

        public void Clear()
        {
            isLoaded = false;
            for (int i = 0; i < childGameObjects.Count; i++)
            {
#if UNITY_EDITOR
                DestroyImmediate(childGameObjects[i]);
#else
                Destroy(childGameObjects[i]);
            #endif

            }

            childGameObjects.Clear();
        }

        public void Save()
        {
            bezierData.Set(workingBezierData.points);
#if UNITY_EDITOR
            EditorUtility.SetDirty(bezierData);
#endif
        }

        public List<BezierPointData> GetPoints()
        {
            return workingBezierData.points;
        }

        void OnDrawGizmos()
        {
            IteratorCurves((p0, p1, p2) =>
            {
                DrawCourve(transform.TransformPoint(p0), transform.TransformPoint(p1),
                    transform.TransformPoint(p2));
            });
        }

        void IteratorCurves(Action<Vector3, Vector3, Vector3> callback)
        {
            for (int i = 0; i < workingBezierData.points.Count - 1; i++)
            {
                callback(workingBezierData.points[i].position, workingBezierData.points[i].controller,
                    workingBezierData.points[i + 1].position);
            }
        }

        void DrawCourve(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 ps = p0;
            for (float i = 0; i <= 1.001f; i += 0.1f)
            {
                var pe = Kernel.MathUtils.Bezier(p0, p1, p2, i);
                Gizmos.DrawLine(ps, pe);
                ps = pe;
            }
        }
    }
}
