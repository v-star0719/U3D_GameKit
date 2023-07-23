using System;
using System.Collections.Generic;
using System.Reflection;
using Kernel.Core;
using Kernel.Editor;
using Kernel.Edit;
using UnityEditor;
using UnityEngine;

namespace Kernel.Edit
{
	/*
	 * 
	 * 要将FightCore的数据和编辑器代码分离开来，所以按照unity inspector这样的写了这么个东西
	 * 编辑器代码另外用一个类写，然后再这类上面用CustomDataEditor属性指明是哪个数据类的编辑器
	 * 
	 * 这个Manager通过反射获取数据类型和它对应的editor，直接调用OnGUI方法就会找到对应的editor进行OnGUI
	 * 
	 */
	public class DataEditorManager : Singleton<DataEditorManager>
	{
		///数据类型和它对应的编辑器
		public Dictionary<Type, IDataEditor> TypeEditor = new Dictionary<Type, IDataEditor>();

		///比如触发器的编辑器 是一个窗口 会同时显示多个，所以每个数据都要对应一个编辑器
		///每个数据和它对应的编辑器
		///类型那个只是用来获取编辑器
		public Dictionary<object, IDataEditor> ObjectEditors = new Dictionary<object, IDataEditor>();

		private Assembly assembly;

		public DataEditorManager()
		{
#if UNITY_EDITOR
			assembly = Assembly.Load("Assembly-CSharp-Editor");
			if(assembly != null)
			{
				foreach(var t in assembly.ExportedTypes)
				{
					var customEditor = t.GetCustomAttribute<DataEditorAttribute>(false);
					if(customEditor != null)
					{
						try
						{
							var editor = assembly.CreateInstance(t.FullName) as IDataEditor;
							TypeEditor.Add(customEditor.DataType, editor);
						}
						catch (Exception e)
						{
							Debug.LogException(e);
							Debug.Log(t.FullName);
						}
						
					}
				}
			}
#endif
		}

		public bool IsEditorExisted(object data)
		{
			return TypeEditor.ContainsKey(data.GetType());
		}

		public IDataEditor GetEditor(object data)
		{
			IDataEditor editor = null;
			if(!TypeEditor.TryGetValue(data.GetType(), out editor))
			{
				return null;
			}
			
			IDataEditor objectEditor = null;
			if(!ObjectEditors.TryGetValue(data, out objectEditor))
			{
				objectEditor = assembly.CreateInstance(editor.GetType().FullName) as IDataEditor;
				ObjectEditors.Add(data, objectEditor);
			}
			return objectEditor;
		}

		///显示一个数据的Editor
		public void ShowDataEditor(object data, ref bool changed, bool showBoxBg = false, string title = null)
		{
			var editor = GetEditor(data);
			if(editor != null)
			{

#if UNITY_EDITOR
				//调试
				//using(GUIUtil.ContentColor(UnityEngine.Color.red))
				//{
				//	UnityEngine.GUILayout.Label($"[{searchTargetData.GetType().Name}] 正在使用分离的编辑器");
				//}
#endif
				editor.SetData(data);
				IDisposable dis2 = null;
				var folded = false;
				if (showBoxBg)
				{
					dis2 = EditorGUIUtil.LayoutVertical(EditorGUIUtil.StyleBox);
					if (!string.IsNullOrEmpty(title))
					{
						Rect rect = EditorGUILayout.BeginHorizontal("box");
						var mark = editor.IsFolded ? "▲" : "▼";
						GUILayout.Label($" {mark}{title} ");
						EditorGUILayout.EndHorizontal();
						if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
						{
							editor.IsFolded = !editor.IsFolded;
						}

						folded = editor.IsFolded;
					}
				}

				if (!folded)
				{
					editor.OnGUI(ref changed);
				}
				dis2?.Dispose();
			}
		}

		public void ShowDataGizmos(object data)
		{
			var editor = GetEditor(data);
			if(editor != null)
			{
				editor.SetData(data);
				editor.OnDrawGizmos();
			}
		}
	}
}
