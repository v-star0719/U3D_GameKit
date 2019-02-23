using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorUtils
{
	[MenuItem("Tqm/GetChildrentCountRecrusive")]
	public static void GetChildrentCountRecrusive()
	{
		int childCount = GetChildCount(Selection.activeTransform);
		Debug.Log("child count is " + childCount);
	}

	[MenuItem("Tqm/TestIterateFolderAssets")]
	public static void TestIterateFolderAssets()
	{
		if (Selection.activeObject == null)
		{
			Debug.LogError("please select an asset");
			return;
		}

		var path = AssetDatabase.GetAssetPath(Selection.activeObject);
		Debug.Log(path);

		var dir = Path.GetDirectoryName(path);
		Debug.Log(dir);

		var paths = Directory.GetFiles(dir);
		foreach (var s in paths)
		{
			if (s.EndsWith(".meta"))
			{
				continue;
			}
			Debug.Log(s);
		}
	}


	private static int GetChildCount(Transform t)
	{
		if(t == null) return 0;
		int n = t.childCount;
		for(int i = 0; i<t.childCount; i++)
		{
			n += GetChildCount(t.GetChild(i));
		}
		return n;
	}
}
