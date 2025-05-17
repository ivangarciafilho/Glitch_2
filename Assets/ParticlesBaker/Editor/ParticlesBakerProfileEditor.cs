using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticlesBakerProfile))]
public class ParticlesBakerProfileEditor : Editor
{
    ParticlesBakerProfile profile;

    SerializedProperty hierarchyOptions_SO;
    SerializedProperty renderingOptions_SO;
    SerializedProperty exportPath_SO;
    SerializedProperty fixedPath_SO;
    SerializedProperty mergeMeshesWithSimilarMaterials_SO;

    private void OnEnable()
    {
        profile = target as ParticlesBakerProfile;

        hierarchyOptions_SO = serializedObject.FindProperty("hierarchyOptions");
        renderingOptions_SO = serializedObject.FindProperty("renderingOptions");
        exportPath_SO = serializedObject.FindProperty("exportPath");
        fixedPath_SO = serializedObject.FindProperty("fixedPath");
        mergeMeshesWithSimilarMaterials_SO = serializedObject.FindProperty("mergeMeshesWithSimilarMaterials");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        titleStyle.fontSize = 16;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(5);
        GUILayout.Label("Particles Baker Profile", titleStyle, GUILayout.ExpandWidth(true));
        GUILayout.Space(5);

        if(!profile.mainProfile)
        {
            if(GUILayout.Button("Set as Main Profile"))
            {
                var currentMainProfile = GetDefault();
                currentMainProfile.mainProfile = false;

                profile.mainProfile = true;
            }
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(7);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Space(2);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(hierarchyOptions_SO);
        EditorGUILayout.PropertyField(renderingOptions_SO);
        EditorGUILayout.PropertyField(mergeMeshesWithSimilarMaterials_SO);

        EditorGUILayout.EndVertical();

        GUILayout.Space(5);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(exportPath_SO);
        if (profile.exportPath == ParticlesBakerSettingsExportPath.ExportToFixedPath)
             EditorGUILayout.PropertyField(fixedPath_SO);

        EditorGUILayout.EndVertical();
        GUILayout.Space(2);

        EditorGUILayout.EndVertical();

        //
        ParticlesBakerProfile[] profileArr = EditorUtils.GetAllInstances<ParticlesBakerProfile>();
        if (profileArr.Length > 0)
        {
            bool allFalse = true;
            int amountOfTrue = 0;       

            foreach(var p in profileArr)
            {
                if(p.mainProfile)
                {
                    allFalse = false;
                    amountOfTrue++;
                }

            }

            if(amountOfTrue > 1)
            {
                foreach (var p in profileArr)
                    p.mainProfile = false;

                profileArr[0].mainProfile = true;
            }

            if(allFalse)
            {
                profileArr[0].mainProfile = true;   
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static ParticlesBakerProfile GetDefault()
    {
        ParticlesBakerProfile profile = null;
        ParticlesBakerProfile[] profileArr = EditorUtils.GetAllInstances<ParticlesBakerProfile>();
        if (profileArr.Length > 0)
        {
            foreach (var p in profileArr)
            {
                if (p.mainProfile)
                {
                    profile = p;
                    break;
                }
            }
        }

        if(profile == null)
        {
            if(profileArr.Length > 0)
            {
                profileArr[0].mainProfile = true;
                profile = profileArr[0];
            }
        }

        return profile;
    }
}
