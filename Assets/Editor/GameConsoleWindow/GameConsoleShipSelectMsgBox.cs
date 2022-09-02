using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit
{
    public class GameConsoleShipSelectMsgBox : EditorWindow
    {
        public static GameConsoleShipSelectMsgBox window;

        private ItemConfig selectItem;
        private Action<ItemConfig> chooseCallback;
        private List<ItemConfig> shipItemList = new List<ItemConfig>();
        private Vector2 scrollPos;

        public static void ShowMsgBox(ItemConfig item, Action<ItemConfig> chooseCallback)
        {
            if (window == null)
            {
                window = GetWindow<GameConsoleShipSelectMsgBox>("选择");
            }

            window.Init(item, chooseCallback);
            window.ShowUtility();
        }

        public void Init(ItemConfig item, Action<ItemConfig> chooseCallback)
        {
            selectItem = item;
            this.chooseCallback = chooseCallback;
            LoadConfigs();
        }

        private void OnGUI()
        {
            using (GUIUtil.Scroll(ref scrollPos, GUILayout.ExpandHeight(true)))
            {
                //划分成3列
                using (GUIUtil.LayoutHorizontal())
                {
                    int index = 0;
                    using (GUIUtil.LayoutVertical())
                    {
                        OnGUIList(shipItemList, 0);
                    }

                    using (GUIUtil.LayoutVertical())
                    {
                        OnGUIList(shipItemList, 1);
                    }

                    using (GUIUtil.LayoutVertical())
                    {
                        OnGUIList(shipItemList, 2);
                    }
                }
            }
        }

        private void OnGUIList(List<ItemConfig> list, int start)
        {
            for (int i = start; i < list.Count; i += 3)
            {
                var conf = list[i];
                IDisposable dis = null;
                if (conf == selectItem)
                {
                    dis = GUIUtil.Color(Color.green);
                }

                var align = GUI.skin.button.alignment;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                if (GUILayout.Button(conf.ToString(), GUILayout.Width(180)))
                {
                    selectItem = list[i];
                    chooseCallback(selectItem);
                    Close();
                }

                GUI.skin.button.alignment = align;

                dis?.Dispose();
            }
        }

        private void LoadConfigs()
        {
            if (shipItemList.Count > 0)
            {
                return;
            }

            //普通舰
            for(int i=0; i<10; i++)
            {
                var itemConf = new ItemConfig();
                itemConf.name = "test ship " + i;
                itemConf.gid = i;
                itemConf.lv = i % 6;
                shipItemList.Add(itemConf);
            }

            shipItemList.Sort((i1, i2) =>
            {
                if (i1.lv != i2.lv)
                {
                    return i1.lv.CompareTo(i2.lv);
                }

                return i1.shipScaleType.CompareTo(i2.shipScaleType);
            });
        }
    }
}