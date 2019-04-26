using System.Runtime.InteropServices;
using LitJson;

public static class JsonExtention
{
	public static int ReadInt(this JsonData json, string key)
	{
		if (json.ContainsKey(key))
		{
			return (int)json[key];
		}
		return 0;
	}

	public static float ReadFloat(this JsonData json, string key)
	{
		if(json.ContainsKey(key))
		{
			return (float)json.ReadDouble(key);
		}
		return 0;
	}

	public static double ReadDouble(this JsonData json, string key)
	{
		if(json.ContainsKey(key))
		{
			var data = json[key];
			if(data.IsInt)
			{
				return (int)data;
			}
			if(data.IsLong)
			{
				return (int)data;
			}
			if(data.IsDouble)
			{
				return (double)data;
			}
			return 0;
		}
		return 0;
	}

	public static string ReadString(this JsonData json, string key)
	{
		if(json.ContainsKey(key))
		{
			return (string)json[key];
		}
		return "";
	}

	public static bool ReadBoolean(this JsonData json, string key)
	{
		if(json.ContainsKey(key))
		{
			return (bool)json[key];
		}
		return false;
	}
}