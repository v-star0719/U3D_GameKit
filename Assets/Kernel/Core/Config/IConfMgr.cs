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

public interface IConfMgr<T> where T : ConfBase
{
	void Load();
	void Save();
	void Add(T t);
	void Remove(int id);
	bool Contains(int id);
	T Get(int id);
	T Create(int id);
	List<T> GetConfs();
}
