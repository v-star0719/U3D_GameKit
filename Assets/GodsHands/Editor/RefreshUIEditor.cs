#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class RefreshUIEditor : EditorWindow
{
	[MenuItem("Tools/RefreshUI")]
	public static void OpenPatchWindow()
	{
		EditorWindow.GetWindow<RefreshUIEditor>();
	}

	void OnGUI()
	{
		Rect r = this.position;
		if(GUI.Button(new Rect(0, 0, r.width, r.height), "RefreshUI"))
		{
			GameObject go = GameObject.Find("UI Root");
			if(go != null)
			{
				go.SetActive(false);
				go.SetActive(true);
			}
			else
				Debug.LogError("UIRoot was not found");
		}
	}
}
#endif
