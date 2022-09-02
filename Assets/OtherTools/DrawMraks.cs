using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMraks : MonoBehaviour
{
    private static DrawMraks instance;
    private List<Mark> marks = new List<Mark>();

    public static DrawMraks Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject();
                instance = go.AddComponent<DrawMraks>();
                GameObject.DontDestroyOnLoad(go);
            }
            return instance;
        }
        set => instance=value;
    }

    void Init()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < marks.Count; )
        {
            var m = marks[i];
            m.duration -= Time.deltaTime;
            if (m.duration <= 0)
            {
                var renderer = m.go.GetComponentInChildren<Renderer>();
                Destroy(renderer.material);
                renderer.material = null;
                Destroy(m.go);
                marks.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    public void AddPoint(Vector3 pos, float duration, float scale, Color clr, string name)
    {
        var m = new Mark();
        m.go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m.go.transform.parent = Instance.transform;
        m.go.transform.localScale = new Vector3(scale, scale, scale);
        m.go.transform.position = pos;
        m.duration = duration;
        if (!string.IsNullOrEmpty(name))
        {
            m.go.name = name;
        }
        Instance.marks.Add(m);
    }

    public void AddLine(Vector3 starPos, Vector3 endPos, float duration, float width, Color clr)
    {
        var m = new Mark();
        m.go = new GameObject();
        m.go.transform.parent = Instance.transform;
        m.duration = duration;
        m.go.transform.localScale = Vector3.one;
        var line = m.go.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.SetPosition(0, starPos);
        line.SetPosition(1, endPos);
        line.startWidth = width;
        line.endWidth = width * 0.5f;
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.SetColor("_Color", clr);

        Instance.marks.Add(m);
    }

    void OnDestroy()
    {
        Instance = null;
    }

    private class Mark
    {
        public GameObject go;
        public float duration;
    }

    public static void AddPoint_S(Vector3 pos, float duration, float scale, Color clr, string name)
    {
        GetInstance().AddPoint(pos, duration, scale, clr, name);
    }

    public static void AddLine_S(Vector3 starPos, Vector3 endPos, float duration, float width, Color clr)
    {
        GetInstance().AddLine(starPos, endPos, duration, width, clr);
    }

    private static DrawMraks GetInstance()
    {
        if(Instance == null)
        {
            var go = new GameObject("DebugUtils");
            Instance = go.AddComponent<DrawMraks>();
            GameObject.DontDestroyOnLoad(go);
        }

        return Instance;
    }
}
