using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Excel;

public class ExportExcel
{
	//Application.dataPath最后没有带'/'，所以这里补上
	public const string outputDirectory = "/ExcelDataExporter/Test/Output/";//输出目录
	public const string tableScriptsDir = outputDirectory + "TableScripts/";//脚本输出目录
	public const string tablePrefabDir = outputDirectory + "TablePrefabs/";//prefab输出目录
	public const string tableTemplatePath = "/ExcelDataExporter/ScriptTemplate/DBTableTemplate.cs";//表脚本模板路径
	public const string tableNameTemplate = "DB{0}Table";//表脚本名模板，会用excel的sheet名字填充
	public const string tableDefineNameTemplate = "DB{0}Conf";//表定义脚本名模板，会用excel的sheet名字填充
	public const string tableEnumPrefix = "Em";//表中枚举类型前缀
	public const string languageFileOutputDir = "/ExcelDataExporter/Test/Output/";//多语言文件的输出路径

	//导出脚本
	//导出数据
	const string logPrefix = "【导出excel】";
	const string logIndent = "====";

	delegate void DealWithTableDelegate(string fileName, DataTable dt);
	
	[MenuItem("配置表/生成表脚本")]
	public static void ExportTableScripts()
	{
		Debug.Log(logPrefix + "======================================================");
		Debug.Log(logPrefix + "生成表脚本");
		IterateSelectedFiles(
			delegate(string fileName, DataTable dt)
			{
				int mark1 = 0;
				int mark2 = 0; 
				try
				{
					ExportScript_TableDefine(fileName, dt);
					mark1++;
					Debug.Log(logPrefix + logIndent + "生成表定义，sheet：" + dt.TableName + "......完成");
					ExportScript_DataTable(fileName, dt);
					mark2++;
					Debug.Log(logPrefix + logIndent + "生成表脚本，sheet：" + dt.TableName + "......完成");
				}
				catch(Exception e)
				{
					Debug.LogException(e);
					if(mark1 == 0)
						Debug.Log(logPrefix + logIndent + "生成表定义，sheet：" + dt.TableName + "......中断");
					if(mark2 == 0)
						Debug.Log(logPrefix + logIndent + "生成表脚本，sheet：" + dt.TableName + "......中断");
				}
			}
		);
		AssetDatabase.Refresh();
	}

	[MenuItem("配置表/生成表数据")]
	public static void ExportTableData()
	{
		Debug.Log(logPrefix + "======================================================");
		Debug.Log(logPrefix + "生成表数据......");
		IterateSelectedFiles(
			delegate(string fileName, DataTable dt)
			{
				try
				{
					ExportTablePrefab(dt);
					Debug.Log(logPrefix + logIndent + "导出sheet：" + dt.TableName + "......完成");
				}
				catch(Exception e)
				{
					Debug.LogException(e);
					Debug.Log(logPrefix + logIndent + "导出sheet：" + dt.TableName + "......中断");
				}
			}
		);
		AssetDatabase.Refresh();
	}

	[MenuItem("配置表/导出多语言")]
	public static void ExportLanguage()
	{
		Debug.Log(logPrefix + "======================================================");
		Debug.Log(logPrefix + "导出多语言......");
		IterateSelectedFiles(
			delegate(string fileName, DataTable dt)
			{
				try
				{
					ExportLanguageFile(dt);
					Debug.LogFormat("sheet{0}......完成， 总计{1}行", dt.TableName, dt.Rows.Count);
				}
				catch(Exception e)
				{
					Debug.LogException(e);
					Debug.LogFormat("sheet{0}......中断", dt.TableName);
				}
			}
		);
		AssetDatabase.Refresh();
	}

	private static void IterateSelectedFiles(DealWithTableDelegate dg)
	{
		if(Selection.objects.Length == 0)
			Debug.Log(logPrefix + "没有选择文件");

		for(int i=0; i<Selection.objects.Length; i++)
		{
			string filePath = AssetDatabase.GetAssetPath(Selection.objects[i]);
			string fileName = Path.GetFileName(filePath);
			if(!filePath.EndsWith(".xlsx"))
			{
				Debug.LogError(logPrefix + fileName + " 不是*.xlsx文件");
				continue;
			}

			Debug.Log(logPrefix + "●处理 " + fileName + "●");
			filePath = filePath.Substring(filePath.IndexOf('/'));
			DataSet excel = null;
			try{
				excel = LoadExcelData(Application.dataPath + filePath);
			}
			catch(System.Exception e){
				Debug.LogError(logPrefix + logIndent + "打开xlsx异常：" + fileName);
				Debug.LogException(e);
			}
			if(excel == null) continue;
			for(int j=0; j<excel.Tables.Count; j++)
			{
				DataTable dt = excel.Tables[j];
				if(!IsTableAvailable(fileName, dt))
					continue;
				dg(fileName, dt);
			}
		}
	}

	private static DataSet LoadExcelData(string fullFilePath)
	{
		FileStream stream = File.Open(fullFilePath, FileMode.Open, FileAccess.Read);
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
		DataSet result = excelReader.AsDataSet();
		excelReader.Close();
		stream.Close();
		return result;
	}

	private static bool IsTableAvailable(string xlsxFileName, DataTable dt)
	{
		string tableName = dt.TableName;
		char c = tableName[0];
		if(c < 'A' || (c > 'Z' && c < 'a') || c > 'z'){
			Debug.LogFormat(logPrefix + logIndent + "忽略sheet：[{0}]-[{1}]，因为没有以字母开头", xlsxFileName, tableName);
			return false;//不以字母开头的表忽略
		}
		if(dt.Rows.Count < 4){
			Debug.LogErrorFormat(logPrefix + logIndent + "错误sheet：[{0}]-[{1}]，行数小于4", xlsxFileName, tableName);
			return false;
		}
		return true;
	}

	public static void ExportTablePrefab(DataTable dt)
	{
		//输出文件夹不存在就创建
		string fullDirPath = Application.dataPath + tablePrefabDir;
		if(!Directory.Exists(fullDirPath))
			Directory.CreateDirectory(fullDirPath);

		string tableDefineScriptName = string.Format(tableDefineNameTemplate, dt.TableName);
		string tableScriptName = string.Format(tableNameTemplate, dt.TableName);

		//表
		Type tableType = GetType(tableScriptName);
		if(tableType == null)
		{
			Debug.LogErrorFormat(logPrefix + logIndent + "表脚本{0}未找到", tableScriptName);
			return;
		}

		//表定义
		Type tableDefineType = GetType(tableDefineScriptName);
		if(tableDefineType == null)
		{
			Debug.LogErrorFormat(logPrefix + logIndent + "表定义脚本{0}未找到", tableDefineScriptName);
			return;
		}

		//获取预置
		string tablePrefabPath = "Assets" + tablePrefabDir + tableScriptName + ".prefab";
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(tablePrefabPath);
		GameObject go = null;
		object table = null;
		bool isPrefabExist = false;
		if(prefab == null)
		{
			go = new GameObject(tableScriptName);
			table = go.AddComponent(tableType);
			isPrefabExist = false;
		}
		else
		{
			go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			table = go.GetComponent(tableType);
			isPrefabExist = true;
		}

		//这个库会把最后多出的空行（比如调整单元格格式的）也算进来，所以需要额外跳过末尾的空行
		int recordCount = dt.Rows.Count - 4;
		for(int i=dt.Rows.Count-1; i>=4; i--)
		{
			if(!string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
			   break;
			recordCount--;
		}

		//创建record数组
		Array recordArray = Array.CreateInstance(tableDefineType, recordCount);
		int columnCount = dt.Columns.Count;
		for(int i=0; i<recordCount; i++)
		{
			int row = i+4;
			object record = Activator.CreateInstance(tableDefineType);
			recordArray.SetValue(record, i);
			for(int j=0; j<columnCount; j++)
			{
				//不导出的列跳过
				if(string.Compare(dt.Rows[0][j].ToString(), "1") != 0) continue;
				string fieldName = dt.Rows[2][j].ToString();
				FieldInfo pi = tableDefineType.GetField(fieldName, BindingFlags.Instance|BindingFlags.Public);
				if(pi == null)
				{
					Debug.LogErrorFormat(logPrefix + "获取第{0}列字段{1}信息失败：可能不在脚本中", j+1, fieldName);
					continue;
				}

				try{
					pi.SetValue(record, GetValueFromString(row, fieldName, dt.Rows[1][j].ToString(), dt.Rows[row][j].ToString()));
				}
				catch(Exception e){
					Debug.LogErrorFormat(logPrefix + "解析{0}行{1}列{2}字段异常，如下：", row+1, j+1, fieldName);
					Debug.LogException(e);
				}
			}
		}

		FieldInfo fi = tableType.GetField("recordArray");
		fi.SetValue(table, recordArray);

		if(isPrefabExist)
			PrefabUtility.ReplacePrefab(go, prefab);
		else
			PrefabUtility.CreatePrefab(tablePrefabPath, go);
		GameObject.DestroyImmediate(go);
	}

	//如果不存在，则创建
	public static void ExportScript_TableDefine(string xlsxFileName, DataTable dt)
	{
		//输出文件夹不存在就创建
		string fullDirPath = Application.dataPath + tableScriptsDir;
		if(!Directory.Exists(fullDirPath))
			Directory.CreateDirectory(fullDirPath);

		string scriptName = string.Format(tableDefineNameTemplate, dt.TableName);
		string fileFullPath = string.Format("{0}{1}{2}.cs", Application.dataPath, tableScriptsDir, scriptName);
		FileStream fs = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs);

		//类名
		sw.Write("using UnityEngine;\r\n\r\n[System.Serializable]\r\npublic class ");
		sw.Write(scriptName + "\r\n{\r\n");

		//编号
		for(int i=0; i<dt.Columns.Count; i++)
		{
			//是否导出
			if(string.Compare(dt.Rows[0][i].ToString(), "1") != 0) continue;
			//注释
			sw.Write("\t///" + dt.Rows[3][i].ToString() + "\r\n");
			//类型
			sw.Write("\tpublic " + dt.Rows[1][i].ToString());
			//字段名
			sw.Write(" " + dt.Rows[2][i].ToString() + ";\r\n");
		}
		sw.Write("}");
		sw.Close();
		fs.Close();
	}

	//不存在，则创建
	public static void ExportScript_DataTable(string xlsxFileName, DataTable dt)
	{
		//读取模板文件
		StreamReader sr = new StreamReader(Application.dataPath + tableTemplatePath);
		sr.ReadLine();sr.ReadLine();sr.ReadLine();sr.ReadLine();//跳过说明行
		string templateScript = sr.ReadToEnd();
		sr.Close();

		//输出文件夹不存在就创建
		string fullDirPath = Application.dataPath + tableScriptsDir;
		if(!Directory.Exists(fullDirPath))
			Directory.CreateDirectory(fullDirPath);

		string tableDefineScriptName = string.Format(tableDefineNameTemplate, dt.TableName);
		string scriptName = string.Format(tableNameTemplate, dt.TableName);
		string fileFullPath = string.Format("{0}{1}{2}.cs", Application.dataPath, tableScriptsDir, scriptName);
		string recordId = dt.Rows[2][0].ToString();
		string recordIdType = dt.Rows[1][0].ToString();

		FileStream fs = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs);
		templateScript = templateScript.Replace("DBTableDefineTemplate", tableDefineScriptName);
		templateScript = templateScript.Replace("DBTableTemplate", scriptName);
		templateScript = templateScript.Replace("recordId", recordId);
		templateScript = templateScript.Replace("DBTableRecordIDType", recordIdType);

		sw.Write(templateScript);
		sw.Close();
		fs.Close();
	}

	public static Type GetType(string typeName)
	{
		//内置类型
		Type t = Type.GetType(typeName);
		if(t != null) return t;

		//自定义类型
		t = Type.GetType(typeName + ",Assembly-CSharp");
		if(t != null) return t;

		Debug.LogError(logPrefix + "Can't get type of " + typeName);
		return null;
	}

	public static object GetValueFromString(int row, string valueName, string valueType, string valueString)
	{
		//按使用频率依次处理
		if(valueType == "int")
		{
			int n = 0;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!int.TryParse(valueString, out n))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return n;
		}
		else if(valueType == "string")
		{
			return valueString;
		}
		else if(valueType == "float")
		{
			float f = 0;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!float.TryParse(valueString, out f))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return f;
		}
		else if(valueType == "Vector2")
		{
			Vector2 v = Vector2.zero;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!TryParseVector2(valueString, out v))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return v;
		}
		else if(valueType == "Vector3")
		{
			Vector3 v = Vector3.zero;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!TryParseVector3(valueString, out v))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return v;
		}
		else if(valueType == "bool")
		{
			bool b = false;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!bool.TryParse(valueString, out b))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return b;
		}
		else if(valueType == "long")
		{
			long l = 0;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!long.TryParse(valueString, out l))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return l;
		}
		else if(valueType == "int[]")
		{
			int[] array = null;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!TryParseIntArray(valueString, out array))
					GetValueFromStringLog(row+1, valueName, valueType, valueString);
			}
			return array;
		}
		else if(valueType.StartsWith(tableEnumPrefix))
		{
			object o = null;
			if(!string.IsNullOrEmpty(valueString))
			{
				if(!TryParseEnum(valueType, valueString, ref o))
					GetValueFromStringLog(row + 1, valueName, valueType, valueString);
			}
			else
				Debug.LogWarningFormat(logPrefix + "行{0}的{1}的枚举类型{2}值为空，默认为值为0的枚举", row + 1, valueName, valueType);
			return o;
		}

		Debug.LogErrorFormat(logPrefix + "从行{0}的{1}字段获取{2}类型的值失败：暂不支持的数据类型 {3}", row+1, valueName, valueType, valueString);
		return null;
	}
	private static void GetValueFromStringLog(int row, string valueName, string valueType, string valueString){
		Debug.LogErrorFormat(logPrefix + "从行{0}的{1}字段获取{2}类型的值失败：数据格式错误 {3}", row+1, valueName, valueType, valueString);
	}

	private static bool TryParseVector2(string value, out Vector2 v)
	{
		v = Vector2.zero;
		int index = 0;
		int d = 1;
		for(; d<3; d++)
		{
			string floatString = GetFloatInString(value, ref index);
			float f = 0;
			if(!float.TryParse(floatString, out f))
				return false;
			if(d == 1) v.x = f;
			if(d == 2) v.y = f;
		}
		return d == 3;
	}

	private static bool TryParseVector3(string value, out Vector3 v)
	{
		v = Vector2.zero;
		int index = 0;
		int d = 1;
		for(; d<4; d++)
		{
			string floatString = GetFloatInString(value, ref index);
			float f = 0;
			if(!float.TryParse(floatString, out f))
				return false;
			if(d == 1) v.x = f;
			if(d == 2) v.y = f;
			if(d == 3) v.z = f;
		}
		return d == 4;
	}

	private static string GetFloatInString(string value, ref int start)
	{
		int valueStart = -1;
		for(; start<value.Length; start++)
		{
			//先找到值的开始
			char c = value[start];
			if(valueStart < 0)
			{
				if(('0'<=c && c<='9') || c == '-')
					valueStart = start;
			}
			else
			{
				//如果没有找到开始，则不找结束点
				//再找到数值的结束点
				if(c!='.' && (c<'0' || c>'9'))
					return value.Substring(valueStart, start-valueStart);
			}
		}
		//如果到最后也没有结尾符，直接取字符串到末尾
		//最后就一个数值时会直接退出循环，也相当于没找到结束符
		if(valueStart >=0 && start == value.Length)
			return value.Substring(valueStart, value.Length-valueStart);

		return string.Empty;
	}

	private static List<int> workingIntlist = new List<int>(8);
	private static bool TryParseIntArray(string value, out int[] array)
	{
		workingIntlist.Clear();
		for(int i=0; i<value.Length; i++)
		{
			char c = value[i];
			if('0' <= c && c <= '9')
			{
				//发现数值，开始读取
				int n=0;
				while('0' <= c && c <= '9')
				{
					n = n*10 + c - '0';
					i++;
					if(i >= value.Length) break;
					c = value[i];
				}
				workingIntlist.Add(n);
			}
		}
		array = workingIntlist.ToArray();
		if(array.Length == 0)
			return false;
		return true;
	}

	private static bool TryParseEnum(string enumType, string enumValue, ref object outValue)
	{
		Type t = GetType(enumType);//如果没有该枚举类型，则前面会报错
		if(t == null) return false;

		outValue = Enum.Parse(t, enumValue);
		return true;
	}

	//一个sheet是一个多语言
	//刷新ProgressBar很费时间，所以就1个表刷一次
	private static void ExportLanguageFile(DataTable dt)
	{
		//输出文件夹不存在就创建
		string fullDirPath = Application.dataPath + languageFileOutputDir;
		if(!Directory.Exists(fullDirPath))
			Directory.CreateDirectory(fullDirPath);

		//打开多语言文件，准备写入
		string languageFileName = dt.TableName;
		string languageFilePath =  string.Format("{0}{1}.txt", fullDirPath, languageFileName);
		FileStream fs = new FileStream(languageFilePath, FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs);

		//导出到文本
		ExportLanguageHelper(dt, sw, languageFilePath);

		sw.Close();
		fs.Close();
	}

	private static void ExportLanguageHelper(DataTable sheet, StreamWriter sw, string fileFullPath)
	{
		try
		{
			//模块分隔线
			sw.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////////////////////");
			sw.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////////////////////");
			sw.WriteLine("//@" + sheet.TableName);
			sw.WriteLine("///////////////////////////////////////////////////////////////////////////////////////////////////////////////");

			//模块内容
			for(int i = 0; i < sheet.Rows.Count; i++)
			{
				DataRow row = sheet.Rows[i];
				string value1 = row[0].ToString();
				string value2 = row[1].ToString();
				if(!string.IsNullOrEmpty(value1))
				{
					if(!string.IsNullOrEmpty(value2))
						sw.WriteLine(string.Format("{0} = {1}", value1, value2));
					else
						sw.WriteLine(string.Format("{0} = ", value1));
				}
			}
		}
		catch(System.Exception ex)
		{
			Debug.LogError("发生了异常，" + "文件 = " + fileFullPath + "，sheetName = " + sheet.TableName);
			Debug.LogException(ex);
		}
	}
}
