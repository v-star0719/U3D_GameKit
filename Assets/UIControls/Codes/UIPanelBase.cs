using UnityEngine;
using System.Collections;

/*
	public override void OnOpen(params object[] args)
	{
	}
	public override void OnClose()
	{
	}
*/

public class UIPanelBase : MonoBehaviour
{
	public int id;

	//[HideInInspector]
	public bool isOpend;

	public virtual void OnOpen(params object[] args) { }
	public virtual void OnClose() { }

	public void OnCloseBtnClicked()
	{
		UIManager.instance.ClosePanel(this);
	}
}
