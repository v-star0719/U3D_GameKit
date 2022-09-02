using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
 * 动态列表，即列表项数变化的列表。当列表数量少不会超出滚动区域太多时建议使用。
 * 
 */

[ExecuteInEditMode]
public class UIDynamicListCtrl : MonoBehaviour
{
	public List<UIDynamicListItemCtrlBase> itemList = new List<UIDynamicListItemCtrlBase>();
	public UIDynamicListItemCtrlBase itemPrefab;//如果为null，则使用itemList[0]作为预置
	public float itemHeight;
	public float itemWidth;
	public int itemCountPerLine = 0;//小于等于0不做限制
	public string itemNamePrefix = "";

	private int dataCount;
	private object dataList;

	private Action<UIDynamicListItemCtrlBase> onClick;

	private UIScrollView mScrollView = null;
	private UIScrollView scrollView
	{
		get
		{
			if(mScrollView != null) return mScrollView;
			else
			{
				mScrollView = GetComponentInParent<UIScrollView>();
				return mScrollView;
			}
		}
	}

	/* <设置动态列表>
	 * 参数：
	 *     _dataCount: 数据量
	 *     _dataList: 数据列表，具体类型user在使用时自行转换
	 */
	public void SetList(int _dataCount, object _dataList, Action<UIDynamicListItemCtrlBase> onClick)
	{
		this.onClick = onClick;
		if(itemPrefab == null && itemList.Count == 0)
        {
			Debug.LogError("there is no item prefab");
			return;
		}
        else if(itemPrefab == null)
        {
            itemPrefab = itemList[0];
        }
		else
		{
			itemPrefab.gameObject.SetActive(false);
		}
		dataCount = _dataCount;
		dataList = _dataList;

		Vector3 pos = Vector3.zero;
		int n = 0;
		for(int i=0; i<dataCount; i++)
		{
			UIDynamicListItemCtrlBase item = null;
			Transform trans = null;
			if(i >= itemList.Count)
			{
				//不够，创建新的
				item = GameObject.Instantiate<UIDynamicListItemCtrlBase>(itemPrefab);
				trans = item.transform;
				item.name = itemNamePrefix + i;
				trans.parent = transform;
				trans.localScale = Vector3.one;
				item.parent = this;
				itemList.Add(item);
			}
			else{
				item = itemList[i];
				trans = item.transform;
			}
			
			item.gameObject.SetActive(true);
			item.SetData(i, dataList);

			//排列
			trans.localPosition = pos;
			pos.x += itemWidth;
			n++;
			if(itemCountPerLine > 0 && n >= itemCountPerLine)
			{
				pos.x = 0;
				pos.y -= itemHeight;
				n = 0;
			}
		}

		for(int i=dataCount; i<itemList.Count; i++)
			itemList[i].gameObject.SetActive(false);
	}

	public void Reposition()
	{
		Vector3 pos = Vector3.zero;
		int n = 0;
		for(int i=0; i<transform.childCount; i++)
		{
			Transform trans = transform.GetChild(i);			
			trans.localPosition = pos;
			pos.x += itemWidth;
			n++;
			if(itemCountPerLine > 0 && n >= itemCountPerLine)
			{
				pos.x = 0;
				pos.y -= itemHeight;
				n = 0;
			}
		}
	}

	public void OnItemClick(UIDynamicListItemCtrlBase item)
	{
		if (onClick != null)
		{
			onClick(item);
		}
	}

	public static void RepositonT<T>(List<T> itemList, bool isHorz, float width, float height, int itemPerLine) where T : MonoBehaviour
	{
	}
}
