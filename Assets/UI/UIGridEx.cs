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

	public enum EmPivot
	{
		TopCenter,
		TopLeft
	}

	public UIPanel clipPanel;

	//单元格尺寸
	public float cellWith;
	public float cellHeight;

	//行数
	public int itemPerRow = 0;
	public int itemPerCol = 0;

	public bool isVariableWidth = false;
	public bool isVariableHeight = false;

	//滚动位置
	private Vector3 scrollPos;

	public EmDirection dir;
	public EmPivot pivot;

	//所有列表项从这个克隆，这个项也会使用
	public UIGridExItemCtrlBase templateItem;
	
	public AnimationCurve springCurve;

	public LinkedList<UIGridExItemCtrlBase> itemList = new LinkedList<UIGridExItemCtrlBase>();

	private LinkedList<UIGridExItemCtrlBase> cachedItemList = new LinkedList<UIGridExItemCtrlBase>();//回收的item项在这里，以便重复利用，只增不减。尾进头出。

	private bool isInited = false;
	private Vector4 clipRegion;
	private float clipRegionLeft;
	private float clipRegionRight;
	private float clipRegionTop;
	private float clipRegionBottom;


	private Vector3 startPos = Vector3.zero;
	private System.Object userData;
	private int dataCount = 0;
	private bool isFit = false;

	private Vector2 lastDragDelta;
	//private vec

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

	void Init()
	{
		if(isInited) return;

		if(cachedItemList.Count == 0 && itemList.Count == 0)
		{
			cachedItemList.AddLast(templateItem);
		}

		//the center and offser of panel clipRegion is all vector3.zero
		clipRegion = clipPanel.finalClipRegion;
		clipRegionRight = clipRegion.z * 0.5f;
		clipRegionLeft = -clipRegionRight;
		clipRegionTop = clipRegion.w * 0.5f;
		clipRegionBottom = -clipRegionTop;

		isInited = true;
	}

	//isVariableHeight的话只能是单行或者单列
	public void SetGrid(int _dataCount, System.Object _userData)
	{
		Init();

		if(isVariableHeight && itemPerRow != 1)
		{
			itemPerRow = 1;
			Debug.LogError("itemPerRow must be 1, if it is variable height");
		}

		userData = _userData;
		dataCount = _dataCount;

		RecycleAllItems();

		

		int xIndex = 0;
		int yIndex = 0;
		Vector3 pos = Vector3.zero;
		bool isChaneLine = false;
		for(int i=0; i<_dataCount; i++)
		{
			UIGridExItemCtrlBase item = GetItem();
			item.Init(i, xIndex, yIndex, cellWith, cellHeight, this);
			item.SetData(userData);
			item.Prepare();

			//calculate start pos here, because in variable height mode, the first item's height is confirmed now
			if(i == 0)
			{
				if(pivot == EmPivot.TopCenter)
				{
					startPos.x = (1 - itemPerRow) * item.halfHeight;//it is:  -itemPerRow * cellWith*0.5f + cellWith * 0.5f;
					startPos.y = clipRegionTop - item.halfHeight;
				}
				else if(pivot == EmPivot.TopLeft)
				{
					startPos.x = clipRegionLeft + item.halfWidth;
					startPos.y = clipRegionTop - item.halfHeight;
				}
				pos = startPos;
			}

			if(isChaneLine)
			{
				pos.y -= item.halfHeight;//cur item's half height, and next item's half height
				isChaneLine = false;
			}

			item.transform.localPosition = pos;
			itemList.AddLast(item);

			xIndex++;

			//下一个item的位置
			if(xIndex < itemPerRow)
			{
				pos.x += item.width;
			}
			else
			{
				pos.x = startPos.x;
				pos.y -= item.halfHeight;//cur item's half height, and next item's half height
				xIndex = 0;
				yIndex++;
				isChaneLine = true;

				//if the last item's bottom is out of clip region, stop show item
				if(item.bottom < clipRegionBottom)
					break;
			}
		}

		isFit = false;
		if(itemList.Count > 0)
		{
			UIGridExItemCtrlBase item = itemList.Last.Value;
			isFit = item.bottom >= clipRegionBottom && itemList.Count == dataCount;
		}
	}

	void RecycleAllItems()
	{
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
		while(node != null)
		{
			LinkedListNode<UIGridExItemCtrlBase> recycleNode = node;
			node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);//move away
			node = node.Next;
			itemList.Remove(recycleNode);
			cachedItemList.AddLast(recycleNode);
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
		}
		else
		{
			item = cachedItemList.First.Value;
			cachedItemList.RemoveFirst();
		}
		return item;
	}

	public void OnDragStart()
	{
		isSpring = false;
	}

	public void OnDrag(Vector2 delta)
	{
		if(dir == EmDirection.Vertical && delta.y != 0)
			lastDragDelta = delta;

		if(dir == EmDirection.Horizontal) delta.y = 0;
		else if(dir == EmDirection.Vertical) delta.x = 0;

		if(!isSpring)
		{
			if(dir == EmDirection.Vertical)
			{
				if(delta.y < -cellHeight) delta.y = -cellHeight;
				if(delta.y > cellHeight) delta.y = cellHeight;

				if(delta.y > 0)
				{
					//it is moving up
					if(isFit)
					{
						//if it is fit, the toppest item can't move too faraway
						LinkedListNode<UIGridExItemCtrlBase> node2 = itemList.First;
						float paddingTop = node2.Value.top - clipRegionTop;
						delta.y = Mathf.Lerp(delta.y, 0, paddingTop/MAX_PADDING);//the closer paddingTop to MAX_PADDING, the slower the darg speed will be.
					}
					else
					{
						//if it is not fit, the bottomest item can't move too faraway
						LinkedListNode<UIGridExItemCtrlBase> node2 = itemList.Last;
						float paddingBottom = node2.Value.bottom - clipRegionBottom;
						delta.y = Mathf.Lerp(delta.y, 0, paddingBottom/MAX_PADDING);//the closer paddingBottom to MAXPADDING, the slower the darg speed will be.
					}
				}
				else
				{
					//it is moving down, the first line can't move too faraway
					LinkedListNode<UIGridExItemCtrlBase> node2 = itemList.First;
					float paddingTop = clipRegionTop - node2.Value.top;
					delta.y = Mathf.Lerp(delta.y, 0, paddingTop/MAX_PADDING);//the closer paddingTop to MAXPADDING, the slower the darg speed will be.
				}
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
		bool enableSpring = false;

		//if there is padding at top, then spring to top, no matter wether there is padding at bottom
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
		springDist = clipRegionTop - node.Value.top;
		if(isFit)
			enableSpring = true;//if it is fit, always spring the top item to fit top age
		else
			enableSpring = springDist > 0;

		//if there is no padding at top, then consider of spring to bottom
		if(!enableSpring)
		{
			node = itemList.Last;
			springDist = clipRegionBottom - node.Value.bottom;
			if(springDist < 0)
				enableSpring = true;
		}

		//if there is no padding, spring one item to fit with the top or bottm edge
		if(!enableSpring)
		{
			if(lastDragDelta.y < 0)
			{
				//it is moving down, spring the top item to fit with top edge
				//we don't spring the bottom item, because it may make the first to far to top when there are last two items.
				node = itemList.First;
				//find the first one that partly visible
				while(node != null)
				{
					if(node.Value.bottom < clipRegionTop && node.Value.top > clipRegionTop)
					{
						springDist = clipRegionTop - node.Value.top;
						enableSpring = true;
						break;
					}
					node = node.Next;
				}
			}
			else
			{
				//it is moving up, spring the bottom item to fit with bottom edge
				//we don't spring the top item, because it may make the last to far to bottom when there are last two items.
				node = itemList.Last;
				//find the last one that partly visible
				while(node != null)
				{
					float distToBottom = clipRegionBottom - node.Value.bottom;
					if(node.Value.bottom < clipRegionBottom && node.Value.top > clipRegionBottom)
					{
						springDist = clipRegionBottom - node.Value.bottom;
						enableSpring = true;
						break;
					}
					node = node.Previous;
				}
			}
		}

		if(enableSpring)
		{
			isSpring = true;
			timer = 0;
			preFrameSprintPos = 0;
		}
	}

	//the line just out of the clip region will be recycled
	void RecycleTop()
	{
		if(itemList.Count == 0 || isFit) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;

		while(node != null)
		{
			if(node.Value.bottom > clipRegionTop)
			{
				node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);
				LinkedListNode<UIGridExItemCtrlBase> recycleNode = node;
				node = node.Next;
				itemList.Remove(recycleNode);
				cachedItemList.AddLast(recycleNode);// recycle it
				Debug.LogFormat("recycle top, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
			}
			else
				break;
		}
	}

	//the line just out of the clip region will be recycled.
	void RecyleBottom()
	{
		if(itemList.Count == 0 || isFit) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.Last;

		while(node != null)
		{
			if(node.Value.top < clipRegionBottom)
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

	//if the last line is visible, add new line after it.
	void AddToBottom()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.Last;
		if(node.Value.index == dataCount-1) return;
		
		if(node.Value.top > clipRegionBottom)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line end
			pos.x = startPos.x;
			pos.y -= node.Value.halfHeight;//cur item's half height, and next item's half height
			for(int i=1; i<=itemPerRow; i++)
			{
				UIGridExItemCtrlBase item = GetItem();
				item.Init(node.Value.index + i, i - 1, node.Value.yIndex + 1, cellWith, cellHeight, this);
				item.SetData(userData);
				item.Prepare();

				if(i == 1)
					pos.y -= item.halfHeight;//cur item's half height, and next item's half height

				item.transform.localPosition = pos;
				pos.x += cellWith;
				itemList.AddLast(item);
				Debug.Log("add to bottom " + cachedItemList.Count);

				if(item.index == dataCount - 1)
					break;
			}
		}
	}

	//if the last line is visible, then add new line after it.
	void AddToTop()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIGridExItemCtrlBase> node = itemList.First;
		if(node.Value.index == 0) return;

		if(node.Value.bottom < clipRegionTop)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line begin
			pos.x += cellWith*(itemPerRow-1);
			pos.y += node.Value.halfHeight;//cur item's half height, and next item's half height
			for(int i=0; i<itemPerRow; i++)
			{
				UIGridExItemCtrlBase item = GetItem();
				item.Init(node.Value.index - i - 1, itemPerRow - i - 1, node.Value.yIndex - 1, cellWith, cellHeight, this);
				item.SetData(userData);
				item.Prepare();
				if(i == 0)
					pos.y += item.halfHeight;

				item.transform.localPosition = pos;
				pos.x -= cellWith;
				itemList.AddFirst(item);
				Debug.Log("add to top " + cachedItemList.Count);
			}
		}
	}
}
