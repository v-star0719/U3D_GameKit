using UnityEngine;
using System.Collections;

public class UIGridExItemCtrlBase : MonoBehaviour
{
	[HideInInspector]
	public int index;//总的在链表里的索引值
	public int xIndex;//行索引
	public int yIndex;//列索引

	[HideInInspector]
	public UIGridEx exGrid;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrag(Vector2 delta)
	{
		if(exGrid != null)
			exGrid.OnDrag(delta);
	}

	public virtual void SetData(System.Object userData)
	{
		UILabel label = GetComponentInChildren<UILabel>();
		if(label != null)
			label.text = index.ToString();
	}
}
