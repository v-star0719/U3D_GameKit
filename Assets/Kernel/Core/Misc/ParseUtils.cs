using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Kernel.Core
{
    class ParseUtils
    {
		public static bool TryParseVector2(string value, out Vector2 v)
		{
			v = Vector2.zero;
			int index = 0;
			int d = 1;
			for (; d < 3; d++)
			{
				string floatString = GetFloatInString(value, ref index);
				if (!float.TryParse(floatString, out float f))
					return false;
				if (d == 1) v.x = f;
				if (d == 2) v.y = f;
			}
			return d == 3;
		}

        public static bool TryParseVector3(string value, out Vector3 v)
		{
			v = Vector2.zero;
			int index = 0;
			int d = 1;
			for (; d < 4; d++)
			{
				string floatString = GetFloatInString(value, ref index);
				if (!float.TryParse(floatString, out float f))
					return false;
				if (d == 1) v.x = f;
				if (d == 2) v.y = f;
				if (d == 3) v.z = f;
			}
			return d == 4;
		}

        public static string GetFloatInString(string value, ref int start)
		{
			int valueStart = -1;
			for (; start < value.Length; start++)
			{
				//先找到值的开始
				char c = value[start];
				if (valueStart < 0)
				{
					if (('0' <= c && c <= '9') || c == '-')
						valueStart = start;
				}
				else
				{
					//如果没有找到开始，则不找结束点
					//再找到数值的结束点
					if (c != '.' && (c < '0' || c > '9'))
						return value.Substring(valueStart, start - valueStart);
				}
			}
			//如果到最后也没有结尾符，直接取字符串到末尾
			//最后就一个数值时会直接退出循环，也相当于没找到结束符
			if (valueStart >= 0 && start == value.Length)
				return value.Substring(valueStart, value.Length - valueStart);

			return string.Empty;
		}

		private static readonly List<int> workingIntlist = new List<int>(8);
        public static bool TryParseIntArray(string value, out int[] array)
		{
			workingIntlist.Clear();
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if ('0' <= c && c <= '9')
				{
					//发现数值，开始读取
					int n = 0;
					while ('0' <= c && c <= '9')
					{
						n = n * 10 + c - '0';
						i++;
						if (i >= value.Length) break;
						c = value[i];
					}
					workingIntlist.Add(n);
				}
			}
			array = workingIntlist.ToArray();
			if (array.Length == 0)
				return false;
			return true;
		}
	}
}
