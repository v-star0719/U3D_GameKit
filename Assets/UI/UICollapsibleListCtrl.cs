using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 使用说明：
 * （1）从UICollapsibleListItemCtrlBase派生一个自己的列表项控件，重写SetData方法。
 * （2）编辑好一个列表项就可以，其他项从它克隆
 * （3）调用列表的Set方法创建列表
 * 
 * 注意：
 * （1）列表项控件分成了2个部分：titleBar和Content，content是进行伸缩的部分。这个两个部分都应该添加BoxCollider和UIDragScrollView以进行鼠标响应和拖拽
 */

public class UICollapsibleListCtrl : MonoBehaviour
{
	public float itemGap;
	public float collapsingTime;
	public System.Object userData;
	public List<UICollapsibleListItemCtrlBase> itemList = new List<UICollapsibleListItemCtrlBase>();
	[HideInInspector]public bool isCollapsing = false;
	[HideInInspector]public bool isExpanding = false;

	private int usingItemCount = 0;
	private VoidDelegate callBack;
	private int collapsingItemIndex = -1;
	private float collapsingTimer = 0;

	// Update is called once per frame
	void Update ()
	{
		if(collapsingItemIndex >= 0)
		{
			UICollapsibleListItemCtrlBase ctrl = itemList[collapsingItemIndex];
			float deltaTime = 0.04f;
			if(Time.deltaTime < 0.04f)
				deltaTime = Time.deltaTime;
			collapsingTimer += deltaTime;

			//伸缩
			float scale = 1f;
			if(collapsingTimer < collapsingTime)
				scale = collapsingTimer/collapsingTime;
			if(isCollapsing)
				scale = 1 - scale;
			ctrl.content.localScale = new Vector3(1, scale, 1);
			ctrl.curContentHeight = scale*ctrl.contentHeight;

			//重新计算位置
			CalculateItemPos(collapsingItemIndex+1, -1);

			//是否折叠完成
			if(collapsingTimer >= collapsingTime)
			{
				if(isCollapsing)
					ctrl.content.gameObject.SetActive(false);//隐藏起来
				isCollapsing = false;
				isExpanding = false;
				collapsingItemIndex = -1;
                if (callBack != null) callBack();
			}
		}
	}

	///列表项的局部坐标原点都要求在标题栏的中心，可折叠部分的坐标原点在上边中心
	///itemCount: 需要多少列表项
	///_listData: 列表数据
	///collapseArg: 哪个项是折叠的: 
	///     -2 = 保留上次的折叠情况; 
	///     -1 = 所有项折叠; 
	///     [0, 项数)=指定项不折叠; 
	///     [项数,~) = 所有项均不折叠
	public void Set(int itemCount, System.Object _userData, int collapseArg)
	{
		if(itemList.Count < 1)
		{
			Debug.LogError("itemList is empty, must has at least one item");
			return;
		}

		userData = _userData;
		usingItemCount = itemCount;
		int preItemCount = itemList.Count;
		for(int i=0; i<itemCount; i++)
		{
		    Transform itemTrans;
		    UICollapsibleListItemCtrlBase itemCtrl;
			if(i >= preItemCount)
			{
				itemCtrl = Instantiate(itemList[0]);
				itemCtrl.listCtrl = this;
				itemCtrl.myIndex = i;
				itemList.Add(itemCtrl);
				//
				itemTrans = itemCtrl.transform;
				itemTrans.parent = transform;
				itemTrans.localScale = Vector3.one;
			}
			else
			{
				itemCtrl = itemList[i];
				itemCtrl.listCtrl = this;
				itemCtrl.myIndex = i;
				//
				itemTrans = itemCtrl.transform;
			}

            itemCtrl.gameObject.SetActive(true);
            itemCtrl.SetData(i, userData);

		    //是否展开处理
			bool isCollapsed = false;
			if(collapseArg == -2)
				isCollapsed = itemCtrl.isCollapsed;//保留上次的折叠情况
			if(collapseArg == -1)
				isCollapsed = true;//所有项折叠
			else if(collapseArg >=0 && collapseArg < itemCount)
				isCollapsed = (collapseArg != i);//指定项不折叠
			//else
			//	isCollapsed = false;//所有项均折叠，默认值就是这个

			//折叠/展开处理
			itemCtrl.isCollapsed = isCollapsed;
			if(isCollapsed)
			{
				itemCtrl.curContentHeight = 0;
				itemCtrl.content.gameObject.SetActive(false);
				itemCtrl.content.localScale = new Vector3(1, 0, 1);
			}
			else
			{
				itemCtrl.curContentHeight = itemCtrl.contentHeight;
				itemCtrl.content.gameObject.SetActive(true);
				itemCtrl.content.localScale = new Vector3(1, 1, 1);
			}
			CalculateItemPos(i, 1);
		}

		//多余的隐藏
		for(int i=usingItemCount; i<itemList.Count; i++)
			itemList[i].gameObject.SetActive(false);
	}

	///count: -1表示重排后面所有的，大于等于0表示从startIndex处开始起重排的数量
	private void CalculateItemPos(int startIndex, int count)
	{
		int end = startIndex + count;
		if(count < 0 || end > usingItemCount)
			end = usingItemCount;

		UICollapsibleListItemCtrlBase preItemCtrl;
		Vector3 pos;
		//string str = "";
		for(int i=startIndex; i<end; i++)
		{
			if(i > 0)
			{
				preItemCtrl = itemList[i-1];
				pos = preItemCtrl.trans.localPosition;
				pos.y = pos.y - preItemCtrl.titleBarHeight - preItemCtrl.realContentHeight - itemGap;
				itemList[i].trans.localPosition = pos;

			}
			else
				itemList[i].trans.localPosition = Vector3.zero;
			//str += " " + i;
		}
		//Debug.Log(str);
	}

	public void Collapsible(int itemIndex, bool isCollapse, VoidDelegate endFunc = null)
	{
		collapsingItemIndex = itemIndex;
		collapsingTimer = 0;
		isCollapsing = isCollapse;
		isExpanding = !isCollapse;
	    callBack = endFunc;
		if(isExpanding)
			itemList[itemIndex].content.gameObject.SetActive(true);//要展开，先显示出来
	}

    public void SetCallBackFunction(GameObject handle, string fun)
    {
        foreach (UICollapsibleListItemCtrlBase o in itemList)
        {
            UICollapsibleListItemCtrlBase itemCtrl = o.GetComponent<UICollapsibleListItemCtrlBase>();
            if (itemCtrl != null)
            {
                itemCtrl.SetCallback(handle, fun);
            }
        }
    }
}
