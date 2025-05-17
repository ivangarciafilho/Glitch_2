using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParticlesBakerRuntime))]
public class ParticlesBakerRuntimeEditor : Editor
{
    ParticlesBakerRuntime pbRuntime;

    private void OnEnable()
    {
        pbRuntime = (ParticlesBakerRuntime)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        pbRuntime.profile = ParticlesBakerProfileEditor.GetDefault();

        if(GUILayout.Button("Generate"))
        {
            pbRuntime.Generate();
        }
    }
}
