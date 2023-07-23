using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorWindowBase : EditorWindow
{
    protected float refreshInterval = 0.2f;
    protected float refreshTimer;
    protected virtual void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshInterval)
        {
            refreshTimer = 0;
            Repaint();
        }
    }
}
