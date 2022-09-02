using UnityEngine;

namespace GameKit
{
    public class GCWGuide : GCWFuncBase
    {
        public int guideGid;

        public GCWGuide()
        {
            name = "引导";
        }

        public override void OnGUI()
        {
            bool b = false;
            using (GUIUtil.LayoutHorizontal("box"))
            {
                GUILayout.Label("【跳过新手引导】", GUILayout.ExpandWidth(false));
                if (GUILayout.Button("发送", GUILayout.ExpandWidth(false)))
                {
                    Send("testFinishTutor");
                }
            }
        }
    }
}