#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorGetChildrenCountRecursive
{
	[MenuItem("Tools/GetChildrentCountRecrusive")]
	public static void GetChildrentCountRecrusive()
	{
		int childCount = GetChildCount(Selection.activeTransform);
		Debug.Log("child count is " + childCount);
	}

	private static int GetChildCount(Transform t)
	{
		if(t == null) return 0;
		int n = t.childCount;
		for(int i=0; i<t.childCount; i++)
		{
			n += GetChildCount(t.GetChild(i));
		}
		return n;
	}
}


#endif