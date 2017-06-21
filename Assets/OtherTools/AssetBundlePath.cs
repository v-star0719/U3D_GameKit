using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 0162
//public class PlatformManager 
//{	
//	public const int PLATFORM_LAN = 1;
//	public const int PLATFORM_WAN = 2;
//	public const int PLATFORM_ThirdPart = 4;
//	
//	private static PlatformManager instance = null;		
//	public static PlatformManager GetInstance()
//	{
//		if(instance == null)
//		{
//			instance = new PlatformManager();
//		}
//		return instance;
//	}
//	public static bool IsEditor()
//	{
//		return (Application.platform == RuntimePlatform.WindowsEditor ||
//		        Application.platform == RuntimePlatform.OSXEditor);
//	}
//
//}


public class AssetBundlePath : MonoBehaviour
{
	public static string LEVEL_ASSETBUNDLE_PREFIX = "Level_";
    public const string UISCENES_NAME = "Streamed-UIScenes.unity3d";
	public const string MAIN_CITY_UI = "MainCityUI.unity3d";
	public const string BATTLE_UI = "BattleUI.unity3d";
	public const string STATIC_DATA_CENTER = "StaticDataCenter.unity3d";

	public static bool isLoadUIFromAssetBundle = false;//如果是true，则从AssetBundle导入UI，否则直接从Resurces文件夹读

    public static string texturePath;
    public static string localDataPath;
    public static string assetBundlePath;
    public static string configPath;

	public static bool IsEditor()
	{
		return (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor);
	}

    public static string GetStreamingAssetsPath()
    {
        string path = "";
        if (IsEditor())
        {
            path = "file://" + Application.streamingAssetsPath + "/";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = "file://" + Application.streamingAssetsPath + "/";
			path = GameUtilities.ConvertNativeUrlToWindowsPlatform(path);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.streamingAssetsPath + "/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = "file://" + Application.streamingAssetsPath + "/";
        }
        return path;
    }

    public static string GetStreamingAssetbuddleResourcePath()
    {
        string path = "";

#if UNITY_EDITOR
        if(Application.isEditor)
        { 
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                path = GetStreamingAssetsPath() + "Android/";
            else if(UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                path = GetStreamingAssetsPath() + "IOS/";
			else
				path = GetStreamingAssetsPath () + "Windows/";


        }
        else
#endif
        if (Application.platform == RuntimePlatform.Android             
        #if UNITY_EDITOR
                || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
        #endif
                )
		{
			path = GetStreamingAssetsPath() + "Android/";
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer ||Application.platform == RuntimePlatform.OSXEditor )
		{
			path = GetStreamingAssetsPath() + "IOS/";
		} 
		
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			path = GetStreamingAssetsPath()+"Windows/";
		}
        return path;
    }

    public static string GetLanguageAssetPath()
    {
        assetBundlePath = GetStreamingAssetsPath() + "Language/";
       // if (ResourceManager.GetResPath("SimplifiedChinese.txt") == AssetSheetOneLine.LoadArea.Persistent)
        {
            //assetBundlePath = GetOutPackagePath();
        }
        return assetBundlePath;
    }

	public static string GetSettingAssetPath()
	{
		return GetStreamingAssetsPath()+"Setting.txt";
	}


    public static string GetOutPackagePath(bool withFileHead = true)
    {
        string path = "";
        if(Random.Range(0, 1) ==1)
        {
            if (IsEditor())
            {
                path = "file://" + Application.dataPath + "/../DownloadData/";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = "file://" + Application.persistentDataPath + "/";
				path = GameUtilities.ConvertNativeUrlToWindowsPlatform(path);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                path = withFileHead ? "file://" + Application.persistentDataPath + "/" : Application.persistentDataPath + "/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = "file://" + Application.persistentDataPath + "/";
            }
        }
        else
        {
            path = GetStreamingAssetbuddleResourcePath();
        }
        return path;
    }



    public static string GetSettingConfigPath()
    {
        return GetStreamingAssetsPath() + "Setting/";
    }

}
