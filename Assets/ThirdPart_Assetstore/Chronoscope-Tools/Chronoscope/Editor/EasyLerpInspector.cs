using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ChronoscopeTools;

[CustomEditor(typeof(EasyLerp))]
public class EasyLerpInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("_name"), new GUIContent("Name"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("_profile"), new GUIContent("Profile"));
    }
}
