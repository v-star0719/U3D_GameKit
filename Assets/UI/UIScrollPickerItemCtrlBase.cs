using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScrollPickerItemCtrlBase : MonoBehaviour
{
	public int index = 0;
	private UIScrollPickerCtrl ctrl;

	public virtual void OnItemClicked()
	{
		ctrl.Select(index, true);
	}

	// Use this for initialization
	void Start ()
	{
		ctrl = GetComponentInParent<UIScrollPickerCtrl>();
	}

	public void OnDragStart()
	{
		if(ctrl != null) ctrl.OnDragStart();
	}

	public void OnDrag(Vector2 v)
	{
		if(ctrl != null) ctrl.OnDrag(v);
	}

	public void OnDragEnd()
	{
		if(ctrl != null) ctrl.OnDragEnd();
	}

	public void OnClick()
	{
		OnItemClicked();
	}
}
