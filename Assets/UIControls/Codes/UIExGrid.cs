using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIExGrid : MonoBehaviour
{
	const float MAX_PADDING = 100;//the distance that user can drag moved when no new item can show
	const float SPRING_TIME = 1;

	public enum EmDirection
	{
		Horizontal,
		Vertical,
	}

	public enum EmPivotVert
	{
		TopCenter,
		TopLeft
	}

	public enum EmPivotHorz
	{
		LeftCenter,
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

	public EmDirection direction = EmDirection.Vertical;
	public EmPivotHorz pivotHorz;
	public EmPivotVert pivotVert;

	//滚动位置
	private Vector3 scrollPos;

	//所有列表项从这个克隆，这个项也会使用
	public UIExGridItemCtrlBase templateItem;
	
	public AnimationCurve springCurve;

	public LinkedList<UIExGridItemCtrlBase> itemList = new LinkedList<UIExGridItemCtrlBase>();

	private LinkedList<UIExGridItemCtrlBase> cachedItemList = new LinkedList<UIExGridItemCtrlBase>();//回收的item项在这里，以便重复利用，只增不减。尾进头出。

	private bool isInited = false;
	private Vector4 clipRegion;
	[HideInInspector]
	public float clipRegionLeft;
	[HideInInspector]
	public float clipRegionRight;
	[HideInInspector]
	public float clipRegionTop;
	[HideInInspector]
	public float clipRegionBottom;


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
		if(direction == EmDirection.Vertical)
			OnDrag(new Vector2(0, f - preFrameSprintPos));
		else
			OnDrag(new Vector2(f - preFrameSprintPos, 0));
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
		userData = _userData;
		dataCount = _dataCount;
		RecycleAllItems();


		if(direction == EmDirection.Vertical)
			SetGrid_Vert();
		else
			SetGrid_Horz();
		
	}

	void SetGrid_Vert()
	{
		if(isVariableHeight && itemPerRow != 1)
		{
			itemPerRow = 1;
			Debug.LogError("itemPerRow must be 1, if it is variable height");
		}

		int xIndex = 0;
		int yIndex = 0;
		Vector3 pos = Vector3.zero;
		bool isChangeLine = false;
		for(int i=0; i<dataCount; i++)
		{
			UIExGridItemCtrlBase item = GetItem();
			item.Init(i, xIndex, yIndex, cellWith, cellHeight, this);
			item.SetData(userData);
			item.Cache();
			item.SetBoxCollider();

			//calculate start pos here, because in variable height mode, the first item's height is confirmed now
			if(i == 0)
			{
				if(pivotVert == EmPivotVert.TopCenter)
				{
					startPos.x = (1 - itemPerRow) * item.halfWidth;//it is:  -itemPerRow * cellWith*0.5f + cellWith * 0.5f;
					startPos.y = clipRegionTop - item.halfHeight;
				}
				else if(pivotVert == EmPivotVert.TopLeft)
				{
					startPos.x = clipRegionLeft + item.halfWidth;
					startPos.y = clipRegionTop - item.halfHeight;
				}
				pos = startPos;
			}

			if(isChangeLine)
			{
				pos.y -= item.halfHeight;//cur item's half height, and next item's half height
				isChangeLine = false;
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
				isChangeLine = true;

				//if the last item's bottom is out of clip region, stop show item
				if(item.bottom < clipRegionBottom)
					break;
			}
		}

		isFit = false;
		if(itemList.Count > 0)
		{
			UIExGridItemCtrlBase item = itemList.Last.Value;
			isFit = item.bottom >= clipRegionBottom && itemList.Count == dataCount;
		}
	}

	public void SetGrid_Horz()
	{
		if(isVariableWidth && itemPerCol != 1)
		{
			itemPerCol = 1;
			Debug.LogError("itemPerCol must be 1, if it is variable width");
		}

		int xIndex = 0;
		int yIndex = 0;
		Vector3 pos = Vector3.zero;
		bool isChangeLine = false;
		for(int i=0; i<dataCount; i++)
		{
			UIExGridItemCtrlBase item = GetItem();
			item.Init(i, xIndex, yIndex, cellWith, cellHeight, this);
			item.SetData(userData);
			item.Cache();
			item.SetBoxCollider();

			//calculate start pos here, because in variable with mode, the first item's width is confirmed just now
			if(i == 0)
			{
				if(pivotHorz == EmPivotHorz.LeftCenter)
				{
					startPos.x = clipRegionLeft + item.halfWidth;
					startPos.y = (itemPerCol - 1) * item.halfHeight;//it is:  itemPerCol * cellHeigth*0.5f - cellHeight * 0.5f;
				}
				else if(pivotHorz == EmPivotHorz.TopLeft)
				{
					startPos.x = clipRegionLeft + item.halfWidth;
					startPos.y = clipRegionTop - item.halfHeight;
				}
				pos = startPos;
			}

			if(isChangeLine)
			{
				pos.x += item.halfWidth;//cur item's half width, and next item's half width
				isChangeLine = false;
			}

			item.transform.localPosition = pos;
			itemList.AddLast(item);

			yIndex++;

			//下一个item的位置
			if(yIndex < itemPerCol)
			{
				pos.y -= item.height;
			}
			else
			{
				pos.x += item.halfWidth;//cur item's half width, and next item's half width
				pos.y = startPos.y;
				xIndex++;
				yIndex = 0;
				isChangeLine = true;

				//if the last item's right is out of clip region, stop show item
				if(item.right > clipRegionRight)
					break;
			}
		}

		isFit = false;
		if(itemList.Count > 0)
		{
			UIExGridItemCtrlBase item = itemList.Last.Value;
			isFit = item.right <= clipRegionRight && itemList.Count == dataCount;
		}
	}

	void RecycleAllItems()
	{
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
		while(node != null)
		{
			LinkedListNode<UIExGridItemCtrlBase> recycleNode = node;
			node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);//move away
			node = node.Next;
			itemList.Remove(recycleNode);
			cachedItemList.AddLast(recycleNode);
		}
	}

	UIExGridItemCtrlBase GetItem()
	{
		UIExGridItemCtrlBase item = null;
		if(cachedItemList.Count == 0)
		{
			item = GameObject.Instantiate<UIExGridItemCtrlBase>(templateItem);
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
		if(direction == EmDirection.Vertical)
			OnDrag_Vert(delta);
		else
			OnDrag_Horz(delta);
	}

	void OnDrag_Vert(Vector2 delta)
	{
		delta.x = 0;
		if(delta.y != 0) lastDragDelta = delta;

		if(!isSpring)
		{
			if(delta.y > 0)
			{
				////clam the max delta.y to to item's top edge to bottom clip region edge
				//if(!isFit)
				//{
				//	float maxY = clipRegionBottom - itemList.Last.Value.bottom;
				//	if(maxY < 0) maxY = 0;
				//	if(delta.y > maxY) delta.y = maxY;
				//}

				//it is moving up
				if(isFit)
				{
					//if it is fit, the toppest item can't move too faraway
					LinkedListNode<UIExGridItemCtrlBase> node2 = itemList.First;
					float paddingTop = node2.Value.top - clipRegionTop;
					delta.y = Mathf.Lerp(delta.y, 0, paddingTop/MAX_PADDING);//the closer paddingTop to MAX_PADDING, the slower the darg speed will be.
				}
				else
				{
					//if it is not fit, the bottomest item can't move too faraway
					LinkedListNode<UIExGridItemCtrlBase> node2 = itemList.Last;
					float paddingBottom = node2.Value.bottom - clipRegionBottom;
					delta.y = Mathf.Lerp(delta.y, 0, paddingBottom/MAX_PADDING);//the closer paddingBottom to MAXPADDING, the slower the darg speed will be.
				}
			}
			else
			{
				////clam the max delta.y to to item's top edge to bottom clip region edge
				//if(!isFit)
				//{
				//	float maxY = clipRegionTop - itemList.First.Value.top;
				//	if(maxY > 0) maxY = 0;
				//	if(delta.y < maxY) delta.y = maxY;
				//}

				//it is moving down, the first line can't move too faraway
				LinkedListNode<UIExGridItemCtrlBase> node2 = itemList.First;
				float paddingTop = clipRegionTop - node2.Value.top;
				delta.y = Mathf.Lerp(delta.y, 0, paddingTop/MAX_PADDING);//the closer paddingTop to MAXPADDING, the slower the darg speed will be.
			}
		}

		//update position
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
		while(node != null)
		{
			node.Value.transform.localPosition += new Vector3(delta.x, delta.y, 0);
			node = node.Next;
		}

		if(delta.y > 0)//move up
		{
			RecycleTopOrLeft();
			AddToBottom();
		}
		else
		{
			RecyleBottomOrRight();
			AddToTop();
		}
	}

	void OnDrag_Horz(Vector2 delta)
	{
		delta.y = 0;
		if(delta.x != 0) lastDragDelta = delta;

		if(!isSpring)
		{
			if(delta.x < 0)
			{
				////clam the max delta.x to right item's right edge to right clip region edge
				//if(!isFit)
				//{
				//	float maxX = clipRegionRight - itemList.Last.Value.right;
				//	if(maxX > 0) maxX = 0;
				//	if(delta.x < maxX) delta.x = maxX;
				//}

				//it is moving left
				if(isFit)
				{
					//if it is fit, the leftest item can't move too faraway
					LinkedListNode<UIExGridItemCtrlBase> node2 = itemList.First;
					float paddingLeft = clipRegionLeft - node2.Value.left;
					delta.x = Mathf.Lerp(delta.x, 0, paddingLeft/MAX_PADDING);//the closer paddingLeft to MAX_PADDING, the slower the darg speed will be.
				}
				else
				{
					//if it is not fit, the rightest item can't move too faraway
					LinkedListNode<UIExGridItemCtrlBase> node2 = itemList.Last;
					float paddingRight = clipRegionRight - node2.Value.right;
					delta.x = Mathf.Lerp(delta.x, 0, paddingRight/MAX_PADDING);//the closer paddingRight to MAXPADDING, the slower the darg speed will be.
				}
			}
			else
			{
				////clam the max delta.x to left item's left edge to left clip region edge
				//if(!isFit)
				//{
				//	float maxX = clipRegionLeft - itemList.First.Value.left;
				//	if(maxX < 0) maxX = 0;
				//	if(delta.x > maxX) delta.x = maxX;
				//}

				//it is moving right, the first line can't move too faraway
				LinkedListNode<UIExGridItemCtrlBase> node2 = itemList.First;
				float paddingLeft = node2.Value.left - clipRegionLeft;
				delta.x = Mathf.Lerp(delta.x, 0, paddingLeft/MAX_PADDING);//the closer paddingLeft to MAXPADDING, the slower the darg speed will be.
			}
		}

		//update position
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
		while(node != null)
		{
			node.Value.transform.localPosition += new Vector3(delta.x, delta.y, 0);
			node = node.Next;
		}

		if(delta.x < 0)//move left
		{
			RecycleTopOrLeft();
			AddToRight();
		}
		else
		{
			RecyleBottomOrRight();
			AddToLeft();
		}
	}

	public void OnDragEnd()
	{
		if(direction == EmDirection.Vertical)
			OnDragEnd_Vert();
		else
			OnDragEnd_Horz();
	}

	void OnDragEnd_Vert()
	{
		bool enableSpring = false;

		//if there is padding at top, then spring to top, no matter wether there is padding at bottom
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
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

	void OnDragEnd_Horz()
	{
		bool enableSpring = false;

		//if there is padding at left, then spring to left, no matter wether there is padding at right
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
		springDist = clipRegionLeft - node.Value.left;
		if(isFit)
			enableSpring = true;//if it is fit, always spring the left item to fit left edge
		else
			enableSpring = springDist < 0;

		//if there is no padding at left, then consider of spring to right
		if(!enableSpring)
		{
			node = itemList.Last;
			springDist = clipRegionRight - node.Value.right;
			if(springDist > 0)
				enableSpring = true;
		}

		//if there is no padding, spring one item to fit with the left or right edge
		if(!enableSpring)
		{
			if(lastDragDelta.x > 0)
			{
				//it is moving right, spring the left item to fit with left edge
				//we don't spring the right item, because it may make the first to far to left when there are last two items.
				node = itemList.First;
				//find the first one that partly visible
				while(node != null)
				{
					if(node.Value.left < clipRegionLeft && node.Value.right > clipRegionLeft)
					{
						springDist = clipRegionLeft - node.Value.left;
						enableSpring = true;
						break;
					}
					node = node.Next;
				}
			}
			else
			{
				//it is moving left, spring the right item to fit with right edge
				//we don't spring the left item, because it may make the last to far to right when there are last two items.
				node = itemList.Last;
				//find the last one that partly visible
				while(node != null)
				{
					if(node.Value.left < clipRegionRight && node.Value.right > clipRegionRight)
					{
						springDist = clipRegionRight - node.Value.right;
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
	void RecycleTopOrLeft()
	{
		if(itemList.Count == 0 || isFit) return;
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;

		while(node != null)
		{
			bool recycle = false;
			if(direction == EmDirection.Vertical)
				recycle = node.Value.bottom > clipRegionTop;
			else
				recycle = node.Value.right < clipRegionLeft;

			if(recycle)
			{
				node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);
				LinkedListNode<UIExGridItemCtrlBase> recycleNode = node;
				node = node.Next;
				itemList.Remove(recycleNode);
				cachedItemList.AddLast(recycleNode);// recycle it
				//if(direction == EmDirection.Vertical)
				//	Debug.LogFormat("recycle top, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
				//else
				//	Debug.LogFormat("recycle left, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
			}
			else
				break;
		}
	}


	//the line just out of the clip region will be recycled.
	void RecyleBottomOrRight()
	{
		if(itemList.Count == 0 || isFit) return;
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.Last;

		while(node != null)
		{
			bool recycle = false;
			if(direction == EmDirection.Vertical)
				recycle = node.Value.top < clipRegionBottom;
			else
				recycle = node.Value.left > clipRegionRight;

			if(recycle)
			{
				node.Value.transform.localPosition =  new Vector3(float.MaxValue, 0, 0);//move away
				LinkedListNode<UIExGridItemCtrlBase> delNode = node;
				node = node.Previous;
				itemList.Remove(delNode);
				cachedItemList.AddLast(delNode);// recycle it
				//if(direction == EmDirection.Vertical)
				//	Debug.LogFormat("recycle bottom, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
				//else
				//	Debug.LogFormat("recycle right, item{0}, cachedItemCount = {1}", node.Value.index, cachedItemList.Count);
			}
			else
				break;
		}
	}


	//if the last line is visible, add new line after it.
	void AddToBottom()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.Last;
		if(node.Value.index == dataCount-1) return;
		
		if(node.Value.top > clipRegionBottom)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line end
			pos.x = startPos.x;
			pos.y -= node.Value.halfHeight;//cur item's half height, and next item's half height
			for(int i=1; i<=itemPerRow; i++)
			{
				UIExGridItemCtrlBase item = GetItem();
				item.Init(node.Value.index + i, i - 1, node.Value.yIndex + 1, cellWith, cellHeight, this);
				item.SetData(userData);
				item.Cache();
				item.SetBoxCollider();

				if(i == 1)
					pos.y -= item.halfHeight;//cur item's half height, and next item's half height

				item.transform.localPosition = pos;
				pos.x += cellWith;
				itemList.AddLast(item);
				//Debug.Log("add to bottom " + cachedItemList.Count);

				if(item.index == dataCount - 1)
					break;
			}
		}
	}

	//if the first line is visible, then add new line after it.
	void AddToTop()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
		if(node.Value.index == 0) return;

		if(node.Value.bottom < clipRegionTop)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line begin
			pos.x += cellWith*(itemPerRow-1);
			pos.y += node.Value.halfHeight;//cur item's half height, and next item's half height
			for(int i=0; i<itemPerRow; i++)
			{
				UIExGridItemCtrlBase item = GetItem();
				item.Init(node.Value.index - i - 1, itemPerRow - i - 1, node.Value.yIndex - 1, cellWith, cellHeight, this);
				item.SetData(userData);
				item.Cache();
				item.SetBoxCollider();
				if(i == 0)
					pos.y += item.halfHeight;

				item.transform.localPosition = pos;
				pos.x -= cellWith;
				itemList.AddFirst(item);
				//Debug.Log("add to top " + cachedItemList.Count);
			}
		}
	}

	//if the first line is visible, then add new line after it.
	void AddToLeft()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.First;
		if(node.Value.index == 0) return;

		if(node.Value.right > clipRegionLeft)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line begin
			pos.x -= node.Value.halfWidth;//cur item's half width, and next item's half width
			pos.y -= cellHeight*(itemPerCol-1);
			for(int i=0; i<itemPerCol; i++)
			{
				UIExGridItemCtrlBase item = GetItem();
				item.Init(node.Value.index - i - 1, node.Value.xIndex - 1, itemPerCol - i - 1, cellWith, cellHeight, this);
				item.SetData(userData);
				item.Cache();
				item.SetBoxCollider();
				if(i == 0)
					pos.x -= item.halfWidth;

				item.transform.localPosition = pos;
				pos.y += cellHeight;
				itemList.AddFirst(item);
				//Debug.Log("add to left " + cachedItemList.Count);
			}
		}
	}

	//if the last line is visible, add new line after it.
	void AddToRight()
	{
		if(itemList.Count == 0) return;
		LinkedListNode<UIExGridItemCtrlBase> node = itemList.Last;
		if(node.Value.index == dataCount-1) return;

		if(node.Value.left < clipRegionRight)
		{
			Vector3 pos = node.Value.transform.localPosition;//this node must be line end
			pos.x += node.Value.halfWidth;//cur item's half width, and next item's half width
			pos.y = startPos.y;
			for(int i=1; i<=itemPerCol; i++)
			{
				UIExGridItemCtrlBase item = GetItem();
				item.Init(node.Value.index + i, node.Value.xIndex + 1, i - 1, cellWith, cellHeight, this);
				item.SetData(userData);
				item.Cache();
				item.SetBoxCollider();

				if(i == 1)
					pos.x += item.halfWidth;//cur item's half width, and next item's half width

				item.transform.localPosition = pos;
				pos.y -= cellHeight;
				itemList.AddLast(item);
				//Debug.Log("add to right " + cachedItemList.Count);

				if(item.index == dataCount - 1)
					break;
			}
		}
	}
}
