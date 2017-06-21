using UnityEngine;
using System.Collections;

public class GameViewScreenSize : MonoBehaviour
{
	public static float width;
	public static float height;


    void Awake()
    {
        if(GetComponent<UIPanel>() != null)
        {
            Destroy(GetComponent<UIPanel>());
        }
    }
	#if UNITY_EDITOR

	//private int gameViewScreenHeight;
	//private int gameViewScreenWidth;

	void Update ()
	{
		GetScreenWidthAndHeightFromEditorGameViewViaReflection();
	}

	void GetScreenWidthAndHeightFromEditorGameViewViaReflection()
	{
		//Taking game view using the method shown below	
		var gameView = GetMainGameView();
		var prop = gameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var gvsize = prop.GetValue(gameView, new object[0]{});
		var gvSizeType = gvsize.GetType();

		//I have 2 instance variable which this function sets:
		height = (float)((int)gvSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0]{}));
		width = (float)((int)gvSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0]{}));
	}

	UnityEditor.EditorWindow GetMainGameView()
	{
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null,null);
		return (UnityEditor.EditorWindow)Res;
	}
	#endif
}
