using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Edit;
using Kernel.Editor;
using UnityEditor;
using UnityEngine;

public class ListEditor<T>
{
    protected readonly Func<T, bool> drawItemCallback;
    protected readonly Func<bool> drawHeadCallback;
    protected readonly Func<bool> drawFootCallback;

    protected readonly Func<T> createCallback;
    protected readonly Action<T> deleteCallback;

    protected readonly string name;
    protected int deleteIndex = -1;
    protected string foldKey;
    protected int idWidth;
    protected bool isFolded;

    //很少用一些功能，直接赋值开关吧
    public bool showIndex = true;
    public bool indexStartFromOne;
    public bool firstRemovable = true;

    public ListEditor(string name, string foldKey,
        Func<T, bool> drawItemCallback, 
        Func<bool> drawHeadCallback,
        Func<bool> drawFootCallback,
        Func<T> createCallback,
        Action<T> deleteCallback)
    {
        this.name = name ?? typeof(T).Name;
        this.foldKey = foldKey;
        this.drawItemCallback = drawItemCallback;
        this.drawHeadCallback = drawHeadCallback;
        this.drawFootCallback = drawFootCallback;
        this.createCallback = createCallback;
        this.deleteCallback = deleteCallback;
        isFolded = string.IsNullOrEmpty(foldKey) || EditorPrefs.GetBool(foldKey, true);
    }

    public virtual void OnGUI(List<T> list, ref bool changed)
    {
        using (GUIUtil.LayoutVertical(EditorGUIUtil.StyleBox, GUILayout.ExpandWidth(true)))
        {
            var fold = DrawHead(list.Count, ref changed);
            if (!fold)
            {
                DrawBody(list, ref changed);
                DrawFoot(list, ref changed);
            }
        }

        if (deleteIndex >= 0)
        {
            list.RemoveAt(deleteIndex);
            deleteIndex = -1;
        }
    }

    protected virtual bool DrawHead(int count, ref bool changed)
    {
        var rect = EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label(isFolded ? "▲" : "▼", GUILayout.ExpandWidth(false));
            GUILayout.Label($"{name} ({count})", GUILayout.ExpandWidth(true));
            if (drawHeadCallback != null)
            {
                changed = changed || drawHeadCallback.Invoke();
            }
        }
        EditorGUILayout.EndHorizontal();

        if(GUI.Button(rect, GUIContent.none, GUIStyle.none))
        {
            isFolded = !isFolded;
            EditorPrefs.SetBool(foldKey, isFolded);
        }

        return isFolded;
    }

    protected virtual void DrawBody(List<T> list, ref bool changed)
    {
        idWidth = 0;
        var n = list.Count;
        while(n > 0)
        {
            idWidth++;
            n = n / 10;
        }

        for(var i = 0; i < list.Count; i++)
        {
            DrawItem(list[i], i, ref changed);
        }
    }

    protected virtual void DrawItem(T t, int i, ref bool changed)
    {
        using(GUIUtil.LayoutHorizontal())
        {
            //编号
            if(showIndex)
            {
                using(GUIUtil.LayoutVertical(GUILayout.Width(idWidth * 10)))
                {
                    GUILayout.Label(indexStartFromOne ? (i + 1).ToString() : i.ToString(), GUILayout.ExpandWidth(false));
                }
            }

            //内容
            using(GUIUtil.LayoutVertical())
            {
                if(drawItemCallback != null)
                {
                    changed = drawItemCallback(t) || changed;
                }
            }

            //删除
            using(GUIUtil.LayoutVertical(GUILayout.Width(20)))
            {
                if(i != 0 || firstRemovable)
                {
                    if(GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                    {
                        if(deleteIndex < 0)
                        {
                            deleteCallback?.Invoke(t);
                            changed = true;
                            deleteIndex = i;
                        }
                    }
                }
            }
        }
    }

    protected virtual void DrawFoot(List<T> list, ref bool changed)
    {
        using(GUIUtil.LayoutHorizontal())
        {
            GUILayout.FlexibleSpace();
            if (drawFootCallback != null)
            {
                changed |= drawFootCallback.Invoke();
            }

            if(GUILayout.Button("+", GUILayout.Width(20)))
            {
                var rt = createCallback.Invoke();
                if(rt != null)
                {
                    list.Add(rt);
                    changed = true;
                }
            }
        }
    }
}
