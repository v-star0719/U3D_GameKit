using UnityEngine;
using System.Collections;

public class TestExGrid : MonoBehaviour
{
	public UIExGrid exGrid;
	public int itemCount = 5;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	public bool testBtn;
	void Update ()
	{
		if(testBtn)
		{
			testBtn = false;
			exGrid.SetGrid(itemCount, null);
		}
	}
}
