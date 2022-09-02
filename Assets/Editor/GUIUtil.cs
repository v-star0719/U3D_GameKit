using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace GameKit
{

    public partial class GUIUtil
    {
        public static IDisposable BackgroundColor(Color color)
        {
            return new GUIBackgroundColor(color);
        }

        public static IDisposable Color(Color color)
        {
            return new GUIColor(color);
        }

        public static IDisposable ContentColor(Color color)
        {
            return new GUIContentColor(color);
        }

        public static IDisposable Enabled(bool enabled)
        {
            return new GUIEnabled(enabled);
        }

#if UNITY_EDITOR
        public static bool RightClicked(Rect rect)
        {
            return Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition);
        }

        public static IDisposable Settings(params IDisposable[] args)
        {
            return new GUISettings(args);
        }

        ///紧凑的，label宽度为文字宽度，options控制菜单部分
        public static int IntFieldCampact(string label, int value, ref bool changed, params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return IntField("", value, ref changed, EditorStyles.numberField, options);
        }

        public static int IntField(string label, int value, ref bool changed, params GUILayoutOption[] options)
        {
            return IntField(label, value, ref changed, EditorStyles.numberField, options);
        }

        public static int IntField(string label, int value, ref bool changed, GUIStyle style,
            params GUILayoutOption[] options)
        {
            var o = value;
            var n = EditorGUILayout.IntField(label, value, style, options);
            if (n != o)
            {
                changed = true;
            }

            return n;
        }

        public static float FloatFieldCampact(string label, float value, ref bool changed,
            params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return FloatField("", value, ref changed, EditorStyles.numberField, options);
        }

        public static float FloatField(string label, float value, ref bool changed, params GUILayoutOption[] options)
        {
            return FloatField(label, value, ref changed, EditorStyles.numberField, options);
        }

        public static float FloatField(string label, float value, ref bool changed, GUIStyle style,
            params GUILayoutOption[] options)
        {
            //var o = value;
            var n = EditorGUILayout.FloatField(label, value, style, options);
            //if(!n.Equals(o))
            //{
            //	changed = true;
            //}
            return n;
        }

        ///紧凑的，label宽度为文字宽度，options控制菜单部分
        public static string TextFieldCampact(string label, string value, ref bool changed,
            params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return TextField("", value, ref changed, EditorStyles.textField, options);
        }

        public static string TextField(string label, string value, ref bool changed)
        {
            return TextField(label, value, ref changed, EditorStyles.textField);
        }

        public static string TextField(string label, string value, ref bool changed, GUIStyle style,
            params GUILayoutOption[] options)
        {
            var o = value;
            var n = EditorGUILayout.TextField(label, value, style, options);
            if (n != o)
            {
                changed = true;
            }

            return n;
        }

        public static Color ColorField(string label, Color value, ref bool changed, params GUILayoutOption[] options)
        {
            var o = value;
            var n = EditorGUILayout.ColorField(label, value, options);
            if (n != o)
            {
                changed = true;
            }

            return n;
        }

        public static Vector3 Vector3Field(string label, Vector3 value, ref bool changed,
            params GUILayoutOption[] options)
        {
            var o = value;
            var n = EditorGUILayout.Vector3Field(label, value, options);
            if (n != o)
            {
                changed = true;
            }

            return n;
        }

        public static Vector2 Vector2Field(string label, Vector2 value, ref bool changed,
            params GUILayoutOption[] options)
        {
            var o = value;
            var n = EditorGUILayout.Vector2Field(label, value, options);
            if (n != o)
            {
                changed = true;
            }

            return n;
        }

        public static float Slider(string label, float value, float left, float right, ref bool changed,
            params GUILayoutOption[] options)
        {
            //var o = value;
            var n = EditorGUILayout.Slider(label, value, left, right, options);
            //if(!n.Equals(o))
            //{
            //	changed = true;
            //}
            return n;
        }

        public static bool ToggleCompact(string label, bool value, ref bool changed, params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return Toggle("", value, ref changed, EditorStyles.toggle, options);
        }

        public static bool Toggle(string label, bool value, ref bool changed, params GUILayoutOption[] options)
        {
            return Toggle(label, value, ref changed, EditorStyles.toggle, options);
        }

        public static bool Toggle(string label, bool value, ref bool changed, GUIStyle style,
            params GUILayoutOption[] options)
        {
            var o = value;
            var n = EditorGUILayout.Toggle(label, value, style, options);
            if (n != o)
            {
                changed = true;
            }

            return n;
        }

        public static Object ObjectField(Object obj, Type type, bool allowSceneObj, ref bool changed)
        {
            var oldObj = obj;
            var newObj = EditorGUILayout.ObjectField(obj, type, allowSceneObj);
            if (newObj != null && oldObj != null)
            {
                if (!newObj.Equals(oldObj))
                {
                    changed = true;
                }
            }
            else if (newObj != null || oldObj != null)
            {
                changed = true;
            }

            return newObj;
        }

        public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick)
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, 16f, 16f);
            return EditorGUI.Foldout(rect, foldout, content, toggleOnLabelClick);
        }

        //会在本地记录折叠状态
        public static bool Foldout(string content, string key)
        {
            bool foldout = EditorPrefs.GetBool(key, false);
            bool val = Foldout(foldout, content, true);
            if (val != foldout)
            {
                EditorPrefs.SetBool(key, val);
            }

            return val;
        }

        ///紧凑的，label宽度为文字宽度，options控制菜单部分
        public static Enum EnumPopupExCompact(string label, Enum value, ref bool changed,
            params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return EnumPopupEx("", value, ref changed, EditorStyles.popup, options);
        }

        public static int PopupCompact(string label, int selectedIndex, string[] popup, ref bool changed,
            params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return Popup("", selectedIndex, popup, ref changed, EditorStyles.popup, options);
        }

        public static int Popup(string label, int selectedIndex, string[] popup, ref bool changed,
            params GUILayoutOption[] options)
        {
            return Popup(label, selectedIndex, popup, ref changed, EditorStyles.popup, options);
        }

        public static int Popup(string label, int selectedIndex, string[] popup, ref bool changed, GUIStyle style,
            params GUILayoutOption[] options)
        {
            int ret = EditorGUILayout.Popup(label, selectedIndex, popup, style, options);
            if (ret != selectedIndex)
            {
                changed = true;
            }

            return ret;
        }

        public static Enum EnumPopupEx(string label, Enum value, ref bool changed, params GUILayoutOption[] options)
        {
            return EnumPopupEx(label, value, ref changed, EditorStyles.popup, options);
        }

        public static Enum EnumPopupExCampact(string label, Enum value, ref bool changed,
            params GUILayoutOption[] options)
        {
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            return EnumPopupEx("", value, ref changed, EditorStyles.popup, options);
        }

        public static Enum EnumPopupEx(string label, Enum value, ref bool changed, GUIStyle style,
            params GUILayoutOption[] options)
        {
            //通过反射获取到枚举列表
            FieldInfo[] fields = value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
            string[] popup = new string[fields.Length];

            //获取对应Comment属性，生成菜单列表
            int selected = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                CommentAttribute attr =
                    fields[i].GetCustomAttributes(typeof(CommentAttribute), false).FirstOrDefault() as CommentAttribute;
                popup[i] = attr != null ? attr.Comment : fields[i].Name;

                if (fields[i].GetValue(null).Equals(value))
                {
                    selected = i;
                }
            }

            //弹出菜单
            selected = Popup(label, selected, popup, ref changed, options);
            if (fields.Length > 0)
            {
                value = (Enum) fields[selected].GetValue(null);
            }

            return value;
        }

        public static void ShowMenu(string content0, GenericMenu.MenuFunction func0,
            string content1 = null, GenericMenu.MenuFunction func1 = null,
            string content2 = null, GenericMenu.MenuFunction func2 = null,
            string content3 = null, GenericMenu.MenuFunction func3 = null)
        {
            var menu = new GenericMenu();
            if (content0 != null)
            {
                menu.AddItem(new GUIContent
                {
                    text = content0
                }, false, func0);
            }

            if (content1 != null)
            {
                menu.AddItem(new GUIContent
                {
                    text = content1
                }, false, func1);
            }

            if (content2 != null)
            {
                menu.AddItem(new GUIContent
                {
                    text = content2
                }, false, func2);
            }

            if (content3 != null)
            {
                menu.AddItem(new GUIContent
                {
                    text = content3
                }, false, func3);
            }

            menu.ShowAsContext();
        }

        public static bool HelpBtn()
        {
            return GUILayout.Button("?", GUILayout.ExpandWidth(false));
        }

        public static bool Btn_TextLeft(string text, int width)
        {
            bool b = false;
            var align = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            GUILayoutOption option;
            if (width < 0) option = null;
            else if (width == 0) option = GUILayout.ExpandWidth(false);
            else option = GUILayout.Width(width);

            if (GUILayout.Button(text, option))
            {
                b = true;
            }

            GUI.skin.button.alignment = align;
            return b;
        }

        public static void Box_TextLeft(string text, params GUILayoutOption[] options)
        {
            var align = GUI.skin.box.alignment;
            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUILayout.Box(text, options);
            GUI.skin.box.alignment = align;
        }

        public static IDisposable HandleColor(Color color)
        {
            return new GUIHandleColor(color);
        }

        public static IDisposable Indent()
        {
            return new GUIEditorIndent();
        }

        public static IDisposable LableWidth(float width)
        {
            return new GUILabelWidth(width);
        }

        public static IDisposable LayoutHorizontal()
        {
            return new GUILayoutHorizontal();
        }

        public static IDisposable LayoutHorizontal(GUIStyle style)
        {
            return new GUILayoutHorizontal(style);
        }

        public static IDisposable LayoutHorizontal(params GUILayoutOption[] options)
        {
            return new GUILayoutHorizontal(options);
        }

        public static IDisposable LayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            return new GUILayoutHorizontal(style, options);
        }

        public static IDisposable LayoutVertical(params GUILayoutOption[] options)
        {
            return new GUILayoutVertical(options);
        }

        public static IDisposable LayoutVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            return new GUILayoutVertical(style, options);
        }

        public static IDisposable LayoutVertical(GUIStyle style)
        {
            return new GUILayoutVertical(style);
        }

        public static IDisposable Scroll(ref Vector2 scroll)
        {
            return new GUIScroll(ref scroll);
        }

        public static IDisposable Scroll(ref Vector2 scroll, params GUILayoutOption[] options)
        {
            return new GUIScroll(ref scroll, options);
        }

        public static IDisposable Scroll(ref Vector2 scroll, GUIStyle style, params GUILayoutOption[] options)
        {
            return new GUIScroll(ref scroll, style, options);
        }

        public static IDisposable TextAlign(GUIStyle style, TextAnchor anchor)
        {
            return new GUITextAlign(style, anchor);
        }
#endif
    }
}