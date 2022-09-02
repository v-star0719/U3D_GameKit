using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///用于循环滚动列表，比如寻仙的记录
[ExecuteInEditMode]
public class UICirculationRollingList : MonoBehaviour
{
	public delegate void SetDataCallback(int itemIndex, int dataIndex);

	public float rollAreaHeight = 0;
	public float rollAreaWidth = 0;
	public float rollSpeed = 0;
	public float itemHeight;
	public float itemGap;
	public UICirculationRollingListItemBase[] itemArray = new UICirculationRollingListItemBase[0];
	public bool reposition = false;

	private int dataIndex = 0;//即: 最上面那一项的数据索引
	private List<UICirculationRollingListItemBase> rollItemList = null;//头部是最上面的列表，尾部是最下面的节点
	private bool enableRoll = false;
	private object dataList = null;
	private int dataCount = 0;
	
	//
	//_itemList是列表项列表，最佳列表项数是比滚动区域可显示的列表数多1。
	//
	public void SetList(int _dataCount, object _dataList)
	{
		int itemCountInArean = (int)((rollAreaHeight + itemGap)/(itemHeight + itemGap));//区域内可刚好完全显示不滚动的列表数量

		if(rollItemList == null) rollItemList = new List<UICirculationRollingListItemBase>(itemArray.Length);
		dataCount = _dataCount;
		dataList = _dataList;
		enableRoll = dataCount > itemCountInArean;//数据量太多才滚动


		dataIndex = 0;
		rollItemList.Clear();
		float y = (rollAreaHeight - itemHeight)*0.5f;
		Vector3 pos;
		for(int i=0; i<itemArray.Length; i++)
		{
			//排列
			Transform trans = itemArray[i].transform;
			pos = trans.localPosition;
			pos.y = y;
			trans.localPosition = pos;
			rollItemList.Add(itemArray[i]);
			y -= itemHeight + itemGap;
			//设置列表项
			if(enableRoll || i<dataCount)
			{
				itemArray[i].SetData(i%dataCount, dataList);
				trans.gameObject.SetActive(true);
			}
			else
			{
				trans.gameObject.SetActive(false);
			}
		}
	}

	public bool testBtn = false;
	public int testDataCount = 0;
	// Update is called once per frame
	void Update ()
	{
		if(testBtn)
		{
			SetList(testDataCount, null);
			testBtn = false;
			return;
		}

		if(reposition){
			Reposition();
			reposition = false;
			return;
		}

		if(!enableRoll) return;

		float deltaTime = 0.04f;
		if(Time.deltaTime < 0.04f)
			deltaTime = Time.deltaTime;

		//移动
		Vector3 delta = Vector3.zero;
		delta.y = rollSpeed * deltaTime;
		for(int i=0; i<rollItemList.Count; i++)
			rollItemList[i].transform.localPosition += delta;

		//最上面那个是否超出了
		Transform trans = rollItemList[0].transform;
		delta = trans.localPosition;//借用下变量
		if(delta.y >= (rollAreaHeight + itemHeight)*0.5f)
		{
			//挪到最下面去
			//坐标计算
			delta = rollItemList[rollItemList.Count-1].transform.localPosition;
			delta.y -= itemHeight + itemGap;
			trans.localPosition = delta;
			//列表更新
			UICirculationRollingListItemBase first = rollItemList[0];
			rollItemList.RemoveAt(0);
			rollItemList.Add(first);
			//列表项刷新
			dataIndex++;
			if(dataIndex >= dataCount)
				dataIndex = 0;
			first.SetData(dataIndex, dataList);
		}
	}

	void Reposition()
	{
		float y = (rollAreaHeight - itemHeight)*0.5f;
		Vector3 pos;
		for(int i=0; i<itemArray.Length; i++)
		{
			Transform trans = itemArray[i].transform;
			pos = trans.localPosition;
			pos.y = y;
			trans.localPosition = pos;
			y -= itemHeight + itemGap;
		}
	}

	void OnDrawGizmos()
	{
		Vector3 size = new Vector3(rollAreaWidth, rollAreaHeight, 0);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position, size);
	}

	public void Stop()
	{
		enableRoll = false;
	}
}
