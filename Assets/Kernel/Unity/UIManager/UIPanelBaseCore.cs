using UnityEngine;
using System.Collections;

public class UIPanelBaseCore : MonoBehaviour
{
    protected int depth;
    protected UIManagerCore manager;

    public bool isMainUI;

    public int Id { get; private set; }

    public virtual int Depth
    {
        get => depth;
        set => depth = value;
    }

    public virtual void Open(UIManagerCore mgr, params object[] args)
    {
        manager = mgr;
        OnOpen(args);
    }
    
    public virtual void Close(bool isCloseByMgr)
    {
        if (isCloseByMgr)
        {
            OnClose();
            GameObject.Destroy(gameObject);
        }
        else
        {
            manager.ClosePanel(Id);
        }
    }

    protected virtual void OnOpen(params object[] args)
    {
    }

    protected virtual void OnClose()
    {
    }
}
