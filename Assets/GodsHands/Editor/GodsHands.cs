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
		if(GUILayout.Button("GMAssistant"))
		{
			OpenInspectorFunction<GMAssistant>("GMAssistant");
		}
		//属性修改器
		if(GUILayout.Button("Trainer"))
		{
			OpenInspectorFunction<Trainer>("Trainer");
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

		GUILayout.BeginHorizontal();
		//重进游戏
		GUI.enabled = Application.isPlaying;
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("重进游戏"))
		{
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;

		//引导工具
		GUILayout.BeginHorizontal();
		//快速登录
		bool isOpened = IsUIFunctionOpened("FastLogin", true);
		if(GUILayout.Button(isOpened ? "关闭FastLogin" : "打开FastLogin"))
		{
			if(isOpened) CloseUIFunction("FastLogin");
			else OpenUIFunction("FastLogin", "Assets/GodsHands/FastLogin/FastLogin.prefab");
		}
		GUILayout.EndHorizontal();
	}

	GameObject GetGodsHands()
	{
		GameObject godsHands = GameObject.Find("#GodsHands");
		if(godsHands == null)
		{
			godsHands = new GameObject("#GodsHands");
			godsHands.transform.localPosition = Vector3.one;
			godsHands.transform.localScale = Vector3.zero;
		}
		return godsHands;
	}

	void OpenInspectorFunction<T>(string name) where T : MonoBehaviour
	{
		GameObject godsHands = GetGodsHands();
		Transform funcTrans = godsHands.transform.Find(name);
		if(funcTrans == null)
		{
			GameObject go = new GameObject(name);
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			go.transform.parent = godsHands.transform;
			go.AddComponent<T>();
			funcTrans = go.transform;
		}
		Selection.activeObject = funcTrans;
	}

	//prefabPath是Assets/XX/XX的
	void OpenUIFunction(string functionName, string prefabPath)
	{
		if(UIRoot.list == null || UIRoot.list.Count == 0)
			return;

		Transform trans = GetUIFunction(functionName);
		if(trans != null)
		{
			//exist
			Selection.activeObject = trans;
			return;
		}

		//Create new
		GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
		if(go == null)
		{
			Debug.LogError("prefab is not found at: " + prefabPath);
			return;
		}

		go = GameObject.Instantiate<GameObject>(go);
		go.transform.parent = UIRoot.list[0].transform;
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
		go.name = "#GodsHands." + functionName;
		go.SetActive(true);
		Selection.activeObject = go;
	}

	void CloseInspectorFunction(string functionName)
	{
		GameObject godsHands = GetGodsHands();
		Transform trans = godsHands.transform.Find(functionName);
		if(trans != null)
			DestroyImmediate(trans.gameObject);
	}

	void CloseUIFunction(string functionName)
	{
		Transform trans = GetUIFunction(functionName);
		if(trans != null)
			DestroyImmediate(trans.gameObject);
	}

	bool IsUIFunctionOpened(string functionName, bool isUI)
	{
		return GetUIFunction(functionName) != null;
	}

	bool IsInspectorFunctionOpened(string functionName, bool isUI)
	{
		GameObject godsHands = GetGodsHands();
		return godsHands.transform.Find(functionName);
	}

	Transform GetUIFunction(string functionName)
	{
		if(UIRoot.list == null || UIRoot.list.Count == 0)
			return null;

		string hierachyName = "#GodsHands." + functionName;
		Transform root = UIRoot.list[0].transform;
		for(int i=0; i<root.childCount; i++)
		{
			Transform trans = root.GetChild(i);
			if(trans.name == hierachyName)
				return trans;
		}
		return null;
	}
}
