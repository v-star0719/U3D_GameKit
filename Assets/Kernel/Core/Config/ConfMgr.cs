using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using System.Xml.Serialization;
using Kernel;
using Kernel.Core;

public class ConfMgr<TMgr, TData> : IConfMgr<TData> where TData : ConfBase where TMgr : new()
{
	protected static TMgr instance;
	public static TMgr Instance
	{
		get
		{
			var mgr = instance;
			if (mgr != null)
			{
				return mgr;
			}
			return (instance = new TMgr());
		}
	}

	public List<TData> List { get; private set; }
	public Dictionary<int, TData> Dict { get; private set; }
	public string FileName { get; protected set; }

	public ConfMgr(string fileName)
	{
		FileName = fileName;
		List = new List<TData>();
		Dict = new Dictionary<int, TData>();
		Load();
	}

	public void Load()
	{
		if(File.Exists(GetConfigFilePath()))
		{
			FileStream fs = File.Open(GetConfigFilePath(), FileMode.Open);
			using(StreamReader sr = new StreamReader(fs, Encoding.UTF8))
			{
				XmlSerializer xz = new XmlSerializer(List.GetType());
				List = (List<TData>)xz.Deserialize(sr);
			}

			foreach(var conf in List)
			{
				if (Dict.ContainsKey(conf.id))
				{
                    LoggerX.Error($"{FileName} 表id重复：{conf.id}");
				}
				else
				{
					Dict.Add(conf.id, conf);
				}
			}

			return;
		}
		else
		{
			Debug.Log("File is not exist: " + GetConfigFilePath());
		}
	}

	private void IterateFilds(object data)
	{
		var t = data.GetType();
		if(t.IsClass)
		{
			return;
		}

		if (t.IsGenericType && t.IsClass)
		{

		}

		var methos = t.GetMethods(BindingFlags.CreateInstance | BindingFlags.Public);
		foreach(var methodInfo in methos)
		{
			if(methodInfo.GetCustomAttribute<OnDeserializedAttribute>() != null)
			{
				methodInfo.Invoke(data, null);
			}
		}

		var fields = t.GetFields(BindingFlags.CreateInstance | BindingFlags.Public);
		foreach(var field in fields)
		{
			var fv = field.GetValue(data);
			var ft = field.FieldType;
			if(ft.IsClass)
			{
				IterateFilds(fv);
			}
		}
	}

	public void Save()
	{
		using(StringWriter sw = new StringWriter())
		{
			XmlSerializer xz = new XmlSerializer(List.GetType());
			xz.Serialize(sw, List);
			File.WriteAllText(GetConfigFilePath(), sw.ToString());
		}
	}

	private string GetConfigFilePath()
	{
#if UNITY_EDITOR
		return Application.dataPath + $"/Resources/Configs/{FileName}.xml";
#else
		return Application.persistentDataPath + configFileName;
#endif
	}

	public void Add(TData t)
	{
		List.Add(t);
		List.Sort((a, b) => a.id .CompareTo(b.id));
		Dict.Add(t.id, t);
	}

	public void Remove(int id)
	{
		TData s;
		if(Dict.TryGetValue(id, out s))
		{
			Dict.Remove(id);
			List.Remove(s);
		}
	}

	public bool Contains(int id)
	{
		return Dict.ContainsKey(id);
	}

	public TData Get(int id)
	{
		TData rt;
		if (Dict.TryGetValue(id, out rt))
		{
			return rt;
		}
        LoggerX.Error(GetConfigFilePath() + " 配置找不到: " + id);
		return null;
	}

	public List<TData> GetConfs()
	{
		return List;
	}

	public virtual TData Create(int id)
	{
		var rt = Activator.CreateInstance<TData>();
		rt.id = id;
		return rt;
	}
}
