
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;

[ExecuteInEditMode]
public class UIManager
{
	public static UIManager instance = new UIManager();
	
	//面板名和id映射
	private Dictionary<int, string> panelNames = null;

	private Dictionary<int, UIPanelBase> openedPanels = new Dictionary<int, UIPanelBase>();
	private Transform uiRoot;

	public void Init(Dictionary<int, string> idNames, Transform root)
	{
		uiRoot = root;
		panelNames = idNames;
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
	}

	public UIPanelBase OpanelPanel(Enum id, params object[] args)
	{
		int n = (int)(System.Object)id;
		var panel = OpanelPanel(n, args);
		if (panel == null)
		{
			Debug.LogErrorFormat("panel id = {0} / {1} load failed", n, id.ToString());
		}

		return panel;
	}

	public UIPanelBase OpanelPanel(int id, params object[] args)
	{
		if (panelNames == null)
		{
			Debug.LogError("UIManager has not inited");
			return null;
		}

		string name;
		if (!panelNames.TryGetValue(id, out name))
		{
			Debug.LogErrorFormat("panel id = {0} has no name", id);
			return null;
		}

		UIPanelBase panel;
		if (!openedPanels.TryGetValue(id, out panel))
		{
			panel = LoadPanelByName(name);
			if (panel != null)
			{
				panel.id = id;
				openedPanels.Add(id, panel);
			}
			else
			{
				return null;
			}
		}

		if (panel.isOpend)
		{
			Debug.LogErrorFormat("panel = {0} is opened", name);
		}
		else
		{
			panel.gameObject.SetActive(true);
			panel.isOpend = true;
			panel.OnOpen(args);
		}

		return panel;
	}

	public UIPanelBase LoadPanelByName(string panelName)
	{
		var panelPrefab = Resources.Load<GameObject>(panelName);
		if (panelPrefab == null)
		{
			Debug.LogError("PanelName = " + panelName + " is not found");
			return null;
		}

		var pb = panelPrefab.GetComponent<UIPanelBase>();
		if (pb == null)
		{
			Debug.LogErrorFormat("PanelName = {0}, has no UIPanelBase component", panelName);
			return null;
		}
		
		var panel = GameObject.Instantiate(pb, uiRoot);
		panel.transform.localPosition = Vector3.zero;
		panel.transform.localScale = Vector3.one;
		panel.transform.localRotation = Quaternion.identity;
		
		return panel;
	}

	public void ClosePanel(int id)
	{
		UIPanelBase panel = null;
		if (openedPanels.TryGetValue(id, out panel))
		{
			openedPanels.Remove(id);
			ClosePanel(panel);
		}
	}

	public void ClosePanel(UIPanelBase panel)
	{
		panel.gameObject.SetActive(false);
		panel.isOpend = false;
		panel.OnClose();
	}

	public void CloseAllPanel()
	{
		foreach(UIPanelBase panel in openedPanels.Values)
		{
			ClosePanel(panel);
		}
		openedPanels.Clear();
	}
}