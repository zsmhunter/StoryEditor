// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-06
// Description :基础配置文件
// **********************************************************************
using UnityEngine;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector;

namespace StoryEditor
{
    public class StoryEditorSetting : ScriptableObject
    {
        #region 静态数值
        [ShowInInspector]
        [LabelText("编辑器编辑场景")]
        public static readonly string DirectorScenePath = "Assets/StoryEditor/Scenes";
        /// <summary>
        /// 是否在运行状态下，这个主要是会影响资源加载模式（非runtime直接加载原生资源）
        /// 2个工程的区别到时候看看怎么设置。。这个暂时没想好怎么切换，要不做成按钮？
        /// </summary>
        public static readonly bool IsRunTime = false;
        [ShowInInspector]
        [ReadOnly]
        [LabelText("资源目录")]
        public static string AssetbundlePath { get { return $"{EditorHelper.ProjectPath}/StandaloneWindows64"; } }
        /// <summary>
        /// 模型文件
        /// </summary>
        public static string Resources_Root = "StoryResources";
        /// <summary>
        /// 模型文件
        /// </summary>
        public static string Resources_Model = Resources_Root + "/Model";
        /// <summary>
        /// 场景文件
        /// </summary>
        public static string Resources_Scene = Resources_Root + "/Scene";
        /// <summary>
        /// 特效目录
        /// </summary>
        public static string Resources_Effect = Resources_Root + "/Effect";
        /// <summary>
        /// UI预设目录
        /// </summary>
        public static string Resources_UI = Resources_Root + "/UI";
        /// <summary>
        /// 贴图资源
        /// </summary>
        public static string Resources_Texture = Resources_Root + "/Texture";

        [LabelText("自动添加代码注释头")]
        public bool AutoAddComment = true;

        private static string CoderPath { get { return "名字读取文件:" + AddFileHeadComment.CoderPath; } }

        /// <summary>
        /// 修改这个，代码注释的名字
        /// </summary>
        [ShowInInspector]
        [PropertyTooltip("$CoderPath")]
        [LabelText("当前作者(代码注释用）")]
        public static string coder { get { return AddFileHeadComment.CoderName; } }
        #endregion


        public static StoryEditorSetting _instance = null;
        public static StoryEditorSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        string assetName = "t:"+ typeof(StoryEditorSetting);
                        string[] files = AssetDatabase.FindAssets(assetName);
                        if (files.Length > 0)
                        {
                            if (files.Length > 1)
                            {
                                foreach (string f in files)
                                {
                                    Debug.LogError("超过2个配置文件，路径：" + AssetDatabase.GUIDToAssetPath(f));
                                }
                            }                            
                            _instance = AssetDatabase.LoadAssetAtPath<StoryEditorSetting>(AssetDatabase.GUIDToAssetPath(files[0]));
                        }
                        if (_instance == null)
                        {
                            string settingPath = Application.dataPath + "/StoryEditor";
                            _instance = CreateSettingFile(settingPath);
                        }
                    }
                    catch(System.Exception ex)
                    {
                        Debug.LogError(ex);
                        _instance = ScriptableObject.CreateInstance<StoryEditorSetting>();
                    }
                    
                }
                return _instance;
            }
        }

        private static StoryEditorSetting CreateSettingFile(string pathName)
        {
            StoryEditorSetting settings = null;
            settings = ScriptableObject.CreateInstance<StoryEditorSetting>();
            AssetDatabase.CreateAsset(settings, pathName);
            return settings;
        }

        internal class DoCreateStoryEditorSetting : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var settings = StoryEditorSetting.CreateSettingFile(pathName);
                ProjectWindowUtil.ShowCreatedAsset(settings);
            }
        }

        public static void CreateNewStoryEditorSetting()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateStoryEditorSetting>(), "StoryEditorSetting.asset", null, null);
        }

    }
}
