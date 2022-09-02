using UnityEngine;

namespace GameKit
{
    public class GCWGame : GCWFuncBase
    {
        public GCWGame()
        {
            name = "游戏";
        }
        public override void OnGUI()
        {
            GUILayout.Label("【游戏】", GUILayout.ExpandWidth(false));
            //加速键
            using(GUIUtil.LayoutHorizontal("box"))
            {
                GUI.enabled = Application.isPlaying;
                using(GUIUtil.Enabled(Application.isPlaying))
                {
                    if(GUILayout.Button("x0", GUILayout.Width(30))) Time.timeScale = 0;
                    if(GUILayout.Button("x.05", GUILayout.Width(30))) Time.timeScale = 0.1f;
                    if(GUILayout.Button("x.1", GUILayout.Width(30))) Time.timeScale = 0.1f;
                    if(GUILayout.Button("x.2", GUILayout.Width(30))) Time.timeScale = 0.2f;
                    if(GUILayout.Button("x.3", GUILayout.Width(30))) Time.timeScale = 0.3f;
                    if(GUILayout.Button("x.5", GUILayout.Width(30))) Time.timeScale = 0.5f;
                    if(GUILayout.Button("x.8", GUILayout.Width(30))) Time.timeScale = 0.8f;
                    if(GUILayout.Button("x1", GUILayout.Width(30))) Time.timeScale = 1;
                    if(GUILayout.Button("x2", GUILayout.Width(30))) Time.timeScale = 2;
                    if(GUILayout.Button("x3", GUILayout.Width(30))) Time.timeScale = 3;
                    if(GUILayout.Button("x4", GUILayout.Width(30))) Time.timeScale = 4;
                    if(GUILayout.Button("x8", GUILayout.Width(30))) Time.timeScale = 8;
                    if(GUILayout.Button("x16", GUILayout.Width(30))) Time.timeScale = 16;
                    GUILayout.Label(string.Format("x{0:F2}", Time.timeScale, GUILayout.ExpandWidth(false)));
                }
            }

            //帧数
            using(GUIUtil.LayoutHorizontal("box"))
            {
                if(GUILayout.Button("10fp", GUILayout.Width(40)) && Application.isPlaying)
                    Application.targetFrameRate = 10;
                if(GUILayout.Button("30fp", GUILayout.Width(40)) && Application.isPlaying)
                    Application.targetFrameRate = 30;
                if(GUILayout.Button("60fp", GUILayout.Width(40)) && Application.isPlaying)
                    Application.targetFrameRate = 60;
                if(GUILayout.Button("~fp", GUILayout.Width(40)) && Application.isPlaying)
                    Application.targetFrameRate = -1;
                GUILayout.Label(string.Format("{0}fp", Application.targetFrameRate, GUILayout.ExpandWidth(false)));
            }
        }
    }
}