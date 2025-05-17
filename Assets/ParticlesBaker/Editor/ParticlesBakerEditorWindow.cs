using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParticlesBakerEditorWindow : EditorWindow
{
    SerializedObject serializedObject;

    public GameObject[] items;

    [MenuItem("Window/Particles Baker Tools")]
    static void Init()
    {
        ParticlesBakerEditorWindow window = (ParticlesBakerEditorWindow)EditorWindow.GetWindow(typeof(ParticlesBakerEditorWindow));
        window.Show();
    }

    private void OnEnable()
    {
        ScriptableObject target = this;
        serializedObject = new SerializedObject(target);
    }

    void OnGUI()
    {
        serializedObject.Update();

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        titleStyle.fontSize = 18;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5);
        GUILayout.Label("Particles Baker Tools", titleStyle, GUILayout.ExpandWidth(true));
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
        GUILayout.Space(7);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Space(5);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("Bake selected Particle Systems in the Scene");
        if (GUILayout.Button("Bake Selected"))
        {
            BakeGameObjects(Selection.gameObjects);
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Bake list of Particle Systems");

        SerializedProperty itemsProperty = serializedObject.FindProperty("items");

        EditorGUILayout.PropertyField(itemsProperty, true);

        if (GUILayout.Button("Bake List"))
        {
            BakeGameObjects(items);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();

 
        serializedObject.ApplyModifiedProperties(); // Remember to apply modified properties
    }

    void BakeGameObjects(GameObject[] list)
    {
        foreach (var objSelected in list)
        {
            if (objSelected == null) continue;

            ParticleSystem ps = null;
            if (objSelected.TryGetComponent(out ps))
            {
                ParticlesBakerContextMenu.GenerateFromContext(ps);
            }
        }
    }
}
