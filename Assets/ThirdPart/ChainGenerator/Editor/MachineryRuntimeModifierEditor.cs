using System.Collections;
using System.Collections.Generic;
using ChainInGame;
using UnityEditor;
using UnityEngine;

namespace ChainEditor
{
    [CustomEditor(typeof(MachineryRuntimeModifier))]
    public class MachineryRuntimeModifierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MachineryRuntimeModifier runtimeModifier = (MachineryRuntimeModifier)target;
        
            if (GUILayout.Button("Change Speed at Runtime"))
            {
                runtimeModifier.ChangeSpeedAtRunTime(runtimeModifier.runTimeSpeed);
            }

            GUILayout.Space(10);
      
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Motion"))
            {
                runtimeModifier.StartMotion();
            }
        
            if (GUILayout.Button("Stop Motion"))
            {
                runtimeModifier.StopMotion();
            }
            EditorGUILayout.EndHorizontal();
        
        }
    }

}
