using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGridEx : MonoBehaviour
{
	public enum EmDirection
	{
		Horizontal,
		Vertical,
	}

	public UIPanel clipPanel;

	//单元格尺寸
	public float cellWith;
	public float cellHeight;

	//行数
	public int itemPerRow = 0;
	public int itemPerCol = 0;

	//滚动位置
	private Vector3 scrollPos;

	public EmDirection dir;

	//所有列表项从这个克隆，这个项也会使用
	public UIGridExItemCtrlBase templateItem;

	public LinkedList<UIGridExItemCtrlBase> itemList = new LinkedList<UIGridExItemCtrlBase>();

	private LinkedList<UIGridExItemCtrlBase> cachedItemList = new LinkedList<UIGridExItemCtrlBase>();//回收的item项在这里，以便重复利用，只增不减。尾进头出。

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//isVariableHeight的话只能是单行或者单列
	public void SetGrid(int itemCount, System.Object userData, bool isVariableHeight)
	{
		if(cachedItemList.Count == 0 && itemList.Count == 0)
		{
			templateItem.exGrid = this;
			cachedItemList.AddLast(templateItem);
		}

		Vector3 startPos = Vector3.zero;
		Vector4 clipRegion = clipPanel.finalClipRegion;

		int maxRowCount = 0;
		if(dir == EmDirection.Vertical)
			maxRowCount = (int)(clipRegion.z/cellWith) + 2;//最下面多出两个，一个是部分显示的，一个是为了平滑滚动额外增加的
		int maxNoScrollItemCount = (int)(clipRegion.z/cellWith) * itemPerRow;

		int xCount = 0;
		int yCount = 0;
		Vector3 pos = startPos;
		for(int i=0; i<itemCount; i++)
		{
			UIGridExItemCtrlBase item = GetItem();
			item.transform.localPosition = pos;
			itemList.AddLast(item);

			xCount++;

			//下一个item的位置
			if(xCount < itemPerRow)
			{
				pos.x += cellWith;
			}
			else
			{
				pos.x = startPos.x;
				pos.y -= cellHeight;
				xCount = 0;
				yCount++;
			}
		}

		if(isVariableHeight)
		{
		}
	}

	UIGridExItemCtrlBase GetItem()
	{
		UIGridExItemCtrlBase item = null;
		if(cachedItemList.Count == 0)
		{
			item = GameObject.Instantiate<UIGridExItemCtrlBase>(templateItem);
			item.transform.parent = transform;
			item.transform.localScale = Vector3.one;
			item.exGrid = this;
		}
		else
		{
			item = cachedItemList.First.Value;
			cachedItemList.RemoveFirst();
		}
		return item;
	}

	public void OnDrag(Vector2 delta)
	{
		//刷新位置
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
		while(node != null)
		{
			node.Value.transform.localPosition += new Vector3(delta.x, delta.y, 0);
			node = node.Next;
		}

		//上面刚好看不见的那行保留，其他超出的行回收利用
		node = itemList.First;
		int line = node.Value.yIndex;
		float top = clipPanel.finalClipRegion.y + clipPanel.finalClipRegion.w * 0.5f;
		while(node != null)
		{
			if(node.Value.transform.localPosition.y > top + cellHeight * 1.5f)
			{
				itemList.Remove(node);
				cachedItemList.AddLast(node);// recycle it
				Debug.Log("recycle " + cachedItemList.Count);
			}

			node = node.Next;
		}

	}
}
