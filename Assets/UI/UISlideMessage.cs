using UnityEngine;
using System.Collections;

//方向定义
public enum EmSlideDirection
{
	left,
	right,
	up,
	down
}

//提供鼠标/手指在屏幕上划过的消息。
public class UISlideMessage : MonoBehaviour
{
	public Vector3 slideRegionPos;   //滑动区域中心
	public float slideRegionWidth;   //滑动区域宽度
	public float slideRegionHeight;  //滑动区域高度
	public GameObject msgReceiver;   //滑屏消息接收对象
	public string msgHandle;         //滑屏消息处理函数名
	public UIPanel containPanel;

	private float dragStartTime = 0f;
	private Vector2 dragAmount;
	private bool startSlide = false;
	private Vector4 rect;//topleft = {x, y}, bottomRight = {z, w}
	private UIRoot uiroot;
	//private bool isVertEnable = false;	//当在滚动视图上滑动时，和滚动轴相同的滑动轴将禁用
	private bool isHorzEnable = false; //当在滚动视图上滑动时，和滚动轴相同的滑动轴将禁用
	private Vector3 dragStartPos;//屏幕坐标

	// Use this for initialization
	void Start ()
	{
		UICamera.onDragStart += OnScrollviewDragStarted;
		UICamera.onDrag += OnScrollviewDragged;
		UICamera.onDragEnd += OnScrollviewDragEnd;
		uiroot = UIRoot.list[0];//只有一个UIRoot

		rect.x = slideRegionPos.x - slideRegionWidth*0.5f;
		rect.y = slideRegionPos.x + slideRegionWidth*0.5f;
		rect.z = slideRegionPos.y - slideRegionHeight*0.5f;
		rect.w = slideRegionPos.y + slideRegionHeight*0.5f;
	}
	
	void OnDestroy()
	{
		UICamera.onDragStart -= OnScrollviewDragStarted;
		UICamera.onDrag -= OnScrollviewDragged;
		UICamera.onDragEnd -= OnScrollviewDragEnd;
	}

	void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(slideRegionPos, new Vector3(slideRegionWidth, slideRegionHeight));
	}

	public void OnScrollviewDragStarted(GameObject go)
	{
		UIDragScrollView ds = go.GetComponent<UIDragScrollView>();
		if(ds != null && ds.scrollView != null){
			//isVertEnable = ds.scrollView.movement != UIScrollView.Movement.Vertical;
			isHorzEnable = ds.scrollView.movement != UIScrollView.Movement.Horizontal;
		}
		else{
			//isVertEnable = true;
			isHorzEnable = true;
		}

		//if(!MUtils.IsActive(containPanel.transform)) return;
		if(!IsChild(containPanel.transform, go.transform)) return;
		if(startSlide) return;

		Vector3 mousePos = Input.mousePosition;
		
		float nguiScreenHeigth = uiroot.activeHeight;
		float nguiScreenWidth = uiroot.activeHeight * uiroot.manualWidth / uiroot.manualHeight;

		//需要把原点改为屏幕中心
		mousePos.x = (mousePos.x/Screen.width - 0.5f) * nguiScreenWidth;
		mousePos.y = (mousePos.y/Screen.height - 0.5f) * nguiScreenHeigth;

		//string debugText = string.Format("{0}, {1}", nguiScreenWidth, nguiScreenHeigth);

		//判断是否在区域内
		if((rect.x <= mousePos.x && mousePos.x <= rect.y) &&
		   (rect.z <= mousePos.y && mousePos.y <= rect.w) )
		{
			//debugText += ", In";
			startSlide = true;
			dragAmount = Vector2.zero;
			dragStartTime = Time.time;
			dragStartPos = Input.mousePosition;
		}
		//else
		//	debugText += ", Out";
		//Debug.Log(debugText + mousePos);
		//Debug.Log("start at: " + Input.mousePosition + ", gameObject = " + go.name);
	}
	
	public void OnScrollviewDragged(GameObject go, Vector2 delta)
	{
		/*
		if(!startSlide) return;
		if(!MUtils.IsActive(containPanel.transform)) return;
		if(!IsChild(containPanel.transform, go.transform)) return;

		dragAmount += delta;
		Debug.Log(dragAmount);
		*/
	}
	
	public void OnScrollviewDragEnd(GameObject go)
	{
		if(!startSlide) return;
		//if(!MUtils.IsActive(containPanel.transform)) return;
		if(!IsChild(containPanel.transform, go.transform)) return;
		startSlide = false;

		//目前只处理水平的，垂直的暂不处理
		if(!isHorzEnable) return;

		dragAmount = Input.mousePosition - dragStartPos;
		if(Time.time - dragStartTime <= 0.3f)
		{
			float f = Vector2.Dot(dragAmount.normalized, Vector2.right);
			if(Mathf.Abs(dragAmount.x) >= 50)
			{
				//角度判断
				if(f>0.9f)
				{
					msgReceiver.SendMessage(msgHandle, EmSlideDirection.right);
				}
				else if(f < -0.9f)
				{
					msgReceiver.SendMessage(msgHandle, EmSlideDirection.left);
				}
			}
		}
		//Debug.Log("end at: " + Input.mousePosition + ", Time = " + (Time.time - dragStartTime));
	}

	//判断parent和child是否是父子关系
	private bool IsChild(Transform parent, Transform child)
	{
		if(child == null)
			return false;
		else if(child == parent)
			return true;
		else
			return IsChild(parent, child.parent);
	}
}
