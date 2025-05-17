using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using ChronoscopeTools;

[CustomEditor(typeof(ChronoscopeContinuous))]
public class ChronoscopeContinuousInspector : ChronoscopeInspector
{
    /// <summary>
    /// Override to properly cast target
    /// </summary>
    protected sealed override void SetRepaint()
    {
        ((ChronoscopeContinuous)target).WantRepaint += this.Repaint;
    }    

    /// <summary>
    /// Override to prevent displaying discrete event (there aren't any here)
    /// </summary>
    protected sealed override void DisplayComponents()
    {
        DrawLines(serializedObject.FindProperty("_duration").floatValue);
        DrawMarker(serializedObject.FindProperty("_normalisedValue").floatValue);
        DrawHeader(serializedObject.FindProperty("_normalisedValue").floatValue, serializedObject.FindProperty("_name").stringValue);
        DrawFooter(serializedObject.FindProperty("_loop").boolValue, serializedObject.FindProperty("_runOnAwake").boolValue,
                   serializedObject.FindProperty("_running").boolValue, serializedObject.FindProperty("_pingPong").boolValue);
    }

    /// <summary>
    /// Override to draw central line in red
    /// </summary>
    protected sealed override void DrawMeterArea()
    {
        EditorGUI.DrawRect(dimensions.centerMask, graphicColors.eventAreaBackground);
        EditorGUI.DrawRect(dimensions.centreLine, graphicColors.eventMarkerColor);
        EditorGUI.DrawRect(dimensions.eventAreaTopLine, graphicColors.majorColor);
        EditorGUI.DrawRect(dimensions.eventAreaBottomLine, graphicColors.majorColor);
    }
}
