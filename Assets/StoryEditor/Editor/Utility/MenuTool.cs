using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace StoryEditor
{
    public class MenuTool
    {
#if LENIU_STORYEDITOR_MASTER
        [MenuItem("Assets/��ʱ���԰�ť")]
        //������ʱ����һЩ����
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

        [MenuItem("����༭��/��������")]
        public static void CreateNewStoryEditorSetting()
        {
            StoryEditorSetting.CreateNewStoryEditorSetting();
        }

        [MenuItem("����༭��/�������")]
        public static void ShowBuildAssetWindow()
        {
            BuildAssetWindow.OpenWindow();
        }

        [MenuItem("����༭��/ģ�ͱ༭��")]
        public static void ShowModelWindow()
        {
            ModelWindow.OpenWindow();
        }

        [MenuItem("����༭��/�򿪱༭����")]
        public static void OpenAllScene()
        {
            string sceneDir = StoryEditorSetting.DirectorScenePath;
            EditorSceneManager.OpenScene(sceneDir + "/Director.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene(sceneDir + "/Model.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene(sceneDir + "/Effect.unity", OpenSceneMode.Additive);
        }
    }
}
