using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GameKit
{
    public class GameConsoleWindow : EditorWindow
    {
        public static GameConsoleWindow window;
        public static string configFileName = "/GameKitGameConsoleWindow.xml";

        public static string SERVER1_API = "http://117.50.95.62:9827/api_test/testAny?fm=json&method=";
        public static string SERVER2_API = "http://117.50.95.62:9837/api_test/testAny?fm=json&method=";

        private List<int> itemGids = new List<int>();
        private List<string> itemNames = new List<string>();
        private List<GCWFuncBase> functionList = new List<GCWFuncBase>();

        private string playerName;
        private UnityWebRequest www;
        private string log = "";
        private UnityWebRequestAsyncOperation operation;

        private int resIndex = 0;
        private int resCount = 100000;

        private EmItemType itemType;
        private int itemGid = 0;
        private int itemCount = 1;
        private int itemLevel = 1;

        private int moveCityStellar;
        private int moveCityX;
        private int moveCityY;

        private int chapter;

        private int guideGid;

        private bool isInit;

        private int incidentGroup;

        private float refreshTimer;

        private string acAlliShortName; //联盟活动更改：联盟简称
        private int acType; //联盟活动更改：活动类型
        private int acTime; //联盟活动更改：开启时间，x分钟后

        private ItemConfig selectNormalItem;
        private ItemConfig selectShipItem;
        private int selectNormalItemCount = 1;
        private int selectShipItemCount = 100;
        private Vector2 scrollPos;

        [MenuItem("VStar/GameConsole", false)]
        public static void ShowWindow()
        {
            GetWindow<GameConsoleWindow>();
        }

        private void Init()
        {
            if(functionList.Count > 0 && functionList[0].name != null)
            {
                return;
            }

            functionList.Clear();
            if(!Load())
            {
                functionList.Add(new GCWGame());
                functionList.Add(new GCWProp());
                functionList.Add(new GCWGuide());
                functionList.Add(new GCWOthers());
            }

            foreach(var funcBase in functionList)
            {
                funcBase.Init(this);
            }
        }

        private bool Load()
        {
            if(File.Exists(GetConfigFilePath()))
            {
                FileStream fs = File.Open(GetConfigFilePath(), FileMode.Open);
                using(StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    XmlSerializer xz = new XmlSerializer(functionList.GetType());
                    functionList = (List<GCWFuncBase>)xz.Deserialize(sr);
                }

                return true;
            }

            return false;
        }

        private void Save()
        {
            using(StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(functionList.GetType());
                xz.Serialize(sw, functionList);
                File.WriteAllText(GetConfigFilePath(), sw.ToString());
            }
        }

        private string GetConfigFilePath()
        {
            //Debug.Log(Application.persistentDataPath + configFileName);
            return Application.persistentDataPath + configFileName;
        }

        private void Update()
        {
            Init();

            refreshTimer += Time.deltaTime;
            if (refreshTimer > 0.3f)
            {
                refreshTimer = 0f;
                Repaint();
            }

            var working = false;
            if (Application.isPlaying)
            {
                //var seceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                //if (seceneName == "Game" || seceneName == "TowerDefense" || seceneName == "City")
                {
                    working = true;
                }
            }

            if (operation != null)
            {
                if (operation.isDone)
                {
                    if (www.isHttpError)
                    {
                        log = www.error;
                    }
                    else
                    {
                        log = www.downloadHandler.text;
                    }

                    operation = null;
                    www = null;
                }
            }

            foreach(var funcBase in functionList)
            {
                funcBase.Update();
            }
        }

        private void OnGUI()
        {
            using (GUIUtil.Scroll(ref scrollPos))
            {
                using (GUIUtil.LayoutHorizontal("box"))
                {
                    GUILayout.Label("用户名：", GUILayout.ExpandWidth(false));
                    var name = GetUserName();
                    var serverId = GetServerId();
                    var alliShort = GetAllianceShortName();
                    var server = serverId > 0 ? serverId.ToString() : "未知";
                    GUILayout.Label($"服务器：{server}, 角色：{name}, 联盟：{alliShort}", GUILayout.ExpandWidth(false));
                }

                bool b = false;

                foreach (var func in functionList)
                {
                    var foldout = EditorPrefs.GetBool(func.GetType().Name, false);
                    var foldt = GUIUtil.Foldout(foldout, func.name, true);
                    if (foldt != foldout)
                    {
                        EditorPrefs.SetBool(func.GetType().Name, foldt);
                    }

                    if (foldout)
                    {
                        using (GUIUtil.LayoutVertical("box"))
                        {
                            func.OnGUI();
                        }
                    }
                }

                GUILayout.Label(log);
            }
        }

        public void Send(string method, object v1 = null, object v2 = null, object v3 = null, object v4 = null,
            object v5 = null, object v6 = null)
        {
            if(v6 == null)
            {
                v6 = GetUserName();
            }

            string info = "";
            WWWForm form = new WWWForm();
            if(v1 != null) { form.AddField("v1", v1.ToString()); info += v1.ToString() + "; "; }
            if(v2 != null) { form.AddField("v2", v2.ToString()); info += v2.ToString() + "; "; }
            if(v3 != null) { form.AddField("v3", v3.ToString()); info += v3.ToString() + "; "; }
            if(v4 != null) { form.AddField("v4", v4.ToString()); info += v4.ToString() + "; "; }
            if(v5 != null) { form.AddField("v5", v5.ToString()); info += v5.ToString() + "; "; }
            if(v6 != null) { form.AddField("v6", v6.ToString()); info += v6.ToString() + "; "; }
            Debug.Log("Request Form：" + info);

            string url = "www.baidu.com";

            if(!string.IsNullOrEmpty(url))
            {
                www = UnityWebRequest.Post(url + method, form);
                operation = www.SendWebRequest();
                Save();
            }
        }

        public string GetUserName()
        {
            return "";
        }

        public string GetAllianceShortName()
        {
            return "";
        }

        public void CallServerAPI(string funcName, params object[] p)
        {
        }

        private int GetServerId()
        {
            return 0;
        }
    }
}
