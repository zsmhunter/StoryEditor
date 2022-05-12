// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-06
// Description :公用文件头
// **********************************************************************
using System.IO;
using UnityEngine;

namespace StoryEditor
{
    public class EditorHelper
    {
        public static string CreateParentDirectory(string path, bool isClearDir = false)
        {
            string dirPath = Path.GetDirectoryName(path);
            CreateDirectory(dirPath, isClearDir);
            return path;
        }

        public static string CreateDirectory(string path,bool isClearDir = false)
        {
            if (isClearDir)
                DeleteDirectory(path, true);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string DeleteFile(string path)
        {
            if (File.Exists(path)) File.Delete(path);
            return path;
        }

        public static string DeleteDirectory(string path, bool recursive = true)
        {
            if (!Directory.Exists(path))
                return path;
            Directory.Delete(path, true);
            return path;
        }

        public static void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start(path);
        }

        /// <summary>
        /// 把\转成成/斜杠
        /// </summary>
        public static string FormatFilePath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string ProjectPath
        {
            get
            {
                return Path.GetFullPath(Application.dataPath + "/..");
            }
        }

        public static T FindComponent<T>(string path, UnityEngine.SceneManagement.Scene scene) where T:Component
        {
            return FindComponent<T>(path, scene.GetRootGameObjects());
        }

        public static T FindComponent<T>(string path, GameObject[] rootObjects) where T : Component
        {
            GameObject obj = FindGameObject(path, rootObjects);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            }
            return null;
        }

        public static GameObject FindGameObject(string path, UnityEngine.SceneManagement.Scene scene)
        {
            return FindGameObject(path, scene.GetRootGameObjects());
        }

        public static GameObject FindGameObject(string path, GameObject[] rootObjects)
        {
            int rootIndex = path.IndexOf("/");
            string rootName = path;
            string subPath = null;
            if (rootIndex > 0)
            {
                rootName = path.Substring(0, rootIndex);
                subPath = path.Substring(rootIndex + 1, path.Length - rootIndex - 1);
            }
            foreach (GameObject obj in rootObjects)
            {
                if (obj.name == rootName)
                {
                    if (null == subPath)
                        return obj;
                    Transform transform = obj.transform.Find(subPath);
                    if (null == transform)
                        return null;
                    return transform.gameObject;
                }
            }
            return null;
        }
    }
}
