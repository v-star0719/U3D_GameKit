using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
//(1)目前没有做循环滚动，循环很麻烦
//(2)水平形式：从左到右依次对应数组的01234...索引
//   竖直形式：从上到下依次对应数组的01234...索引
public delegate void OnScrollPickerSelectChanged(UIScrollPickerItemCtrlBase item, int itemIndex);
[ExecuteInEditMode]
public class UIScrollPickerCtrl : MonoBehaviour
{
	public enum EmDirection
	{
		Horizontal,
		Vertical,
	}

	public const float SCALE_POS_FACTOR = 0.5f;//当到原点的距离小于SCALE_POS_FACTOR*ItemWith时进行scale变化，并认为该item被选中了

	public UIPanel clipPanel;
	public EmDirection direction;
	public float pickedItemScale = 1f;
	
	public int itemWidth;
	public int itemHeight;
	public int itemGap;

	public UIScrollPickerItemCtrlBase[] itemArray;

	private float curOffset;
	private int curSelectIndex = 0;
	private OnScrollPickerSelectChanged selectChangeCallback;

	public float springDuration = 1f;
	private bool isDragging = false;
	private Vector3 springStartPos;//终点就是(0,0,)，以把当前item对齐到原点，做弹簧效果
	private float springTimer = 0f;
	private bool isSpring = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public bool testBtn;
	public int testCount;
	public int testCurIndex;
	public UIScrollPickerItemCtrlBase testItemPrefab;
	void Update ()
	{
		if(testBtn)
		{
			Set(testCurIndex, null);
			testBtn = false;
		}

		//if(!Application.isPlaying)
		//{
		//	itemArray = GetComponentsInChildren<UIScrollPickerItemCtrlBase>();
		//	MUtils.SortGoByNumberInName<UIScrollPickerItemCtrlBase>(itemArray);
		//	for(int i=0; i<itemArray.Length; i++)
		//		itemArray[i].index = i;
		//	return;
		//}

		Update_Spring();
	}

	void Update_Spring()
	{
		if(isDragging) return;
		if(!isSpring) return;

		if(Time.deltaTime > 0.05f)
			springTimer += 0.05f;
		else
			springTimer += Time.deltaTime;
		
		float f = springTimer / springDuration;
		Vector3 pos = Vector3.Lerp(springStartPos, Vector3.zero, f);
		itemArray[curSelectIndex].transform.localPosition = pos;
		//其他的项，从selected item向两边对齐
		for(int i=0; i<itemArray.Length; i++)
		{
			UIScrollPickerItemCtrlBase item = itemArray[i];
			Vector3 v = pos;
			if(direction == EmDirection.Horizontal)
				v.x += (itemWidth + itemGap) * (i - curSelectIndex);
			else
				v.y -= (itemHeight + itemGap) * (i - curSelectIndex);

			item.transform.localPosition = v;

			ScaleItem(item);
		}

		if(springTimer >= springDuration)
			isSpring = false;
	}

	public void Set(int selectedIndex, OnScrollPickerSelectChanged selectChangeCallback)
	{
		curSelectIndex = selectedIndex;
		this.selectChangeCallback = selectChangeCallback;
		Reposition();
	}

	public void Reposition()
	{
		int itemCount = itemArray.Length;

		Vector3 startPos = Vector3.zero;
		if(direction == EmDirection.Horizontal)
			startPos.x = -(itemWidth + itemGap) * (curSelectIndex);//左边有几个，起点就得偏移多少
		else
			startPos.y = (itemHeight + itemGap) * (curSelectIndex);//左边有几个，起点就得偏移多少

		for(int i=0; i< itemCount; i++)
		{
			UIScrollPickerItemCtrlBase item = itemArray[i];
			Vector3 p = Vector3.zero;

			if(direction == EmDirection.Horizontal)
				p.x = i* (itemWidth + itemGap);
			else
				p.y = -i* (itemHeight + itemGap);

			item.transform.localPosition = startPos + p;

			ScaleItem(item);
		}
	}

	public void Select(int select, bool moveTo)
	{
		//MoveTo处理为Spring
		curSelectIndex = select;
		if(!moveTo)
		{
			Reposition();
		}
		else
		{
			StartSpring(itemArray[curSelectIndex].transform.localPosition);
		}
	}

	public void OnDragStart()
	{
		isDragging = true;
		isSpring = false;
	}

	public void OnDrag(Vector2 v)
	{
		Move(v);
	}

	public void OnDragEnd()
	{
		isDragging = false;
		StartSpring(itemArray[curSelectIndex].transform.localPosition);
	}

	private void StartSpring(Vector3 startPos)
	{
		springStartPos = startPos;
		springTimer = 0f;
		isSpring = true;
	}

	private void Move(Vector3 delta)
	{
		if(direction == EmDirection.Horizontal)
			delta.y = 0;
		else
			delta.x = 0;

		//begin 如果第一个或者最后一个会拉过头，则不拉
		//第一个
		Vector3 pos = itemArray[0].transform.localPosition;
		Vector3 newPos = pos + delta;
		if(newPos.x > 0) delta.x = -pos.x;
		if(newPos.y < 0) delta.y = -pos.y;
		//最后那个
		pos = itemArray[itemArray.Length-1].transform.localPosition;
		newPos = pos + delta;
		if(newPos.x < 0) delta.x = -pos.x;
		if(newPos.y > 0) delta.y = -pos.y;
		//end

		for(int i=0; i<itemArray.Length; i++)
		{
			UIScrollPickerItemCtrlBase item = itemArray[i];
			item.transform.localPosition += new Vector3(delta.x, delta.y, 0);

			ScaleItem(item);

			//如果item到原点的距离小于一定值，认为被选中了
			float threshold = 0;
			float dist = 0;
			if(direction == EmDirection.Horizontal)
			{
				threshold = itemWidth * SCALE_POS_FACTOR;
				dist = item.transform.localPosition.x;
			}
			else
			{
				threshold = itemHeight * SCALE_POS_FACTOR;
				dist = item.transform.localPosition.y;
			}
			//
			int s = curSelectIndex;
			if(Mathf.Abs(dist) < threshold)
				s = i;
			
			if(s != curSelectIndex)
			{
				curSelectIndex = s;
				if(selectChangeCallback != null)
					selectChangeCallback(item, curSelectIndex);
				Debug.Log("seleced item changed: Cur = " + curSelectIndex);
			}
		}
	}

	private void ScaleItem(UIScrollPickerItemCtrlBase item)
	{
		//当处于(0~半个宽度/高度)的范围内时，会被放大
		float dist = 0;
		float itemWH = 0;
		if(direction == EmDirection.Horizontal)
		{
			dist = item.transform.localPosition.x;
			itemWH = itemWidth * SCALE_POS_FACTOR;
		}
		else
		{
			dist = item.transform.localPosition.y;
			itemWH = itemHeight * SCALE_POS_FACTOR;
		}
		//
		float scale = 1;
		if(dist < 0) dist = -dist;
		if(dist < itemWH)
		{
			scale = Mathf.Lerp(pickedItemScale, 1, dist/itemWH);
		}
		item.transform.localScale = new Vector3(scale, scale, scale);
	}
}
