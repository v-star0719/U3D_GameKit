using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kernel.Core;
using Kernel.Lang.Extension;
using UnityEngine;
using UnityEditor;

namespace Kernel.Editor
{
	public partial class EditorGUIUtil
    {
        private static GUIStyle styleBox;

        public static GUIStyle StyleBox
        {
            get
            {
                if (styleBox == null)
                {
                    styleBox = new GUIStyle(EditorStyles.helpBox);
                    styleBox.margin = new RectOffset(10, 10, 10, 10);
                }

                return styleBox;
            }
        }
    }
}