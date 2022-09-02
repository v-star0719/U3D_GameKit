using UnityEngine;

namespace GameKit
{
    public class GCWOthers : GCWFuncBase
    {
        public GCWOthers()
        {
            name = "其他";
        }

        public override void OnGUI()
        {
            bool b = false;
            using (GUIUtil.LayoutHorizontal("box"))
            {
                GUILayout.Label("【登录测试】", GUILayout.ExpandWidth(false));
                if (GUILayout.Button("发送", GUILayout.ExpandWidth(false)))
                {
                    Send("testLogin");
                }
            }
        }
    }
}