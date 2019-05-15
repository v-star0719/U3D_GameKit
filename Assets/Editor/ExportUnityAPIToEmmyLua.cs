#if UNITY_EDITOR

using System;
using System.IO;
using System.Reflection;
using System.Text;
using SLua;
using UnityEditor;
using UnityEngine;

public class ExportUnityAPIToEmmyLua : MonoBehaviour {

	[MenuItem("Tqm/导出EmmyLuaAPI", false, 14)]
	static void Gen()
	{
		//string path = Application.dataPath + "/EmmyLuaUhityAPI/";
		string path = "D:/Workspace/EmmyLuaUhityAPI/";
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
		Directory.CreateDirectory(path);

		//unity以前的版本直接用LoadAssembly就可以了，新版不行，直接get。
		ExportAssembly(typeof(GameObject).Assembly, path);
		ExportAssembly(typeof(UILabel).Assembly, path);
		//ExportAssembly(typeof(AB).Assembly, path);
	}

	public static void ExportAssembly(Assembly asm, string path)
	{
		Type[] types = asm.GetTypes();
		foreach (Type t in types)
		{
			ExportType(t, path);
		}
	}

	public static void ExportType(Type t, string path)
	{
		if(!CanExport(t))
		{
			return;
		}

		StringBuilder sb = new StringBuilder("");

		if(t.BaseType != null)
		{
			sb.AppendFormat("---@class {0} : {1}\n", t.Name, t.BaseType.Name);
		}
		else
		{
			sb.AppendFormat("---@class {0}\n", t.Name);
		}
		ExportFields(t, sb);
		ExportProperties(t, sb);
		sb.AppendFormat("local {0} = {{}}\n", t.Name);
		ExportMethods(t, sb);

		if (string.IsNullOrEmpty(t.Namespace))
		{
			File.WriteAllText(path + "/" + t.Name + ".lua", sb.ToString(), Encoding.UTF8);
		}
		else
		{
			File.WriteAllText(path + "/" + t.Namespace + "." + t.Name + ".lua", sb.ToString(), Encoding.UTF8);
		}

		Debug.Log(t.Name);
	}

	private static void ExportFields(Type t, StringBuilder sb)
	{
		var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		foreach (var f in fields)
		{
		    //忽略已弃用的
		    if(t.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
		    {
		        continue;
		    }

            sb.AppendFormat("---@field {0} {1} {2}\n", f.Name, GetLuaTypeName(f.FieldType), f.IsStatic ? "@[static]" : "");
		}
	}

	private static void ExportProperties(Type t, StringBuilder sb)
	{
		var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		foreach(var p in properties)
		{
		    //忽略已弃用的
		    if(p.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
		    {
		        continue;
		    }

            sb.AppendFormat("---@field {0} {1}\n", p.Name, GetLuaTypeName(p.PropertyType));
		}
	}

	private static void ExportMethods(Type t, StringBuilder sb)
	{
		var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		foreach(var m in methods)
		{
			if (m.IsGenericMethod || m.Name.StartsWith("set_") || m.Name.StartsWith("get_"))
			{
				continue;
			}

            //忽略已弃用的
		    if (m.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
		    {
                continue;
		    }

			var parameters = m.GetParameters();
			var parameterString = "";
			for (var index = 0; index < parameters.Length; index++)
			{
				var p = parameters[index];
				sb.AppendFormat("---@param {0} {1}\n", p.Name, GetLuaTypeName(p.ParameterType));
				parameterString += p.Name;
				if (index < parameters.Length - 1)
				{
					parameterString += ", ";
				}
			}
			sb.AppendFormat("---@return {0}\n", GetLuaTypeName(m.ReturnType));

			var separator = m.IsStatic ? "." : ":";
			sb.AppendFormat("function {0}{1}{2}({3}) end \n", t.Name, separator, m.Name, parameterString);
		}
	}

	private static bool CanExport(Type t)
	{
		if (!t.IsClass && !t.IsValueType)
		{
			return false;
		}
		
		if (t.IsGenericType || t.IsNested || t.IsInterface || t.IsAbstract)
		{
			return false;
		}
		
		if (t.IsSubclassOf(typeof(Attribute)) || t.IsAssignableFrom(typeof(YieldInstruction)))
		{
			return false;
		}

	    //忽略已弃用的
	    if(t.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
	    {
	        return false;
	    }

        //忽略从Lua导出的
	    if (t.Name.StartsWith("Lua_"))
	    {
	        return false;
	    }

	    return true;
	}

	private static string GetLuaTypeName(Type t)
	{
		if (t.Name == "String")
		{
			return "string";
		}
		else if(t.Name == "Void")
		{
			return "void";
		}
		else
		{
			return t.Name;
		}
	}
}

#endif
