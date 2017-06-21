using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//主要是从AssetBoundle载入的二进制形式的文本资源
public class BinaryTextAsset
{
	public string name;//文件名
	public byte[] text;//内容
}

//基于NGUI(3.7.6)的Localization，请勿直接调用NGUI Localization
//约定：让Localization 一次只记录一种语言，即不使用它的多语言功能
//      不要直接通过Localization.language Get/Set语言，这会导致它自动进行Load，直接通过Set接口进行
public class StormLocalization : MonoBehaviour
{
	public const string GAME_UPDATE_LOCALIZE_FILE_SUFFIX = "_GameUpdate";//游戏更新过程中多语言文件名的后缀，比如SimpleChinise_GameLoading
	public static string curLanguage;
	public static BinaryTextAsset loadedBinaryTextAsset = null;//加载的当前多语言资源，初始化后会释放
	public string[] languagesNames;

	public static StormLocalization instance = null;
	private static bool isInited = false;
	private static Dictionary<string, string> gameUpdateLocalizeDict;//只在第一次进游戏时加载一次

	public delegate void OnLanguageLoadingFinished();

	void Awake ()
	{
		instance = this;
	}
	public static string Get(string key)
	{
		if (!isInited)
		{
			if (Application.isPlaying)
				Debug.LogError("StormLocalization has not been initilized");
			return key;
		}
		return Localization.Get(key);
	}

	//载入多语言文本后，调用这个进行初始化
	public static void Init()
	{
		if(loadedBinaryTextAsset == null)
		{
			Debug.LogError("localized text is not loaded");
			return;
		}

		isInited = true;
		Localization.Set(loadedBinaryTextAsset.name, loadedBinaryTextAsset.text);
		loadedBinaryTextAsset = null;
	}
	/*
	public void Initilize()
	{
		Localization.mLanguage="";
		currentLanguage = PlayerPrefs.GetString("Language", startingLanguage);
		if (string.IsNullOrEmpty(mLanguage) && (languages != null && languages.Count > 0))
		{
			currentLanguage = languages[0].name;
		}
	}
	*/

	//切换到其他语言 !!!!(未实际测试)!!!!
	public static bool SetLanguage(string newLanguage)
	{
		if(newLanguage == curLanguage) return true;

		for(int i=0; i<instance.languagesNames.Length; i++)
		{
			if(newLanguage == instance.languagesNames[i])
			{
				curLanguage = newLanguage;
				instance.LoadLanguageAsset(instance.OnLanguageAssetLoad);
				return true;
			}
		}

		Debug.LogError(string.Format("The language {0} is not exited", newLanguage));
		return false;
	}

	public void OnLanguageAssetLoad()
	{
		isInited = false;
		Init();
	}

	public void LoadLanguageAsset(OnLanguageLoadingFinished calBack)
	{
		if(languagesNames.Length == 0){
			Debug.LogError("no language is set");
			return;
		}
		if(string.IsNullOrEmpty(curLanguage))
			curLanguage = languagesNames[0];

		loadedBinaryTextAsset = null;
		StartCoroutine(LoadLanguage(curLanguage, calBack));
	}
	public IEnumerator LoadLanguage(string languageFileName, OnLanguageLoadingFinished callBack)
	{
		string fileName = languageFileName + ".txt";
		WWW www = new WWW(GameUtilities.ConvertNativeUrlToWindowsPlatform(AssetBundlePath.GetLanguageAssetPath() + fileName));
		yield return www;

		if (string.IsNullOrEmpty(www.error))
		{
			StormLocalization.loadedBinaryTextAsset = new BinaryTextAsset();
			StormLocalization.loadedBinaryTextAsset.name = languageFileName;
			StormLocalization.loadedBinaryTextAsset.text = www.bytes; ;
		}
		else
		{
			Debug.LogError("Can't load file " + fileName + " Error: " + www.error);
		}

		if (callBack != null)
			callBack();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//----begin
	///刚进进入游戏，检查更新时显示的多语言
	///GULK = Game Update Language Key
	public static string GetGULK(string key)
	{
		if (gameUpdateLocalizeDict == null){
			Debug.LogError("No localization data present");
			return key;
		}
		if(string.IsNullOrEmpty(key)){
			Debug.LogError("key is null or empty");
			return key;
		}

		string v;
		if(gameUpdateLocalizeDict.TryGetValue(key, out v))
		   return v;
		Debug.LogError(key + "is not found in dict");
		return key;
	}
	public string GetGameUpdateLanguageFileName()
	{
		if (languagesNames.Length == 0)
		{
			return null;
		}
		if (string.IsNullOrEmpty(curLanguage))
			curLanguage = languagesNames[0];
		return curLanguage + GAME_UPDATE_LOCALIZE_FILE_SUFFIX;
	}
	public static void InitGameUpdateLanguage(byte[] langAssets)
	{
		ByteReader rb = new ByteReader(langAssets);
		gameUpdateLocalizeDict = rb.ReadDictionary();
	}
	//----end
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
