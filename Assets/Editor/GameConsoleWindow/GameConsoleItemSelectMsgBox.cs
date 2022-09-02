using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameKit
{
    public enum EmItemType
    {
        [Comment("任意")] None = -1,
        [Comment("资源")] Resource = 0, //资源
        [Comment("战争")] War = 1, //战争
        [Comment("增益")] Gain = 2, //增益
        [Comment("其他")] Other = 3, //其他
        [Comment("旗舰蓝图")] FlagshipBluePrint = 4, //旗舰蓝图
        [Comment("旗舰装备图纸")] FlagshipEquipDrawing = 5, //旗舰装备图纸
        [Comment("旗舰装备材料")] FlagshipEquipMaterial = 6, //旗舰装备材料
    }

    public class GameConsoleItemSelectMsgBox : EditorWindow
    {
        public static List<ItemConfig> itemList = new List<ItemConfig>();

        public static GameConsoleItemSelectMsgBox window;

        private ItemConfig selectItem;
        private Action<ItemConfig> chooseCallback;
        private List<ItemConfig> displayItemList = new List<ItemConfig>();
        private Vector2 scrollPos;
        private EmItemType itemType = EmItemType.None;

        public static void ShowMsgBox(ItemConfig item, Action<ItemConfig> chooseCallback,
            EmItemType itemType = EmItemType.None)
        {
            if (window == null)
            {
                window = GetWindow<GameConsoleItemSelectMsgBox>("选择");
            }

            window.Init(item, chooseCallback, itemType);
            window.ShowUtility();
        }

        public void Init(ItemConfig item, Action<ItemConfig> chooseCallback, EmItemType itemType = EmItemType.None)
        {
            preColorItemGid = 0;
            colorIndex = 0;
            selectItem = item;
            this.chooseCallback = chooseCallback;
            this.itemType = itemType;
            LoadConfigs();

            displayItemList.Clear();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemType == EmItemType.None || itemType == itemList[i].itemType)
                {
                    displayItemList.Add(itemList[i]);
                }
            }
        }

        private void OnGUI()
        {
            colorIndex = 0;
            using (GUIUtil.Scroll(ref scrollPos, GUILayout.ExpandHeight(true)))
            {
                //划分成3列
                using (GUIUtil.LayoutHorizontal())
                {
                    int index = 0;
                    int count = displayItemList.Count / 3;
                    using (GUIUtil.LayoutVertical())
                    {
                        OnGUIList(displayItemList, 0, count);
                    }

                    using (GUIUtil.LayoutVertical())
                    {
                        OnGUIList(displayItemList, count, count);
                    }

                    using (GUIUtil.LayoutVertical())
                    {
                        OnGUIList(displayItemList, count + count, count);
                    }
                }
            }
        }

        private void OnGUIList(List<ItemConfig> list, int starIndex, int count)
        {
            for (int i = starIndex; i < starIndex + count; i++)
            {
                if (i >= list.Count)
                {
                    break;
                }

                var conf = list[i];
                IDisposable dis = null;
                if (conf == selectItem)
                {
                    dis = GUIUtil.Color(Color.green);
                }
                else
                {
                    dis = GUIUtil.ContentColor(GetColor(conf.gid));
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

                dis.Dispose();
            }
        }

        private void LoadConfigs()
        {
            if (itemList.Count > 0)
            {
                return;
            }

            //物品
            for (int i = 0; i < 10; i++)
            {
                var itemConf = new ItemConfig();
                itemConf.name = "test item " + i;
                itemConf.gid = i;
                itemConf.lv = i % 4;
                itemConf.itemType = (EmItemType)i;
                itemList.Add(itemConf);
            }

            itemList.Sort((i1, i2) =>
            {
                if (i1.gid != i2.gid)
                {
                    return i1.gid.CompareTo(i2.gid);
                }

                return i1.lv.CompareTo(i2.lv);
            });
        }

        private static Color[] colors =
        {
            new Color(227 / 255f, 207 / 255f, 87 / 255f), //香蕉色
            new Color(255 / 255f, 153 / 255f, 18 / 255f), //镉黄
            //new Color(210/255f, 105/255f, 30/255f),//巧克力色
            //new Color(51/255f, 161/255f, 201/255f),//孔雀蓝
            //new Color(50/255f, 205/255f, 50/255f),//酸橙绿
            //new Color(221/255f, 160/255f, 221/255f),//梅红色
        };

        private static int colorIndex = 0;
        private static int preColorItemGid;

        private static Color GetColor(int gid)
        {
            if (preColorItemGid != gid)
            {
                preColorItemGid = gid;
                colorIndex++;
            }

            if (colorIndex >= colors.Length)
            {
                colorIndex = 0;
            }

            return colors[colorIndex];
        }
    }

}