using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace StoryEditor
{
    public class MenuTool
    {
#if LENIU_STORYEDITOR_MASTER
        [MenuItem("Assets/临时测试按钮")]
        //用来临时测试一些代码
        public static void TempTest()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.Log(ResourcesManager.Instance);
                //GameObjectSource obj = null;
                //ResourcesManager.Instance.GetSourceAsync<GameObjectSource>(out obj, "Assets/PacakgeImport/DogKnight/Prefab/DogPolyart.prefab", (result) => { 
                //    Debug.Log($"loadComplete:{obj}, source:{obj.Source}");
                //});

            }
        }
#endif

        [MenuItem("剧情编辑器/基础配置")]
        public static void CreateNewStoryEditorSetting()
        {
            StoryEditorSetting.CreateNewStoryEditorSetting();
        }

        [MenuItem("剧情编辑器/打包工具")]
        public static void ShowBuildAssetWindow()
        {
            BuildAssetWindow.OpenWindow();
        }

        [MenuItem("剧情编辑器/模型编辑器")]
        public static void ShowModelWindow()
        {
            ModelWindow.OpenWindow();
        }

        [MenuItem("剧情编辑器/打开编辑场景")]
        public static void OpenAllScene()
        {
            string sceneDir = StoryEditorSetting.DirectorScenePath;
            EditorSceneManager.OpenScene(sceneDir + "/Director.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene(sceneDir + "/Model.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene(sceneDir + "/Effect.unity", OpenSceneMode.Additive);
        }
    }
}
