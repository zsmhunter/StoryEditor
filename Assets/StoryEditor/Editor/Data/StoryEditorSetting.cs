// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-06
// Description :���������ļ�
// **********************************************************************
using UnityEngine;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector;

namespace StoryEditor
{
    public class StoryEditorSetting : ScriptableObject
    {
        #region ��̬��ֵ
        [ShowInInspector]
        [LabelText("�༭���༭����")]
        public static readonly string DirectorScenePath = "Assets/StoryEditor/Scenes";
        /// <summary>
        /// �Ƿ�������״̬�£������Ҫ�ǻ�Ӱ����Դ����ģʽ����runtimeֱ�Ӽ���ԭ����Դ��
        /// 2�����̵�����ʱ�򿴿���ô���á��������ʱû�����ô�л���Ҫ�����ɰ�ť��
        /// </summary>
        public static readonly bool IsRunTime = false;
        [ShowInInspector]
        [ReadOnly]
        [LabelText("��ԴĿ¼")]
        public static string AssetbundlePath { get { return $"{EditorHelper.ProjectPath}/StandaloneWindows64"; } }
        /// <summary>
        /// ģ���ļ�
        /// </summary>
        public static string Resources_Root = "StoryResources";
        /// <summary>
        /// ģ���ļ�
        /// </summary>
        public static string Resources_Model = Resources_Root + "/Model";
        /// <summary>
        /// �����ļ�
        /// </summary>
        public static string Resources_Scene = Resources_Root + "/Scene";
        /// <summary>
        /// ��ЧĿ¼
        /// </summary>
        public static string Resources_Effect = Resources_Root + "/Effect";
        /// <summary>
        /// UIԤ��Ŀ¼
        /// </summary>
        public static string Resources_UI = Resources_Root + "/UI";
        /// <summary>
        /// ��ͼ��Դ
        /// </summary>
        public static string Resources_Texture = Resources_Root + "/Texture";

        [LabelText("�Զ���Ӵ���ע��ͷ")]
        public bool AutoAddComment = true;

        private static string CoderPath { get { return "���ֶ�ȡ�ļ�:" + AddFileHeadComment.CoderPath; } }

        /// <summary>
        /// �޸����������ע�͵�����
        /// </summary>
        [ShowInInspector]
        [PropertyTooltip("$CoderPath")]
        [LabelText("��ǰ����(����ע���ã�")]
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
                                    Debug.LogError("����2�������ļ���·����" + AssetDatabase.GUIDToAssetPath(f));
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
