using UnityEngine;
using UnityEditor;
using System.IO;

namespace StoryEditor
{
    public class BuildAssetWindow : EditorWindow
    {
        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow(typeof(BuildAssetWindow)) as BuildAssetWindow;
            window.Show();
        }

        class FlodOut
        {
            public static bool ShowTest = false; //�Ƿ���ʾ���԰�ť
            public static bool ShowAddressable = false; //�Ƿ���ʾAddressable��ť
        }

        void OnGUI()
        {

            GUI.skin.label.richText = true;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            EditorStyles.label.richText = true;
            EditorStyles.textField.wordWrap = true;
            EditorStyles.foldout.richText = true;

            FlodOut.ShowTest = EditorGUILayout.Foldout(FlodOut.ShowTest, "���԰�ť");
            if (FlodOut.ShowTest)
            {
                if (GUILayout.Button("��ʱ���ѡ����Դ��"))
                {
                    TestBuildSelect();
                }
            }

            FlodOut.ShowAddressable = EditorGUILayout.Foldout(FlodOut.ShowAddressable, "Addressable��Դ���");
            if (FlodOut.ShowAddressable)
            {
                if (GUILayout.Button("ɨ��Ŀ¼����Addressable��Դ�ṹ"))
                {
                    string[] dirs = Directory.GetDirectories(Application.dataPath + "/" + StoryEditorSetting.Resources_Root, "*.*", SearchOption.TopDirectoryOnly);
                    foreach(string dir in dirs)
                    {
                        BuildAddressable.LoopSetAllDirectorToAddress(EditorHelper.FormatFilePath(dir));

                    }
                }
            }
        }

        static void TestBuildSelect()
        {
            string tips = "��Դ�б�:";
            string outputPath = Path.GetFullPath(Application.dataPath + "/../BuildData");
            AssetBundleBuild[] builds = new AssetBundleBuild[Selection.instanceIDs.Length];
            for (int i = 0; i < builds.Length; i ++)
            {
                int guid = Selection.instanceIDs[i];
                AssetBundleBuild build = new AssetBundleBuild();
                string path = AssetDatabase.GetAssetPath(guid);
                build.assetBundleName = Path.GetFileNameWithoutExtension(path) + ".assetbundle";
                build.assetNames = new string[] { path };
                build.addressableNames = new string[] { path };
                tips += path + "\n";
                builds[i] = build;
            }
            if (EditorUtility.DisplayDialog("ȷ�����һ����Դ��", tips, "ȷ��", "ȡ��"))
            {
                EditorHelper.CreateDirectory(outputPath);
                BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
                EditorHelper.OpenFolder(outputPath);
            }
        }
    }
}
