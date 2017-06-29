using UnityEngine;
using System.Collections;

public class UIExGridItemCtrlBase : MonoBehaviour
{
	[HideInInspector]
	public int index;//the index in linked list
	public int xIndex;//row index
	public int yIndex;//col index

	[HideInInspector]
	public float width;//if the item with is variable, overwrite it in SetData
	[HideInInspector]
	public float height;////if the item height is variable, overwrite it in SetData

	private UIExGrid exGrid;
	private BoxCollider myCollider;

	public float top { get { return transform.localPosition.y + height*0.5f; } }
	public float bottom { get { return transform.localPosition.y - height*0.5f; } }
	public float left { get { return transform.localPosition.x - width*0.5f; } }
	public float right { get { return transform.localPosition.x + width*0.5f; } }
	public float halfHeight;
	public float halfWidth;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDragStart()
	{
		if(exGrid != null)
			exGrid.OnDragStart();
	}

	void OnDrag(Vector2 delta)
	{
		if(exGrid != null)
			exGrid.OnDrag(delta);
	}

	void OnDragEnd()
	{
		if(exGrid != null)
			exGrid.OnDragEnd();
	}

	public void Init(int _index, int _xIndex, int _yIndex, float _width, float _height, UIExGrid _exGrid)
	{
		index = _index;
		xIndex = _xIndex;
		yIndex = _yIndex;
		width = _width;
		height = _height;
		exGrid = _exGrid;
	}

	public virtual void SetData(System.Object userData)
	{
		UILabel label = GetComponentInChildren<UILabel>();
		if(label != null)
			label.text = string.Format("{0} ({1}, {2})", index, xIndex, yIndex);
		if(exGrid.isVariableHeight && exGrid.direction == UIExGrid.EmDirection.Vertical)
		{
			height = Random.Range(exGrid.cellHeight, exGrid.cellHeight*5);
		}
		if(exGrid.isVariableWidth && exGrid.direction == UIExGrid.EmDirection.Horizontal)
		{
			width = Random.Range(exGrid.cellWith, exGrid.cellWith*5);
		}

		UISprite s = GetComponentInChildren<UISprite>();
		if(s != null)
		{
			s.width = (int)width;
			s.height = (int)height;
		}
	}

	public void Cache()
	{
		halfWidth = width * 0.5f;
		halfHeight = height * 0.5f;
	}

	public void SetBoxCollider()
	{
		if(myCollider == null)
			myCollider = GetComponent<BoxCollider>();
		if(myCollider != null)
			myCollider.size = new Vector3(width, height, 0);
	}
}
