// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-12
// Description :
// **********************************************************************
using UnityEditor;
using UnityEngine;

namespace StoryEditor
{
    [CustomEditor(typeof(PreviewExample))]
    public class PreviewExampleInspector : Editor
    {
        private PreviewRenderUtility m_PreviewUtility;
        public GameObject PreviewTarget;
        private GameObject m_previewInstance;
        // Ԥ������İ�Χ��
        private Bounds m_PreviewBounds;
        // Ԥ���ķ���
        private Vector2 m_PreviewDir = new Vector2(120f, -20f);
        public override bool HasPreviewGUI()
        {
            return true;
        }
        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("Ԥ��");
        }
        private void AddSingleGO(GameObject go)
        {

#if UNITY_2017_1_OR_NEWER
            m_PreviewUtility.AddSingleGO(go);

#endif
        }
        public override void OnPreviewSettings()
        {
            GUILayout.Label("�ı�", "preLabel");
            GUILayout.Button("��ť", "preButton");
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            InitPreview();
            // �������ҵ���ת
            m_PreviewDir = Drag2D(m_PreviewDir, r);
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            m_PreviewUtility.BeginPreview(r, background);
            Camera camera = m_PreviewUtility.camera;
            float num = Mathf.Max(m_PreviewBounds.extents.magnitude, 0.0001f);
            float num2 = num * 3.8f;
            Quaternion quaternion = Quaternion.Euler(-m_PreviewDir.y, -m_PreviewDir.x, 0f);
            Vector3 position = m_PreviewBounds.center - quaternion * (Vector3.forward * num2);
            camera.transform.position = position;
            camera.transform.rotation = quaternion;
            camera.nearClipPlane = num2 - num * 1.1f;
            camera.farClipPlane = num2 + num * 1.1f;
            SetEnabledRecursive(m_previewInstance, true);
            camera.Render();
            SetEnabledRecursive(m_previewInstance, false);
            m_PreviewUtility.EndAndDrawPreview(r);
        }
        private void InitPreview()
        {
            if (m_PreviewUtility == null)
            {
                // ����true������Ƴ����ڵ���Ϸ����
                m_PreviewUtility = new PreviewRenderUtility(true);
                // �����������һЩ����
                m_PreviewUtility.cameraFieldOfView = 60f;
                // ����Ԥ������Ϸ����
                Createm_PreviewInstances();
            }
            m_PreviewUtility.camera.cullingMask = 1 << kPreviewCullingLayer;
        }
        private void DestroyPreview()
        {
            if (m_PreviewUtility != null)
            {
                // ���Ҫ���������Ų���������ɵ�����������
                m_PreviewUtility.Cleanup();
                m_PreviewUtility = null;
            }
        }
        private void Destroym_PreviewInstances()
        {
            if (m_previewInstance != null)
            {
                DestroyImmediate(m_previewInstance);
            }
            m_previewInstance = null;
        }
        void OnDestroy()
        {
            Destroym_PreviewInstances();
            DestroyPreview();
        }
        private void Createm_PreviewInstances()
        {
            Destroym_PreviewInstances();
            // ����Ҫ���Ƶ���Ϸ����
            //m_PreviewInstance = GameObject.Find("ThirdPerson");
            if (PreviewTarget == null)
            {
                return;
            }
            // ʵ��������
            m_previewInstance = Instantiate(PreviewTarget, Vector3.zero, Quaternion.identity) as GameObject;
            m_previewInstance.transform.localScale = 100 * Vector3.one;
            // �ݹ��������ر�־�Ͳ�
            InitInstantiatedPreviewRecursive(m_previewInstance);
            // �رն�����Ⱦ
            SetEnabledRecursive(m_previewInstance, false);
            m_PreviewBounds = new Bounds(m_previewInstance.transform.position, Vector3.zero);
            GetRenderableBoundsRecurse(ref m_PreviewBounds, m_previewInstance);
            AddSingleGO(m_previewInstance);
        }
        public static void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
        {
            MeshRenderer meshRenderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (meshRenderer && meshFilter && meshFilter.sharedMesh)
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = meshRenderer.bounds;
                }
                else
                {
                    // ��չ��Χ�У����ð�Χ���ܹ�������һ����Χ��
                    bounds.Encapsulate(meshRenderer.bounds);
                }
            }
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if (skinnedMeshRenderer && skinnedMeshRenderer.sharedMesh)
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = skinnedMeshRenderer.bounds;
                }
                else
                {
                    bounds.Encapsulate(skinnedMeshRenderer.bounds);
                }
            }
            foreach (Transform transform in go.transform)
            {
                GetRenderableBoundsRecurse(ref bounds, transform.gameObject);
            }
        }
        public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && position.width > 50f)
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        // ���������϶�����Ļ��󣬴���һ�߳���
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        // ��ס Shift ���󣬿��Լӿ���ת
                        scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return scrollPosition;
        }
        // Ԥ��������Ļ��Ʋ� Camera.PreviewCullingLayer
        // Ϊ�˷�ֹ������ģ�����ͨ�������ȡ������ֱ��дֵ
        private const int kPreviewCullingLayer = 31;


        private static void InitInstantiatedPreviewRecursive(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;
            go.layer = kPreviewCullingLayer;
            foreach (Transform transform in go.transform)
            {
                InitInstantiatedPreviewRecursive(transform.gameObject);
            }
        }

        public static void SetEnabledRecursive(GameObject go, bool enabled)
        {
            return;
            Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                renderer.enabled = enabled;
            }
        }
    }
    //using UnityEngine;    //using UnityEditor;
    //public class PreviewExampleWindow : EditorWindow
    //    {
    //        private Editor m_Editor;
    //        [MenuItem("Window/PreviewExample")]
    //        static void ShowWindow()
    //        {
    //            GetWindow("PreviewExample");
    //        }
    //        private void OnDestroy()
    //        {
    //            if (m_Editor != null)
    //            {
    //                DestroyImmediate(m_Editor);
    //            }
    //            m_Editor = null;
    //        }
    //        void OnGUI()
    //        {
    //            if (m_Editor == null)
    //            {
    //                // ��һ������������ʱû��ϵ����Ϊ�༭��û��ȡĿ�����
    //                m_Editor = Editor.CreateEditor(this, typeof(PreviewExampleInspector));
    //            }
    //            m_Editor.DrawPreview(GUILayoutUtility.GetRect(300, 200));
    //        }
    //    }
}