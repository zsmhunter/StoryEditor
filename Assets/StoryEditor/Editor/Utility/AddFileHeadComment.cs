// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-06
// Description :自动添加文件头，读取项目组的CoderName.txt文件
// **********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace StoryEditor
{
    public class AddFileHeadComment : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// 把自己名字写在这里面，自动添加注释
        /// </summary>
        public static readonly string CoderPath = EditorHelper.FormatFilePath(EditorHelper.ProjectPath + "/CoderName.txt");
        private static string m_coderName = null;
        public static string CoderName
        {
            get
            {
                if (m_coderName == null)
                {
                    m_coderName = "修改CoderName中的名字";
                    if (File.Exists(CoderPath))
                    {
                        m_coderName = File.ReadAllText(CoderPath);
                    }
                }
                return m_coderName;
            }
        }
        public static void OnWillCreateAsset(string newFileMeta)
        {
            if (!StoryEditorSetting.Instance.AutoAddComment)
                return;
            
            string newFilePath = newFileMeta.Replace(".meta", "");

            string fileExt = Path.GetExtension(newFilePath);

            if (fileExt != ".cs")
            {
                return;
            }
            newFilePath = EditorHelper.FormatFilePath(newFilePath);
            //非此目录的代码不自动添加注释
            if (!newFilePath.Contains("StoryEditor"))
                return;

            //注意，Application.datapath会根据使用平台不同而不同 

            string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;

            string scriptContent = File.ReadAllText(realPath);
            scriptContent =
$@"// **********************************************************************
// Author Name :{CoderName}
// Create Time :{System.DateTime.Now:yyyy-MM-dd}
// Description :
// **********************************************************************
" + scriptContent;

            File.WriteAllText(realPath, scriptContent);

        }

    }
}
