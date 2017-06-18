using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GMAssistant))]
public class GMAssistantEditor : Editor
{
	public override void OnInspectorGUI()
	{
		if(!Application.isPlaying)
			GUILayout.Label("GMAssistant shuld work in game playing");

		//string text = "";

		//GMAssistant gma = (GMAssistant)target;
		//显示的字体是Arial 9，按钮两边的留空大概是4px。
	}
}

