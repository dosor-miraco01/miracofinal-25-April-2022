using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProblemItemRef))]
[CanEditMultipleObjects]
public class ProblemItemRefEditor : Editor
{
    public SerializedProperty id;
    public SerializedProperty title;
    public SerializedProperty icon;
    public SerializedProperty cases;
    //public SerializedProperty menuOrderNum;

    void OnEnable()
    {
        id = serializedObject.FindProperty("id");
        title = serializedObject.FindProperty("title");
        icon = serializedObject.FindProperty("icon");
        cases = serializedObject.FindProperty("cases");
        //menuOrderNum = serializedObject.FindProperty("menuOrderNum");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(id);
        //EditorGUILayout.PropertyField(menuOrderNum);
        EditorGUILayout.PropertyField(title);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(cases);
        serializedObject.ApplyModifiedProperties();
    }
}
