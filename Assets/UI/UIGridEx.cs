using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGridEx : MonoBehaviour
{
	const float MAX_PADDING = 100;//the distance that user can drag moved when no new item can show
	const float SPRING_TIME = 1;

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
	
	public AnimationCurve springCurve;

	public LinkedList<UIGridExItemCtrlBase> itemList = new LinkedList<UIGridExItemCtrlBase>();

	private LinkedList<UIGridExItemCtrlBase> cachedItemList = new LinkedList<UIGridExItemCtrlBase>();//回收的item项在这里，以便重复利用，只增不减。尾进头出。

	private Vector3 startPos = Vector3.zero;
	private System.Object userData;
	private int dataCount = 0;

	private Vector2 lastDragDelta;

	private bool isSpring = false;
	private float springDist = 0;
	private float preFrameSprintPos = 0;
	private float timer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Update_Spring();
	}

	void Update_Spring()
	{
		if(!isSpring) return;

		timer += Time.deltaTime;
		float f = timer/SPRING_TIME;
		f = springCurve.Evaluate(f)*springDist;
		OnDrag(new Vector2(0, f - preFrameSprintPos));
		preFrameSprintPos = f;

		if(timer >= SPRING_TIME)
			isSpring = false;
	}

	//isVariableHeight的话只能是单行或者单列
	public void SetGrid(int _dataCount, System.Object _userData, bool isVariableHeight)
	{
		userData = _userData;
		dataCount = _dataCount;
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

		int xIndex = 0;
		int yIndex = 0;
		Vector3 pos = startPos;
		for(int i=0; i<_dataCount; i++)
		{
			UIGridExItemCtrlBase item = GetItem();
			item.transform.localPosition = pos;
			item.index = i;
			item.xIndex = xIndex;
			item.yIndex = yIndex;
			itemList.AddLast(item);
			item.SetData(userData);

			xIndex++;

			//下一个item的位置
			if(xIndex < itemPerRow)
			{
				pos.x += cellWith;
			}
			else
			{
				pos.x = startPos.x;
				pos.y -= cellHeight;
				xIndex = 0;
				yIndex++;
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
		lastDragDelta = delta;

		if(dir == EmDirection.Horizontal) delta.y = 0;
		else if(dir == EmDirection.Vertical) delta.x = 0;

		if(dir == EmDirection.Vertical)
		{
			if(delta.y < -cellHeight) delta.y = -cellHeight;
			if(delta.y > cellHeight) delta.y = cellHeight;

			Vector4 clipRegion = clipPanel.finalClipRegion;
			if(delta.y > 0)
			{
				LinkedListNode<UIGridExItemCtrlBase> node2 = itemList.Last;
				float bottom = clipRegion.y - clipRegion.w * 0.5f;
				float padding = node2.Value.transform.localPosition.y - bottom;
				delta.y = Mathf.Lerp(delta.y, 0, padding/MAX_PADDING);
			}
			else
			{
				LinkedListNode<UIGridExItemCtrlBase> node2 = itemList.First;
				float top = clipRegion.y + clipRegion.w * 0.5f;
				float padding = top - node2.Value.transform.localPosition.y;

				//predict wether the first item will move below the padding line
				float itemBottom = node2.Value.transform.localPosition.y + delta.y - cellHeight*0.5f;
				if(itemBottom < top - MAX_PADDING)
					delta.y = (top - MAX_PADDING) - (node2.Value.transform.localPosition.y - cellHeight*0.5f);

				delta.y = Mathf.Lerp(delta.y, 0, padding/MAX_PADDING);
			}
		}


		//update position
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
		while(node != null)
		{
			node.Value.transform.localPosition += new Vector3(delta.x, delta.y, 0);
			node = node.Next;
		}

		//handle vertically move
		if(delta.y > 0)//move up
		{
			RecycleTop();
			AddToBottom();
		}
		else
		{
			RecyleBottom();
			AddToTop();
		}
	}

	public void OnDragEnd()
	{
		if(lastDragDelta.y < 0)
		{
			LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
			Vector4 clipRegion = clipPanel.finalClipRegion;
			float top = clipRegion.y + clipRegion.w * 0.5f - cellHeight*0.5f;
			springDist = top - node.Value.transform.localPosition.y;
		}
		else
		{
			LinkedListNode<UIGridExItemCtrlBase> node = itemList.Last;
			Vector4 clipRegion = clipPanel.finalClipRegion;
			float bottom = clipRegion.y - clipRegion.w * 0.5f + cellHeight*0.5f;
			springDist = bottom - node.Value.transform.localPosition.y;
		}
		isSpring = true;
		timer = 0;
		preFrameSprintPos = 0;
	}

	//the line just out of the clip region is reserved, other lines will be recycled
	void RecycleTop()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;

		int yIndex = node.Value.yIndex;
		Vector4 clipRegion = clipPanel.finalClipRegion;
		float top = clipRegion.y + clipRegion.w * 0.5f;
		while(node != null)
		{
			if(node.Value.transform.localPosition.y > top + cellHeight * 1.5f)
			{
				node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);
				LinkedListNode<UIGridExItemCtrlBase> delNode = node;
				node = node.Next;
				itemList.Remove(delNode);
				cachedItemList.AddLast(delNode);// recycle it
				Debug.LogFormat("recycle top, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
			}
			else
				break;
		}
	}

	//the line just out of the clip region is reserved, other lines will be recycled.
	void RecyleBottom()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.Last;

		int yIndex = node.Value.yIndex;
		Vector4 clipRegion = clipPanel.finalClipRegion;
		float bottom = clipRegion.y - clipRegion.w * 0.5f;
		while(node != null)
		{
			if(node.Value.transform.localPosition.y < bottom - cellHeight * 1.5f)
			{
				node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);//move away
				LinkedListNode<UIGridExItemCtrlBase> delNode = node;
				node = node.Previous;
				itemList.Remove(delNode);
				cachedItemList.AddLast(delNode);// recycle it
				Debug.LogFormat("recycle bottom, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
			}
			else
				break;
		}
	}

	//if the last line is just out side the region, then add new line after it.
	void AddToBottom()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.Last;
		if(node.Value.index == dataCount-1) return;
		
		Vector4 clipRegion = clipPanel.finalClipRegion;
		float bottom = clipRegion.y - clipRegion.w * 0.5f;
		if(node.Value.transform.localPosition.y > bottom - cellHeight * 0.5f)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line end
			pos.x = startPos.x;
			pos.y -= cellHeight;
			for(int i=1; i<=itemPerRow; i++)
			{
				UIGridExItemCtrlBase item = GetItem();
				item.index = node.Value.index + i;
				item.xIndex = i - 1;
				item.yIndex = node.Value.yIndex;
				item.transform.localPosition = pos;
				pos.x += cellWith;
				item.SetData(userData);
				itemList.AddLast(item);
				Debug.Log("add to bottom " + cachedItemList.Count);
			}
		}
	}

	//if the last line is just out side the region, then add new line after it.
	void AddToTop()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
		if(node.Value.index == 0) return;

		Vector4 clipRegion = clipPanel.finalClipRegion;
		float top = clipRegion.y + clipRegion.w * 0.5f;
		if(node.Value.transform.localPosition.y < top + cellHeight * 0.5f)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line begin
			pos.x += cellWith*(itemPerRow-1);
			pos.y += cellHeight;
			for(int i=0; i<itemPerRow; i++)
			{
				UIGridExItemCtrlBase item = GetItem();
				item.index = node.Value.index - i - 1;
				item.xIndex = itemPerRow - i - 1;
				item.yIndex = node.Value.yIndex - 1;
				item.transform.localPosition = pos;
				pos.x -= cellWith;
				item.SetData(userData);
				itemList.AddFirst(item);
				Debug.Log("add to top " + cachedItemList.Count);
			}
		}
	}
}
