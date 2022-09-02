using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System;
#endif

public class FastLoginPanel : MonoBehaviour
{
#if UNITY_EDITOR
	private const string dataFilePath = "Assets/DevelopTools/FastLogin/AccountFile.txt";

	public GameObject root;
	public UIGrid grid;
	public List<FastLoginItemCtrl> accountItemCtrlList = new List<FastLoginItemCtrl>();
	public UIInput registerUserNameInput;
	public UIInput registerUserPWInput;
	public UIInput loginUserNameInput;
	public UIInput loginUserPWInput;
	public UILabel saveToFileIndicator;

	private List<string> accountNameList = new List<string>();
	private List<string> accountPWList = new List<string>();
	private bool isSaveAccountToFile = true;

	void Awake()
	{
		root.SetActive(true);
	}

	void OnDestroy()
	{
	}

	// Use this for initialization
	void Start ()
	{
	}

	void OnEnable()
	{
		OpenFile();
		saveToFileIndicator.text = isSaveAccountToFile ? "将保存到文件（点击更改）" : "不保存到文件（点击更改）";
	}

	// Update is called once per frame
	void Update ()
	{
		//hide fast login if it is not login
	}

	void OpenFile()
	{
		FileStream file = new FileStream(dataFilePath, FileMode.OpenOrCreate);
		
		byte[] byteData = new byte[file.Length];
		file.Read(byteData, 0, byteData.Length);

		string[] accountArray = null;
		if(file.Length > 3)
		{
			char[] charData = new char[byteData.Length-3];
			for(int i=3; i<byteData.Length; i++)
				charData[i-3] = (char)byteData[i];

			string fileText = new string(charData);
			fileText = fileText.Replace("\r", "");
			accountArray = fileText.Split('\n');
		}
		else
			accountArray = new string[0];

		foreach(string str in accountArray)
		{
			string[] accountNPW = str.Split(' ');
			if(accountNPW.Length != 2) continue;

			accountNameList.Add(accountNPW[0]);
			accountPWList.Add(accountNPW[1]);

			//Debug.Log("Add " + accountNPW[0] + " " + accountNPW[1]);
		}

		file.Close();

		ShowAccountItems();
	}

	void ShowAccountItems()
	{
		FastLoginItemCtrl accountItem;

		for(int i=0; i<accountNameList.Count; i++)
		{
			if(i >= accountItemCtrlList.Count)
			{
				accountItem = Instantiate<FastLoginItemCtrl>(accountItemCtrlList[0]);
				accountItem.transform.parent = grid.transform;
				accountItem.transform.localScale = Vector3.one;
				accountItem.name = "account " + (i+1);
				accountItemCtrlList.Add(accountItem);
			}
			else
				accountItem = accountItemCtrlList[i];

			accountItem.gameObject.SetActive(true);
			accountItem.SetData(i, accountNameList[i]);
		}

		for(int i=accountNameList.Count; i<accountItemCtrlList.Count; i++)
		{
			accountItemCtrlList[i].gameObject.SetActive(false);
		}

		grid.Reposition();
	}

	void AddNewAccount(string accountName, string accountPW)
	{
		int find = 0;
		for(; find<accountNameList.Count; find++){
			if(accountNameList[find] == accountName)
				break;
		}
		if(find < accountNameList.Count)
		{
			accountNameList[find] = accountName;
			accountPWList[find] = accountPW;
		}
		else
		{
			accountNameList.Add(accountName);
			accountPWList.Add(accountPW);
		}
		ShowAccountItems();

		//重写文件
		string text = "";
		for(int i=0; i<accountNameList.Count; i++)
		{
			text += string.Format("{0} {1}\r\n", accountNameList[i], accountPWList[i]);
		}
		char[] charData = text.ToCharArray();
		byte[] byteData = new byte[charData.Length + 3];
		byteData[0] = 0xEF;
		byteData[1] = 0xBB;
		byteData[2] = 0xBF;
		for(int i=0; i<charData.Length; i++)
			byteData[i+3] = (byte)charData[i];

		FileStream file = new FileStream(dataFilePath, FileMode.Create);
		file.Write(byteData, 0, byteData.Length);
		file.Close();
	}

	public void OnAccountItemClicked(FastLoginItemCtrl ctrl)
	{
		//string accountName = accountNameList[ctrl.index];
		//string accountPW = accountPWList[ctrl.index];
		//Add account code here
	}

	public void OnRegisterOkBtnClicked()
	{
		string account = registerUserNameInput.value;
		string password = registerUserPWInput.value;
		if(!IsAccountLegal(account, password))
			return;
		// add signin code here
		if(isSaveAccountToFile)
			AddNewAccount(account, password);
	}
	
	public void OnLoginOkBtnClicked()
	{
		string account = loginUserNameInput.value;
		string password = loginUserPWInput.value;
		if(!IsAccountLegal(account, password))
			return;

		//LevelManagerLogin.GetInstance().AccountLoginCenterServer(account, password);//不破坏原有结构，使用反射获取
		//add login code here
		if(isSaveAccountToFile)
			AddNewAccount(account, password);
	}

	public void OnSaveToFileIndicatorClicked()
	{
		isSaveAccountToFile = !isSaveAccountToFile;
		saveToFileIndicator.text = isSaveAccountToFile ? "将保存到文件（点击更改）" : "不保存到文件（点击更改）";
	}

	public void OnOpenAccountFileClicked()
	{
		string pathName = Application.dataPath;
		pathName = pathName.Replace("Assets", "") + dataFilePath;
		System.Diagnostics.Process.Start(pathName);
	}

	bool IsAccountLegal(string account, string password)
	{
		//check the accoutName and password, if is legal return true, otherwise return false.
		return true;
	}

#endif

}
