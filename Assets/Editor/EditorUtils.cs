using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameKit
{
    public class EditorUtils
    {
        [MenuItem("VStar/GetChildrenCountRecursive")]
        public static void GetChildrentCountRecrusive()
        {
            int childCount = GetChildCount(Selection.activeTransform);
            Debug.Log("deeply child count is " + childCount);
        }

        [MenuItem("VStar/TestIterateFolderAssets")]
        public static void TestIterateFolderAssets()
        {
            IterateAssetsInFolderOfSelectedAsset((filePath) => { Debug.Log(filePath); });
        }

        [MenuItem("VStar/GetChildrenCount")]
        public static void GetChildrentCount()
        {
            if (Selection.activeTransform != null)
            {
                Debug.Log("child count is " + Selection.activeTransform.childCount);
            }
        }

        [MenuItem("VStar/TestFindSpriteNameInPrefabs")]
        public static void FindSpriteNameInPrefabs()
        {
            IterateAssetsInFolderOfSelectedAsset(
                (filePath) =>
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                    if (go != null)
                    {
                        var sprites = go.GetComponentsInChildren<UISprite>(true);
                        for (int j = 0; j < sprites.Length; j++)
                        {
                            if (sprites[j].spriteName == "alli_currency")
                            {
                                string name = sprites[j].name;
                                var t = sprites[j].transform;
                                while (t.parent)
                                {
                                    name = t.parent.name + "/" + name;
                                    t = t.parent;
                                }

                                Debug.LogError(name);
                            }
                        }
                    }
                });
        }

        [MenuItem("VStar/AutoSetColliderByMesh")]
        public static void AutoSetColliderByMesh()
        {
            if(Selection.transforms.Length == 0)
            {
                return;
            }

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                var trans = Selection.transforms[i];
                BoxCollider collider = trans.GetComponentInChildren<BoxCollider>();
                if(collider == null)
                {
                    Debug.LogError(trans + "no collider");
                    return;
                }

                MeshRenderer renderer = trans.GetComponentInChildren<MeshRenderer>();
                if(renderer == null)
                {
                    Debug.LogError(trans + "no mesh renderer");
                    return;
                }

                collider.center = renderer.bounds.center - trans.position;
                collider.size = renderer.bounds.size;
            }
        }

        public static void IterateAssetsInFolderOfSelectedAsset(Action<string> action)
        {
            IterateAssetsInFolder(GetSelectedFolder(), action);
        }

        /// <summary>
        /// return Assets/XXX...
        /// </summary>
        public static string GetSelectedFolder()
        {
            if (Selection.assetGUIDs.Length == 0)
            {
                return "Assets/";
            }

            //Selection.activeObject是上次选择的asset，和inspector里看到的一样
            //Selection.GetFiltered同上
            var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            if (System.IO.Directory.Exists(path))
            {
                return path;
            }
            else
            {
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">示例：Assets/Test/Test.txt</param>
        /// <param name="action">传入的路径包含Assets/。比如Assets/Test/Test.txt</param>
        /// <param name="opt"></param>
        public static void IterateAssetsInFolder(string path, Action<string> action, SearchOption opt = SearchOption.AllDirectories)
        {
            path = path.Substring("Assets".Length);
            var paths = Directory.GetFiles(Application.dataPath + path, "*", opt);
            float f = 0;
            for(int i = 0; i<paths.Length; i++)
            {
                f++;
                string s = paths[i].Replace(@"\", @"/");
                if(s.EndsWith(".meta"))
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("", i + "/" + paths.Length, f/paths.Length);
                var index = s.IndexOf("Assets/");
                action(s.Substring(index));
            }
            EditorUtility.ClearProgressBar();
        }

        private static int GetChildCount(Transform t)
        {
            if (t == null) return 0;
            int n = t.childCount;
            for (int i = 0; i < t.childCount; i++)
            {
                n += GetChildCount(t.GetChild(i));
            }

            return n;
        }

        [MenuItem("VStar/SaveAsPrefab")]
        public static void SaveAsPrefab()
        {
            foreach (var camera in Camera.allCameras)
            {
                Debug.LogError(GameKit.GameUtilities.GetTransformPath(camera.transform));
            }

            //if (Selection.activeGameObject == null)
            //{
            //    Debug.LogError("请选中要处理的prefab");
            //    return;
            //}

            //string path = "Assets/HotRes/Prefabs/" + Selection.activeGameObject.name + ".prefab";

            //PrefabUtility.SaveAsPrefabAsset(Selection.activeGameObject, path);
            //Debug.Log("save success " + path);
        }

        private static string GetNodeFullName(Transform transform)
        {
            string str = transform.name;
            while (transform.parent != null)
            {
                str = transform.parent.name + "/" + str;
                transform = transform.parent;
            }

            return str;
        }

        private static void ReplaceStringInName(Transform transform, string find, string replace)
        {
            var newName = transform.name.Replace(find, replace);
            if (newName != transform.name)
            {
                Debug.LogFormat("{0} --> {1}", transform.name, newName);
                transform.name = newName;
            }

            int n = transform.childCount;
            for (int i = 0; i < n; i++)
            {
                var t = transform.GetChild(i);
                ReplaceStringInName(t, find, replace);
            }
        }

        private static void IterateTree(Transform transform, Action<Transform> action)
        {
            if (transform != null)
            {
                action(transform);
                return;
            }

            int n = transform.childCount;
            for (int i = 0; i < n; i++)
            {
                var t = transform.GetChild(i);
                IterateTree(t, action);
            }
        }


        public static string GetEnumCommentText(Enum em)
        {
            Type type = em.GetType();
            var field = type.GetField(em.ToString(), BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            if(field == null)
            {
                return em.ToString();
            }

            var ca = field.GetCustomAttribute<CommentAttribute>();
            if(ca != null)
            {
                return ca.Comment;
            }

            return em.ToString();
        }
    }
}
