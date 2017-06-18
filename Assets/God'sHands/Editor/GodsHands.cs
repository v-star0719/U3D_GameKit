using UnityEngine;
using UnityEditor;
using System.Collections;

public class GodsHands : EditorWindow
{
	[MenuItem("Tools/GodsHands")]
	public static void OpenGodsHands()
	{
		EditorWindow.GetWindow<GodsHands>();
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		//GM助手
		if(GUILayout.Button("GMAssistant", GUILayout.Width(150)))
		{
			GameObject godsHands = GetGodsHands();
			Transform gmAssistant = godsHands.transform.FindChild("GMAssistant");
			if(gmAssistant == null)
			{
				GameObject go = new GameObject("GMAssistant");
				go.transform.localPosition = Vector3.zero;
				go.transform.localScale = Vector3.one;
				go.transform.parent = godsHands.transform;
				go.AddComponent(typeof(GMAssistant));
				gmAssistant = go.transform;
			}
			Selection.activeObject = gmAssistant;
		}
		//属性修改器
		if(GUILayout.Button("Trainer", GUILayout.Width(120)))
		{
			GameObject godsHands = GetGodsHands();
			Transform trainer = godsHands.transform.FindChild("FBLX Trainer");
			if(trainer == null)
			{
				GameObject go = new GameObject("FBLX Trainer");
				go.transform.localPosition = Vector3.zero;
				go.transform.localScale = Vector3.one;
				go.transform.parent = godsHands.transform;
				go.AddComponent(typeof(Trainer));
				trainer = go.transform;
			}
			Selection.activeObject = trainer;
		}
		GUILayout.EndHorizontal();

		//加速键
		GUI.enabled = Application.isPlaying;
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("x1")) Time.timeScale = 1;
		if(GUILayout.Button("x2")) Time.timeScale = 2;
		if(GUILayout.Button("x3")) Time.timeScale = 3;
		if(GUILayout.Button("x4")) Time.timeScale = 4;
		if(GUILayout.Button("x8")) Time.timeScale = 8;
		if(GUILayout.Button("x16"))Time.timeScale = 16;
		if(GUILayout.Button("x.5"))Time.timeScale = 0.5f;
		if(GUILayout.Button("x0")) Time.timeScale = 0;

		GUILayout.Label(string.Format("x{0:F0}", Time.timeScale));
		GUILayout.EndHorizontal();
		GUI.enabled = true;

		//帧数
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("10fp", GUILayout.Width(50)) && Application.isPlaying)
            Application.targetFrameRate = 10;
		if(GUILayout.Button("30fp", GUILayout.Width(50)) && Application.isPlaying)
			Application.targetFrameRate = 30;
		if(GUILayout.Button("60fp", GUILayout.Width(50)) && Application.isPlaying)
			Application.targetFrameRate = 60;
		if(GUILayout.Button("~fp", GUILayout.Width(50)) && Application.isPlaying)
			Application.targetFrameRate = -1;
		GUILayout.Label(string.Format("{0}fp", Application.targetFrameRate));
		GUILayout.EndHorizontal();
	}

	public GameObject GetGodsHands()
	{
		GameObject godsHands = GameObject.Find("[-]GodsHands");
		if(godsHands == null){
			godsHands = new GameObject("[-]GodsHands");
			godsHands.transform.localPosition = Vector3.one;
			godsHands.transform.localScale = Vector3.zero;
		}
		return godsHands;
	}
}
