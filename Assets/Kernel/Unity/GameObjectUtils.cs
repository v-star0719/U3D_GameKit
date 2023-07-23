using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameObjectUtils
{
    public static void AddToParent(Transform trans, Transform parent)
    {
        trans.parent = parent;
        trans.localPosition = Vector3.zero;
        trans.rotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static string GetTransformPath(Transform trans)
    {
        string s = trans.name;
        Transform t = trans;
        while (t.parent != null)
        {
            s = t.parent.name + "/" + s;
            t = t.parent;
        }
        return s;
    }

    public static void SetLayer(Transform trans, int layer)
    {
        if (trans.gameObject.layer == layer)
        {
            return;
        }
        trans.gameObject.layer = layer;
        foreach (Transform t in trans)
        {
            SetLayer(t, layer);
        }
    }

    public static void TraverseTransformTree(Transform root, System.Action<Transform> onVisit)
    {
        if (onVisit != null)
        {
            onVisit(root);
        }
        foreach (Transform child in root)
        {
            TraverseTransformTree(child, onVisit);
        }
    }
}
