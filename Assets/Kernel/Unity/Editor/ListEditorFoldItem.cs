using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.Edit;
using Kernel.Editor;
using UnityEditor;
using UnityEngine;

public class ListEditorFoldItem<T>:ListEditor<T>
{
    private static GUIStyle itemTitleStyle;

    public static GUIStyle ItemTitleStyle
    {
        get
        {
            if (itemTitleStyle == null)
            {
                itemTitleStyle = new GUIStyle(GUI.skin.box);
                itemTitleStyle.normal.textColor = Color.green;
            }

            return itemTitleStyle;
        }
    }

    private Action<T> drawItemHeadCallback;
    protected List<bool> itemFoldStatusList = new List<bool>();

    public ListEditorFoldItem(string name, string foldKey,
        Func<T, bool> drawItemCallback,
        Func<bool> drawHeadCallback,
        Func<bool> drawFootCallback,
        Action<T> drawItemHeadCallback,
        Func<T> createCallback,
        Action<T> deleteCallback):base(name, foldKey, drawItemCallback, drawHeadCallback, drawFootCallback, createCallback, deleteCallback)
    {
        this.drawItemHeadCallback = drawItemHeadCallback;
    }

    protected override void DrawItem(T t, int i, ref bool changed)
    {
        using (EditorGUIUtil.LayoutVertical(EditorGUIUtil.StyleBox))
        {
            var fold = IsItemFold(i);
            var clr = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            //标题
            var rect = EditorGUILayout.BeginHorizontal(ItemTitleStyle);
            {
                //编号
                if (showIndex)
                {
                    GUILayout.Label(fold ? "▲" : "▼", GUILayout.Width(20));
                    GUILayout.Label(indexStartFromOne ? (i + 1).ToString() : i.ToString(), GUILayout.Width(idWidth * 12));
                }
                drawItemHeadCallback?.Invoke(t);
                GUILayout.FlexibleSpace();
                //删除
                using (GUIUtil.LayoutVertical(GUILayout.Width(20)))
                {
                    if (i != 0 || firstRemovable)
                    {
                        if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                        {
                            if (deleteIndex < 0)
                            {
                                deleteCallback?.Invoke(t);
                                changed = true;
                                deleteIndex = i;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = clr;

            //点击标题
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                fold = !fold;
                SetItemFold(i, fold);
            }

            //内容
            if (!fold)
            {
                if (drawItemCallback != null)
                {
                    changed = drawItemCallback(t) || changed;
                }
            }
        }
    }

    private bool IsItemFold(int index)
    {
        if (index >= itemFoldStatusList.Count)
        {
            return true;
        }

        return itemFoldStatusList[index];
    }

    private void SetItemFold(int index, bool b)
    {
        for (int i = itemFoldStatusList.Count; i < index + 1; i++)
        {
            itemFoldStatusList.Add(true);
        }

        itemFoldStatusList[index] = b;
    }
}
