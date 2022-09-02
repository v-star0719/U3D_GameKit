using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameKit
{
    [CustomEditor(typeof(BezierEdit))]
    public class BezierEditInspector : Editor
    {
        private int delIndex;
        private int switchIndex1;
        private int switchIndex2;
        private int addIndex;
        private BezierEdit bezierEdit;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bezierEdit = target as BezierEdit;
            var t = typeof(BezierData);
            bezierEdit.bezierData =
                EditorGUILayout.ObjectField(t.Name, bezierEdit.bezierData, typeof(BezierData), true) as BezierData;

            if (!bezierEdit.isLoaded)
            {
                if (bezierEdit.bezierData != null)
                {
                    if (GUILayout.Button("Load", GUILayout.ExpandWidth(false)))
                    {
                        bezierEdit.Load();
                    }
                }

                return;
            }

            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
            {
                bezierEdit.Clear();
            }

            if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
            {
                bezierEdit.Save();
            }

            //serializedObject.FindProperty("moveObj").objectReferenceValue = 
            //EditorGUILayout.ObjectField(t.Name, bezierEdit.moveObj, typeof(Transform), true) as Transform;

            bezierEdit.moveObj =
                EditorGUILayout.ObjectField(t.Name, bezierEdit.moveObj, typeof(Transform), true) as Transform;

            serializedObject.FindProperty("autoMoveSpeed").floatValue =
                EditorGUILayout.FloatField("moveSpeed", bezierEdit.autoMoveSpeed);
            if (GUILayout.Button(bezierEdit.isTesting ? "StopTest" : "StarTest", GUILayout.ExpandWidth(false)))
            {
                if (!bezierEdit.isTesting)
                {
                    bezierEdit.StartTest();
                }
                else
                {
                    bezierEdit.StopTest();
                }
            }

            GUILayout.BeginVertical(GUI.skin.box);
            var points = bezierEdit.GetPoints();
            for (int i = 0; i < points.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(i.ToString(), GUILayout.ExpandWidth(false));
                GUILayout.Label(points[i].position.ToString(), GUILayout.Width(100));
                GUILayout.Label(points[i].quaternion.ToString(), GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }

            if (points.Count == 0)
            {
                GUILayout.Label("not point, please add");
            }

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            delIndex = EditorGUILayout.IntField(delIndex, GUILayout.Width(80));
            if (GUILayout.Button("del", GUILayout.Width(30)))
            {
                Del();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            switchIndex1 = EditorGUILayout.IntField(switchIndex1, GUILayout.Width(80));
            GUILayout.Label("==>", GUILayout.ExpandWidth(false));
            switchIndex2 = EditorGUILayout.IntField(switchIndex2, GUILayout.Width(80));
            if (GUILayout.Button("switch", GUILayout.ExpandWidth(false)))
            {
                Switch();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            addIndex = EditorGUILayout.IntField(addIndex, GUILayout.Width(80));
            if (GUILayout.Button("add", GUILayout.ExpandWidth(false)))
            {
                Add();
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void Del()
        {
            if (!bezierEdit.isLoaded)
            {
                return;
            }

            var points = bezierEdit.GetPoints();
            if (delIndex < points.Count)
            {
                points.RemoveAt(delIndex);
            }

            bezierEdit.RefreshPointGameObjects();
        }

        private void Add()
        {
            if (!bezierEdit.isLoaded)
            {
                return;
            }

            var points = bezierEdit.GetPoints();
            BezierPointData p = new BezierPointData();
            p.quaternion = Quaternion.identity;
            points.Insert(addIndex, p);
            bezierEdit.RefreshPointGameObjects();
        }

        private void Switch()
        {
            if (!bezierEdit.isLoaded)
            {
                return;
            }

            var points = bezierEdit.GetPoints();
            if (switchIndex1 < points.Count && switchIndex2 < points.Count)
            {
                var t = points[switchIndex2];
                points[switchIndex2] = points[switchIndex1];
                points[switchIndex1] = t;
            }

            bezierEdit.RefreshPointGameObjects();
        }
    }
}