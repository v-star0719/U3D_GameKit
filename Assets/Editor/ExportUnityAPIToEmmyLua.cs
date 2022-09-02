#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CamelHot;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class ExportUnityAPIToEmmyLua : MonoBehaviour {
    
    private static string path = "D:/Workspace/EmmyLuaExtenedType/EmmyLuaUnityAPI";

	[MenuItem("Tqm/导出EmmyLuaAPI/All", false, 14)]
	static void Gen_All()
	{
        //unity以前的版本直接用LoadAssembly就可以了，新版不行，直接get。
	    Gen_UnityEngine();
        Gen_Custom();
	}

    [MenuItem("Tqm/导出EmmyLuaAPI/UnityEngine", false, 14)]
    static void Gen_UnityEngine()
    {
        //unity以前的版本直接用LoadAssembly就可以了，新版不行，直接get。
        Debug.Log("=============================================");
        Debug.Log(typeof(GameObject).Assembly.FullName);
        ExportAssembly(typeof(GameObject).Assembly, path);
        Debug.Log("=============================================");
        Debug.Log(typeof(AudioSource).Assembly.FullName);
        ExportAssembly(typeof(AudioSource).Assembly, path);
    }

    [MenuItem("Tqm/导出EmmyLuaAPI/Custom", false, 14)]
    static void Gen_Custom()
    {
        //unity以前的版本直接用LoadAssembly就可以了，新版不行，直接get。
        ExportAssembly(typeof(UILabel).Assembly, path);

        Debug.Log(typeof(CinemachineFreeLook).Assembly.FullName);
        ExportAssembly(typeof(CinemachineFreeLook).Assembly, path);

        Dictionary<Type, string> dict = new Dictionary<Type, string>();
        CamelCustomExport.OnAddCustomClass((t, s) => { dict[t] = s; });
        foreach (var kv in dict)
        {
            ExportType(kv.Key, path, kv.Value);
        }
    }

    static void ExportGenericDelegate(Type t, string ns)
    {

    }

    public static void ExportAssembly(Assembly asm, string path)
	{
	    if(!Directory.Exists(path))
	    {
	        Directory.CreateDirectory(path);
	    }

        Type[] types = asm.GetTypes();
		foreach (Type t in types)
		{
			ExportType(t, path);
		}
	}

	public static void ExportType(Type t, string path, string customName = null)
	{
		if(!CanExport(t))
		{
			return;
		}

		StringBuilder sb = new StringBuilder("");

	    var name = customName ?? t.Name;
        //UnityEngine.Object 不导出基类，不然会出现 ---@class Object : Object 这样的类定义，导致EmmyLua报错
        if(t.BaseType != null && t != typeof(UnityEngine.Object))
		{
			sb.AppendFormat("---@class {0} : {1}\n", name, t.BaseType.Name);
		}
		else
		{
			sb.AppendFormat("---@class {0}\n", name);
		}
		ExportFields(t, sb);
		ExportProperties(t, sb);
        //sb.AppendFormat("local {0} = {{}}\n", t.Name);
	    sb.AppendFormat("{0} = {{}}\n", name); //直接作为全局的对象，这样可以随时提示出来
        ExportMethods(t, sb);

	    try
	    {
	        try
	        {
	            if(string.IsNullOrEmpty(t.Namespace))
	            {
	                File.WriteAllText(path + "/" + t.Name + ".lua", sb.ToString(), Encoding.UTF8);
	            }
	            else
	            {
	                File.WriteAllText(path + "/" + t.Namespace + "." + t.Name + ".lua", sb.ToString(), Encoding.UTF8);
	            }
            }
	        catch (Exception e)
	        {
	            Debug.Log("type:" + t.Name);
	            Debug.LogException(e);
            }
		    
	    }
	    catch(Exception e)
	    {
            Debug.LogError(t.Name + " failed");
	        Debug.LogError(e);
	        throw;
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
		
		if (t.IsGenericType || t.IsInterface)
		{
		    if (t.Name.StartsWith("List") || t.Name.StartsWith("Dictionary"))
		    {
		        return true;
		    }
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
		else if(t.IsGenericType)
		{
		    if (t.Name.StartsWith("List"))
		    {
		        return t.Name + "[]";
		    }
            else if (t.Name.StartsWith("Dictionary"))
		    {
		        return "table<" + t.GenericTypeArguments[0].Name + ", " + t.GenericTypeArguments[1].Name + ">";
		    }
            else
		    {
		        return t.Name;
		    }
        }
		else
		{
		    return t.Name;
		}
	}
}

#endif
