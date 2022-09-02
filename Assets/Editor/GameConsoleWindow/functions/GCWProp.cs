using UnityEngine;

namespace GameKit
{
    public class GCWProp : GCWFuncBase
    {
        public EmItemType itemType;
        public ItemConfig selectNormalItem;
        public int selectNormalItemCount = 0;

        public int itemGid;
        public int itemLevel;
        public int itemCount;

        public GCWProp()
        {
            name = "道具";
        }

        public override void OnGUI()
        {
            GUILayout.Label("【道具】", GUILayout.ExpandWidth(false));

            bool b = false;
            using (GUIUtil.LayoutHorizontal("box"))
            {
                GUILayout.Space(36);

                itemType = (EmItemType) GUIUtil.EnumPopupExCampact("类型", itemType, ref b, GUILayout.Width(100));
                GUILayout.Label(selectNormalItem != null ? selectNormalItem.ToString() : "未选择", GUILayout.Width(200));
                if (GUIUtil.Btn_TextLeft("···", 0))
                {
                    GameConsoleItemSelectMsgBox.ShowMsgBox(selectNormalItem, (item) => { selectNormalItem = item; },
                        itemType);
                }

                selectNormalItemCount =
                    GUIUtil.IntFieldCampact("count", selectNormalItemCount, ref b, GUILayout.Width(50));
                using (GUIUtil.Enabled(selectNormalItem != null))
                {
                    if (GUILayout.Button("发送", GUILayout.ExpandWidth(false)))
                    {
                        Send("testRewardItems", selectNormalItem.gid, selectNormalItem.lv, selectNormalItemCount);
                    }
                }
            }


            using (GUIUtil.LayoutHorizontal("box"))
            {
                GUILayout.Space(36);
                itemGid = GUIUtil.IntFieldCampact("gid", itemGid, ref b, GUILayout.Width(130));
                itemLevel = GUIUtil.IntFieldCampact("lv", itemLevel, ref b, GUILayout.Width(50));
                itemCount = GUIUtil.IntFieldCampact("count", itemCount, ref b, GUILayout.Width(50));
                if (GUILayout.Button("发送", GUILayout.ExpandWidth(false)))
                {
                    Send("testRewardItems", itemGid, itemLevel, itemCount);
                }
            }
        }
    }
}