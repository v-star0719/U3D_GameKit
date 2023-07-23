using System;
using UnityEditor;
using UnityEngine;

namespace Kernel.Editor
{
	public partial class EditorGUIUtil
    {
		public static void CopyToSystemClipboard(string content)
		{
			EditorGUIUtility.systemCopyBuffer = content;
		}

		private class GUILayoutVertical : IDisposable
		{
		    public Rect r;
			public GUILayoutVertical(GUILayoutOption[] options)
			{
				r = EditorGUILayout.BeginVertical(options);
			}

			public GUILayoutVertical(GUIStyle style, params GUILayoutOption[] options)
			{
			    r = EditorGUILayout.BeginVertical(style, options);
			}

			public GUILayoutVertical(GUIStyle style)
			{
			    r = EditorGUILayout.BeginVertical(style);
			}

			public void Dispose()
			{
				EditorGUILayout.EndVertical();
			}
		}

		private class GUILayoutHorizontal : IDisposable
		{
		    public Rect r;
		    public GUILayoutHorizontal()
			{
			    r = EditorGUILayout.BeginHorizontal();
			}

			public GUILayoutHorizontal(GUIStyle style)
			{
			    r = EditorGUILayout.BeginHorizontal(style);
			}

			public GUILayoutHorizontal(params GUILayoutOption[] options)
			{
			    r = EditorGUILayout.BeginHorizontal(options);
			}

			public GUILayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
			{
			    r = EditorGUILayout.BeginHorizontal(style, options);
			}

			public void Dispose()
			{
				EditorGUILayout.EndHorizontal();
			}
		}

		private class GUIEditorIndent : IDisposable
		{
			public GUIEditorIndent()
			{
				++EditorGUI.indentLevel;
			}

			public void Dispose()
			{
				--EditorGUI.indentLevel;
			}
		}

		private class GUIScroll : IDisposable
		{
			public GUIScroll(ref Vector2 scroll)
			{
				scroll = EditorGUILayout.BeginScrollView(scroll);
			}

			public GUIScroll(ref Vector2 scroll, params GUILayoutOption[] options)
			{
				scroll = EditorGUILayout.BeginScrollView(scroll, options);
			}

			public GUIScroll(ref Vector2 scroll, GUIStyle style)
			{
				scroll = EditorGUILayout.BeginScrollView(scroll, style);
			}


			public void Dispose()
			{
				EditorGUILayout.EndScrollView();
			}
		}


		private class GUIHandleColor : IDisposable
		{
			private readonly Color old;

			public GUIHandleColor(Color color)
			{
				old = Handles.color;
				Handles.color = color;
			}

			public void Dispose()
			{
				Handles.color = old;
			}
		}

		private class GUILabelWidth : IDisposable
		{
			private readonly float old;

			public GUILabelWidth(float width)
			{
				old = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = width;
			}

			public void Dispose()
			{
				EditorGUIUtility.labelWidth = old;
			}
		}
	}
}