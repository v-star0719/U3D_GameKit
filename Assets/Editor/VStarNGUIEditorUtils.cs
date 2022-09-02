using System.Collections;
using System.Collections.Generic;
using GameKit;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using Selection = UnityEditor.Selection;

public class VStarNGUIEditorUtils : EditorWindow
{
    private static VStarNGUIEditorUtils wnd;
    private int depthOffset;
    private int depth;
    private int width;
    private int height;
    private float localX;
    private float localY;
    private float localZ;
    private float localXScale;
    private float localYScale;
    private float localZScale;

    [MenuItem("VStar/NGUIEditorUtils")]
    public static void GetWindow()
    {
        if (wnd != null)
        {
            wnd.Show();
            return;
        }

        wnd = GetWindow<VStarNGUIEditorUtils>();
    }

    private void OnGUI()
    {
        using (GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("widget 宽高", GUILayout.ExpandWidth(false)))
            {
                ChangeSize(width, height);
            }
            width = EditorGUILayout.IntField(width, GUILayout.Width(100));
            GUILayout.Label("X", GUILayout.ExpandWidth(false));
            height = EditorGUILayout.IntField(height, GUILayout.Width(100));
        }

        using (GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("widget depth", GUILayout.ExpandWidth(false)))
            {
                ChangeDepth(depth);
            }
            depth = EditorGUILayout.IntField(depth, GUILayout.Width(100));
        }

        using (GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("所有子widget的depth增加"))
            {
                ChangeChildDepthByOffset(depthOffset);
            }
            depthOffset = EditorGUILayout.IntField(depthOffset, GUILayout.Width(100));
        }


        using(GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("所有local坐标缩放"))
            {
                ScaleLocalPosition(localXScale, localYScale, localZScale);
            }
            localXScale = EditorGUILayout.FloatField(localXScale, GUILayout.Width(100));
            localYScale = EditorGUILayout.FloatField(localYScale, GUILayout.Width(100));
            localZScale = EditorGUILayout.FloatField(localZScale, GUILayout.Width(100));
        }

        using(GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("空港模型节点修改"))
            {
                ResetCityModelNode();
            }
        }

        using(GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("空港模型节点修改2"))
            {
                ResetCityModel2();
            }
        }

        using(GUIUtil.LayoutHorizontal())
        {
            if(GUILayout.Button("空港模型节点修改3"))
            {
                ResetCityModel3();
            }
        }

        if (Selection.gameObjects.Length > 0)
        {
            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GUILayout.Label("选中：" + Selection.gameObjects[i].name);
            }
        }
        else
        { 
            GUILayout.Label("请先选中一个NGUI对象根节点");
        }
    }

    private void ChangeSize(int width, int height)
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var w = Selection.gameObjects[i].GetComponent<UIWidget>();
            w.width = width;
            w.height = height;
        }
    }

    private void ChangeDepth(int depth)
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var w = Selection.gameObjects[i].GetComponent<UIWidget>();
            w.depth = depth;
        }
    }

    private void ChangeChildDepthByOffset(int offset)
    {
        var list = Selection.activeGameObject.GetComponentsInChildren<UIWidget>(true);
        for(int i = 0; i < list.Length; i++)
        {
            list[i].depth += offset;
        }
    }

    private void ScaleLocalPosition(float x, float y, float z)
    {
        for(int i = 0; i < Selection.transforms.Length; i++)
        {
            var pos = Selection.transforms[i].localPosition;
            pos.x *= x;
            pos.y *= y;
            pos.z *= z;
            Selection.transforms[i].localPosition = pos;
        }
    }

    private void ResetCityModelNode()
    {
        var city = Selection.activeTransform;
        List<Transform> nodes = new List<Transform>();
        for (int i = 0; i < city.childCount; i++)
        {
            var t = city.GetChild(i);
            if (!t.name.Contains("_"))
            {
                nodes.Add(t);
            }
        }

        foreach (var node in nodes)
        {
            var str = node.name + '_';
            for(int j = 0; j < city.childCount; j++)
            {
                var child = city.GetChild(j);
                if(child.name.Contains(str))
                {
                    node.localPosition = child.localPosition;
                    child.transform.parent = node;
                    child.localPosition = Vector3.zero;
                    break;
                }
            }
        }
    }

    //将特效设置为显示，隐藏节点。特效放进节点里。
    private void ResetCityModel2()
    {
        var city = Selection.activeTransform;
        for(int i = 0; i < city.childCount; i++)
        {
            var t = city.GetChild(i);
            if(!t.name.Contains("_"))
            {
                for (int j = 0; j < t.childCount; j++)
                {
                    var child = t.GetChild(j);
                    if (child.name == "normal")
                    {
                        child.gameObject.SetActive(true);
                    }
                    else if(child.name == "blue")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else if(child.name == "yellow")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else if(child.name == "saomiao")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else if(child.name == "levelup")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else if(child.name == "levelupwancheng")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }

        for(int i = 0; i < city.childCount; i++)
        {
            var t = city.GetChild(i);
            if(!t.name.Contains("_"))
            {
                var list = GetChildren(t);
                for(int j = 0; j < list.Count; j++)
                {
                    var child = list[j];
                    if(child.name.EndsWith("_blue"))
                    {
                        AddToRoot(child, t, "blue");
                    }
                    else if (child.name.EndsWith("_yellow"))
                    {
                        AddToRoot(child, t, "yellow");
                    }
                    else if(child.name.EndsWith("_saomiao"))
                    {
                        AddToRoot(child, t, "saomiao");
                    }
                    else if(child.name.EndsWith("_levelupzhong"))
                    {
                        AddToRoot(child, t, "levelup");
                    }
                    else if(child.name.EndsWith("_levelupwancheng"))
                    {
                        AddToRoot(child, t, "levelupwancheng");
                    }
                    else if(child.name.Contains("_"))
                    {
                        AddToRoot(child, t, "normal");
                    }
                }
            }
        }
    }

    private void ResetCityModel3()
    {
        var city = Selection.activeTransform;
        for(int i = 0; i < city.childCount; i++)
        {
            var t = city.GetChild(i);
            if(!t.name.Contains("_"))
            {
                for(int j = 0; j < t.childCount; j++)
                {
                    var child = t.GetChild(j);
                    if(child.name == "levelup")
                    {
                        child.name = "levelupzhong";
                    }
                }
            }
        }
    }

    private void AddToRoot(Transform child, Transform node, string nodeName)
    {
        var t = Find(node, nodeName);
        if (t != null)
        {
            child.parent = t;
        }
    }

    private Transform Find(Transform t, string name)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            var tt = t.GetChild(i);
            if (tt.name == name)
            {
                return tt;
            }
        }
        return null;
    }

    private List<Transform> GetChildren(Transform t)
    {
        var list = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
        {
            list.Add(t.GetChild(i));
        }

        return list;
    }
}
