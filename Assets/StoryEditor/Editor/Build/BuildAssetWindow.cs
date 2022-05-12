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
            public static bool ShowTest = false; //是否显示测试按钮
            public static bool ShowAddressable = false; //是否显示Addressable按钮
        }

        void OnGUI()
        {

            GUI.skin.label.richText = true;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            EditorStyles.label.richText = true;
            EditorStyles.textField.wordWrap = true;
            EditorStyles.foldout.richText = true;

            FlodOut.ShowTest = EditorGUILayout.Foldout(FlodOut.ShowTest, "测试按钮");
            if (FlodOut.ShowTest)
            {
                if (GUILayout.Button("临时打包选中资源的"))
                {
                    TestBuildSelect();
                }
            }

            FlodOut.ShowAddressable = EditorGUILayout.Foldout(FlodOut.ShowAddressable, "Addressable资源打包");
            if (FlodOut.ShowAddressable)
            {
                if (GUILayout.Button("扫描目录建立Addressable资源结构"))
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
            string tips = "资源列表:";
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
            if (EditorUtility.DisplayDialog("确定打包一下资源吗？", tips, "确定", "取消"))
            {
                EditorHelper.CreateDirectory(outputPath);
                BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
                EditorHelper.OpenFolder(outputPath);
            }
        }
    }
}
