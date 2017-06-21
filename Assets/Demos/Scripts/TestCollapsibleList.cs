using UnityEngine;
using System.Collections;

public class TestCollapsibleList : MonoBehaviour
{
	public UICollapsibleListCtrl listCtrl;
	public int itemCount = 3;
	// Use this for initialization
	void Start () {
		listCtrl.Set(itemCount, null, 1000);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
