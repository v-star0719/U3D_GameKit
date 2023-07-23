using System.Collections;
using System.Collections.Generic;
using Kernel.Core;
using UnityEngine;

public class UIManagerCore
{
    private const int DEPTH_GAP = 20;
    private List<UIPanelBaseCore> panels = new List<UIPanelBaseCore>();

    public Transform UIRoot { get; private set; }

    public UIManagerCore(Transform root)
    {
        UIRoot = root;
    }

    public virtual UIPanelBaseCore LoadPanel(int id)
    {
        return null;
    }

    public virtual UIPanelBaseCore OpenPanel(int id, params object[] args)
    {
        var panel = LoadPanel(id);
        panel.Open(this, args);
        panel.Depth = panels.Count * DEPTH_GAP;
        panels.Add(panel);
        return panel;
    }

    public virtual void ClosePanel(int id)
    {
        for (var i = 0; i < panels.Count; i++)
        {
            if (panels[i].Id == id)
            {
                panels.RemoveAt(i);
                panels[i].Close(true);
                break;
            }
        }
    }

    public virtual void CloseTopPanel()
    {
        if (panels.Count > 0)
        {
            var p = panels[panels.Count - 1];
            panels.RemoveAt(panels.Count - 1);
            p.Close(true);
        }
    }

    public virtual void CloseAllPanel(bool includeMain = false)
    {
        for(int i = panels.Count - 1; i >= 0; i--)
        {
            var p = panels[i];
            if(!p.isMainUI || includeMain)
            {
                p.Close(true);
                panels.RemoveAt(i);
            }
        }
    }

    public virtual UIPanelBaseCore GetPanel(int id)
    {
        for(var i = 0; i < panels.Count; i++)
        {
            if(panels[i].Id == id)
            {
                return panels[i];
            }
        }

        return null;
    }
}
