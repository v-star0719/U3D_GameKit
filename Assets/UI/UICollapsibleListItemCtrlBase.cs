using UnityEngine;
using System.Collections;

public class UICollapsibleListItemCtrlBase : MonoBehaviour
{
	public float titleBarHeight;
	public float contentHeight;
	public float titleContentGap;//如果不想要间隔，可以填负数
	public Transform titleBar;
	public Transform content;

    // callback function 
    public GameObject handleObject;
    public string clickFunctionName;

	[HideInInspector]public UICollapsibleListCtrl listCtrl;
	[HideInInspector]public float curContentHeight;
	[HideInInspector]public bool isCollapsed = false;
	[HideInInspector]public int myIndex = 0;

	public float _itemHeight
	{
		get{return titleBarHeight + contentHeight;}
	}

	public float realContentHeight
	{
		get{
			if(isCollapsed) return curContentHeight;
			else return curContentHeight + titleContentGap;
		}
	}

	private Transform _myTrans;
	public Transform trans
	{
		get{
			if(_myTrans == null) _myTrans = transform;
			return _myTrans;
		}
	}

	void Start()
	{
		UIButtonCustomMessage btn = titleBar.GetComponent<UIButtonCustomMessage>();
		if(btn == null)
			btn = titleBar.gameObject.AddComponent<UIButtonCustomMessage>();
		btn.trigger = UIButtonCustomMessage.Trigger.OnClick;
		btn.functionName = "OnTitleBarClicked";
		btn.target = gameObject;
	}

	//itemIndex是这个列表项的索引，和myIndex的值是一样一样的
	//userData是列表数据，怎么用就是user的事了
	public virtual void SetData(int itemIndex, System.Object userData)
	{
		//如果是变动的contentHeight，则需要在这里的计算出contentHeight
	}

	public void OnTitleBarClicked()
	{
        if (listCtrl.isCollapsing || listCtrl.isExpanding)
            return;
		listCtrl.Collapsible(myIndex, !isCollapsed, null);
		isCollapsed = !isCollapsed;
	}

    public void ClickTitleBar()
    {
        if (listCtrl.isCollapsing || listCtrl.isExpanding)
            return;
        listCtrl.Collapsible(myIndex, !isCollapsed);
        isCollapsed = !isCollapsed;
    }

    public void SetCallback(GameObject handle, string fun = null)
    {
        handleObject = handle;
        clickFunctionName = fun;
    }

    public virtual void OnClick()
    {
        if (handleObject != null && clickFunctionName != null)
        {
            handleObject.SendMessage(clickFunctionName, this, SendMessageOptions.DontRequireReceiver);
        }
    }
}
