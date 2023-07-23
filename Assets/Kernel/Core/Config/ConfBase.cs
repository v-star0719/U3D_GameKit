using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Xml.Serialization;
using Alche.FightCore;

[XmlInclude(typeof(SkillData))]
[XmlInclude(typeof(StageData))]
[XmlInclude(typeof(StageTriggerTemplate))]
[Serializable]
public class ConfBase
{
	public int id;
	public string name;

	public override string ToString()
	{
		return id + " " + name;
	}

	public virtual void OnTraversal()
	{

	}
}
