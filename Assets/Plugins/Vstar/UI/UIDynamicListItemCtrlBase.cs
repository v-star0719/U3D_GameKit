using UnityEngine;
using System.Collections;

public class UIDynamicListItemCtrlBase : MonoBehaviour
{
	[HideInInspector]
	public UIDynamicListCtrl parent;
	//index是数据索引，已确保是在[0, dataList.count)范围内
	//dataList是数据列表，具体列表的类型由user决定，控件不关心这个
	public virtual void SetData(int index, object dataList)
	{
		Debug.Log("UIDynamicListItemCtrlBase.SetData: " + index);
	}

	public void OnClick()
	{
		parent.OnItemClick(this);
	}
}
