using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StoryEditor
{
    public class ModelWindow : EditorWindow
    {

        class FlodOut
        {
            //暂时测试用
            public static bool UseCameraImage = false; //使用那种方式
        }
        private Editor m_previewEditor;
        public double nowTime;
        public const float fpsDelta = 0.0167f;
        private Camera modelCamera = null;
        private GameObject modelContianer = null;

        private void Clear()
        {
            if (m_previewEditor != null)
            {
                DestroyImmediate(m_previewEditor);
            }
            m_previewEditor = null;
            modelCamera = null;
            modelContianer = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow(typeof(ModelWindow)) as ModelWindow;
            window.Show();
        }

        private RenderTexture m_modelRenderTexture;

        private void CreateModelRenderTexture(int width , int height)
        {
            if (m_modelRenderTexture != null)
                return;
            m_modelRenderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            m_modelRenderTexture.Create();
        }

        //private void OnInspectorUpdate()
        //{
        //    this.Repaint();
        //}

        void OnEnable()
        {
            nowTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Clear();
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode || state == UnityEditor.PlayModeStateChange.EnteredPlayMode)
            {
                Clear();
            }
        }

        void OnEditorUpdate()
        {
            if (EditorApplication.isCompiling || !EditorApplication.isPlaying)
            {
                return;
            }
            //if (EditorApplication.isPlaying)
            //    Time.timeScale = 0;
            float delta = (float)(EditorApplication.timeSinceStartup - nowTime);
            if (delta < fpsDelta)
                return;
            nowTime = EditorApplication.timeSinceStartup;
            this.Repaint();
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("必须运行游戏才能使用");
                return;
            }
            var modelRect = Rect.MinMaxRect(50, 50, 500, 500);
            if (modelCamera == null)
            {
                Scene modelScene = SceneManager.GetSceneByName("Model");
                if (modelScene == null)
                {
                    EditorGUILayout.LabelField("找不到模型场景Model");
                    return;
                }
                modelCamera = EditorHelper.FindComponent<Camera>("RootScene/ModelCamera", modelScene);
                if (modelCamera == null)
                {
                    EditorGUILayout.LabelField("找不到模型场景摄像机ModelCamera对象");
                    return;
                }
                modelContianer = EditorHelper.FindGameObject("RootScene/Container", modelScene);
                if (modelContianer == null)
                {
                    EditorGUILayout.LabelField("找不到模型场景的Contianer对象");
                    return;
                }
                CreateModelRenderTexture((int)modelRect.width, (int)modelRect.height);
            }
            FlodOut.UseCameraImage = GUI.Toggle(new Rect(0, 0, 150, 40), FlodOut.UseCameraImage, "使用RenderTexture");
            if (FlodOut.UseCameraImage)
            {
                modelCamera.targetTexture = m_modelRenderTexture;
                GUI.Box(modelRect, string.Empty, Styles.shadowBorderStyle);
                GUI.DrawTexture(modelRect, m_modelRenderTexture);
            }
            else
            {
                if (m_previewEditor == null)
                {
                    // 第一个参数这里暂时没关系，因为编辑器没有取目标对象
                    m_previewEditor = Editor.CreateEditor(this, typeof(PreviewExampleInspector));
                }
                PreviewExampleInspector preview = (m_previewEditor as PreviewExampleInspector);
                preview.PreviewTarget = modelContianer;
                m_previewEditor.DrawPreview(modelRect);
            }
            

        }

    }
}
