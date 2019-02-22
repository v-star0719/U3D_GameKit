using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Security.Cryptography;

public static class GameUtilities
{

#region time

	//获取1970.1.1开始的时间
	public static DateTime ConvertJavaTimeToCSharpTIme(long javaTime)
    {
        DateTime dt = new DateTime(1970, 1, 1);
        DateTime dt_1970 = dt.ToLocalTime();
        long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度     
        long tricksCSharp = 10000 * javaTime + tricks_1970;
        DateTime wantData = new DateTime(tricksCSharp);
        return wantData;
    }

	/// 返回从1970/1/1 00:00:00至今的所经过的毫秒数，java 中的时间是这样表示的
	public static long GetCurJavaTIme()
	{
		return (DateTime.UtcNow.Ticks - CommonDefine.CSHARP_1970_TIME) / 10000;//日志日期刻度   
	}

	//毫秒转化为 分:秒
	public static string GetMinuteSecondString(long t)
	{
		const long MinuteTime = 60 * 1000;
		const long SecondTime = 1000;

		long minute = (t / MinuteTime);
		long second = (t % MinuteTime) / SecondTime;
		return string.Format("{0:00}:{1:00}", minute, second);
	}

	//毫秒转化为 时:分:秒
	public static string GetHourMinuteSecondString(long t)
	{
		//const long DayTime = 24 * 60 * 60 * 1000;
		const long HourTime = 60 * 60 * 1000;
		const long MinuteTime = 60 * 1000;
		const long SecondTime = 1000;

		//long day = time / DayTime;
		long hour = t / HourTime;
		long minute = (t % HourTime) / MinuteTime;
		long second = (t % MinuteTime) / SecondTime;
		return string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
	}
	
	//秒转化为 天:小时
	public static string GetDayHourString(int seconds)
	{
		seconds /= 3600;  //(60 * 60);//剩余的小时数
		int day = seconds / 24;
		int hour = (seconds % 24); 
		return string.Format(StormLocalization.Get("TimeFormat_Day_Hour"), day, hour);//"{0}天{1:D2}小时"
    }

	/// 1.1.1年开始的毫秒数转换成时间格式 年月日 时：分：秒
	public static string GetDateTimeString(long t)
	{
		DateTime time = new DateTime(t);
		string result =  string.Format("{0:D4}", time.Year) + "年"
                        + string.Format("{0:D2}", time.Month) + "月"
                        + string.Format("{0:D2}", time.Day) + "日"
                        + string.Format("{0:D2}", time.Hour) + ":"
                        + string.Format("{0:D2}", time.Minute) + ":" 
                        + string.Format("{0:D2}", time.Second);

		return result;
	}




#endregion


#region GameObject
	//获取GameObject的路径，从根GameObject开始
	public static string GetTransformPath(Transform trans)
	{
		string s = trans.name;
		Transform t = trans;
		while (t.parent != null)
		{
			s = t.parent.name + "/" + s;
			t = t.parent;
		}
		return s;
	}

	//设置所有子节点的层级
	public static void SetAllChildLayer(Transform parent, int layer)
	{
		parent.gameObject.layer = layer;
		foreach(Transform trans in parent)
		{
			SetAllChildLayer(trans, layer);
		}
	}

	//将隐藏改为移到相机外去
	public const float ACTIVE_DISTANCE = 20000f;//平移距离，x方向
	public const float ACTIVE_BASELINE = 10000f;//是否以隐藏的判断基准
	public static void SetActive(Transform trans, bool bActive)
	{
		//Debug.Log(string.Format("Muitls.SetActive({0}, {1}", go.name, bActive));
		if(bActive)
		{
			//显示
			if(IsActive(trans)) return;
			Vector3 pos = trans.localPosition;
			pos.x -= ACTIVE_DISTANCE;
			trans.localPosition = pos;
		}
		else
		{
			//隐藏
			if(!IsActive(trans)) return;
			Vector3 pos = trans.localPosition;
			pos.x += ACTIVE_DISTANCE;
			trans.localPosition = pos;
		}
	}

	//是否可见
	public static bool IsActive(Transform trans)
	{
		return trans.localPosition.x < ACTIVE_BASELINE;
	}

#endregion 

    #region Parse


    #endregion

	#region Draw Gizmos
	
	//draw round on y=0 panel
	public static void DrawRound(Vector3 center, float r)
	{	
		Vector3 beginPoint = Vector3.zero;
		Vector3 firstPoint = Vector3.zero;
		for (float theta = 0; theta < 2 * Mathf.PI; theta += Mathf.Deg2Rad)
		{
			float x = r * Mathf.Cos(theta);
			float z = r * Mathf.Sin(theta);
			Vector3 endPoint = new Vector3(x, 0, z);
			if (theta == 0)
			{
				firstPoint = endPoint;
			}
			else
			{
				Gizmos.DrawLine(beginPoint+center, endPoint+center);
			}
			beginPoint = endPoint;
		}
		
		// 绘制最后一条线段
		Gizmos.DrawLine(firstPoint, beginPoint);
	}
	#endregion

	#region Device State Access
	public static bool HaveWifi()
	{
		if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			return true;
		else 
			return false;
	}
	public static bool HaveNetwork()
	{
		if(Application.internetReachability != NetworkReachability.NotReachable)
			return true;
		else 
			return false;
	}

	#endregion


	#region MD5 encode

	/// MD5 16位加密 加密后密码为大写
	public static string GetMd5Str(string ConvertString)
	{
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
		t2 = t2.Replace("-", "");
		return t2;
	}
	
	/// MD5 16位加密 加密后密码为小写
	public static string Get16Md5Str(string ConvertString)
	{
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
		t2 = t2.Replace("-", "");
		t2 = t2.ToLower();
		return t2;
	}
	
	
	/// MD5　32位加密
	static  string Get32Md5Str(string str)
	{
		string cl = str;
		string pwd = "";
		MD5 md5 = MD5.Create();//实例化一个md5对像
		// 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
		byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
		// 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
		for (int i = 0; i < s.Length; i++)
		{
			// 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
			pwd = pwd + s[i].ToString("X");
			
		}
		return pwd;
	}

	#endregion

	#region text operation
	public static bool IsTelePhone(string str_handset)
	{
		return !string.IsNullOrEmpty(str_handset)&&str_handset.Length==11&&System.Text.RegularExpressions.Regex.IsMatch(str_handset,@"^[1]+\d{10}");
	}

	public static bool IsNumber(string str_number)
	{
		return System.Text.RegularExpressions.Regex.IsMatch(str_number,@"^[0-9]*$");
	}

	public static bool IsChinese(string str)
	{
		return System.Text.RegularExpressions.Regex.IsMatch(str,@"[\u4e00-\u9fa5]+$");
	}

	public static int PraseInt(string str, int defValue)
	{
		try
		{
			str = str.Trim();
			if (string.IsNullOrEmpty(str))
				return defValue;
			return System.Convert.ToInt32(str);
		}
		catch(System.Exception e)
        {
			Debug.LogError("Exception str=" + str+" "+e.ToString());
			return defValue;
		}
	}

	//根据名称中的数字排列
	public static void SortGoByNumberInName<T>(T[] array) where T : MonoBehaviour
	{
		//冒泡排序，从小到大
		int n=array.Length;
		for(int i=0; i<n-1; i++)
		{
			for(int j=0; j<n-1-i; j++)
			{
				if(GetIntFromString(array[j].name) > GetIntFromString(array[j+1].name))
				{
					T t = array[j];
					array[j] = array[j+1];
					array[j+1] = t;
				}
			}
		}
	}

	//读取字符串中的一个数字
	public static int GetIntFromString(string str)
	{
		int number = 0;
		bool bFindInt = false;
		for(int i=0; i<str.Length; i++)
		{
			if(str[i]>='0' && str[i]<='9')
			{
				bFindInt = true;
				number = number*10 + str[i] - '0';
			}
			else if(bFindInt)
				break;
		}
		return number;
	}

	//获取字符数量，这个地方有问题，不同的语言是不一样的
	public static int GetCharactorCount(string str)
	{
		int n=0;
		const char c = (char)255;
		for(int i=0; i<str.Length; i++)
		{
			if(str[i] < c)
				n++;
			else
				n+=2;
		}
		return n;
	}

	//超过数量的部分，用一个后缀表示。问题同上面
	public static string TruncateString(string orgText, int maxCharactorCount, string suffix)
	{
		int n=0;
		const char c = (char)255;
		for(int i=0; i<orgText.Length; i++)
		{
			if(orgText[i] < c)
				n++;
			else
				n+=2;
			if(n > maxCharactorCount)
				return orgText.Substring(0, i) + suffix;//取子字符串
		}
		return orgText;//原来的字数没超，原样返回
	}

	///从124@1233@555@44@这样的字符串中提取数字，@是分隔符
	public static List<int> GetIntArrayInString(string str, char separator)
	{
		List<int> numList = new List<int>();
		if(string.IsNullOrEmpty(str)) return numList;

		string[] strNumbers = str.Split(separator);
		for(int i=0; i<strNumbers.Length; i++)
		{
			try
			{
				numList.Add(System.Convert.ToInt32(strNumbers[i]));
			}
			catch(System.Exception e)
			{
				Debug.Log(e.Message);
			}
		}
		return numList;
	}

	/// Urlencode 如果是中文需要两次encode,服务器再主动调用一次decode,
	public static string UrlEncode(string str)
	{
		string af =  Uri.EscapeDataString(str);
		return af;
	}

	//在Windows上运行Unity3D，WWW用file协议加载文件时，路径部分必需用"\"，否则也可能成功也可能不成功
	//eg: file://e:/a.txt  -> file://e:\a.txt
	public static string ConvertNativeUrlToWindowsPlatform(string url)
	{
		#if UNITY_STANDALONE_WIN
		if (url.IndexOf ("file://") > -1) 
		{
			url = url.Replace (@"/", @"\");
			url = url.Replace (@"file:\\", @"file://");
		}
		#endif
		return url;
	}

#endregion
}

