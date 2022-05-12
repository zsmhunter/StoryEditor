// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-12
// Description :
// **********************************************************************
using UnityEditor;
using UnityEngine;

public static class InspectorTools
{

    static bool minimalisticLook = false;
    static bool mEndHorizontal = false;

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>
    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, InspectorTools.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, InspectorTools.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    /// <summary>
    /// Draw 18 pixel padding on the right-hand side. Used to align fields.
    /// </summary>

    static public void DrawPadding()
    {
        if (!InspectorTools.minimalisticLook)
            GUILayout.Space(18f);
    }

    /// <summary>
	/// Begin drawing the content area.
	/// </summary>
    
    static public void BeginContents() { BeginContents(InspectorTools.minimalisticLook); }

    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    static public void BeginContents(bool minimalistic)
    {
        if (!minimalistic)
        {
            mEndHorizontal = true;
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        }
        else
        {
            mEndHorizontal = false;
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (mEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(3f);
    }


    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(this SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(null, serializedObject, property, false, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(this SerializedObject serializedObject, string property, string label, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, false, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, false, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawPaddedProperty(this SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(null, serializedObject, property, true, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawPaddedProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, true, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
    {
        SerializedProperty sp = serializedObject.FindProperty(property);

        if (sp != null)
        {

            if (InspectorTools.minimalisticLook) padding = false;

            if (sp.isArray && sp.type != "string") DrawArray(serializedObject, property, label ?? property);
            else if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
            else EditorGUILayout.PropertyField(sp, options);

            if (padding)
            {
                InspectorTools.DrawPadding();
                EditorGUILayout.EndHorizontal();
            }
        }
        else Debug.LogWarning("Unable to find property " + property);
        return sp;
    }

    /// <summary>
    /// Helper function that draws an array property.
    /// </summary>

    static public void DrawArray(this SerializedObject obj, string property, string title)
    {
        SerializedProperty sp = obj.FindProperty(property + ".Array.size");

        if (sp != null && InspectorTools.DrawHeader(title))
        {
            InspectorTools.BeginContents();
            int size = sp.intValue;
            int newSize = EditorGUILayout.IntField("Size", size);
            if (newSize != size) obj.FindProperty(property + ".Array.size").intValue = newSize;

            EditorGUI.indentLevel = 1;

            for (int i = 0; i < newSize; i++)
            {
                SerializedProperty p = obj.FindProperty(string.Format("{0}.Array.data[{1}]", property, i));
                if (p != null) EditorGUILayout.PropertyField(p);
            }
            EditorGUI.indentLevel = 0;
            InspectorTools.EndContents();
        }
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public void DrawProperty(string label, SerializedProperty sp, params GUILayoutOption[] options)
    {
        DrawProperty(label, sp, true, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public void DrawProperty(string label, SerializedProperty sp, bool padding, params GUILayoutOption[] options)
    {
        if (sp != null)
        {
            if (padding) EditorGUILayout.BeginHorizontal();

            if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
            else EditorGUILayout.PropertyField(sp, options);

            if (padding)
            {
                InspectorTools.DrawPadding();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    static public void SetLabelWidth(float width)
    {
        EditorGUIUtility.labelWidth = width;
    }

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public float WrapAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    /// <summary>
    /// Create an undo point for the specified objects.
    /// </summary>

    static public void RegisterUndo(string name, params Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
            UnityEditor.Undo.RecordObjects(objects, name);

            foreach (Object obj in objects)
            {
                if (obj == null) continue;
                EditorUtility.SetDirty(obj);
            }
        }
    }
}
