using UnityEngine;
using UnityEditor;
using ChronoscopeTools;

[CustomEditor(typeof(ChronoscopeDiscrete))]
public class ChronoscopeDiscreteInspector : ChronoscopeInspector
{
    /// <summary>
    /// Override repaint as smooth updates are not required here
    /// </summary>
    protected sealed override void SetRepaint()
    {
        // Do nothing
    }

    /// <summary>
    /// Override for drawing Marker and Header, as they use different value 
    /// </summary>
    protected sealed override void DisplayComponents()
    {
        DrawLines(serializedObject.FindProperty("_duration").floatValue);
        DrawMarker(serializedObject.FindProperty("_lastNormalisedEventTime").floatValue);
        DrawHeader(serializedObject.FindProperty("_lastNormalisedEventTime").floatValue, serializedObject.FindProperty("_name").stringValue);
        DrawFooter(serializedObject.FindProperty("_loop").boolValue, serializedObject.FindProperty("_runOnAwake").boolValue,
                   serializedObject.FindProperty("_running").boolValue, serializedObject.FindProperty("_pingPong").boolValue);
        DrawEvents(serializedObject.FindProperty("_discreteListenerTimes"));
    }

    /// <summary>
    /// Override as this is not applicable with this type of timer
    /// </summary>
    protected sealed override void ShowFixedUpdateOption()
    {
        // Do Nothing
    }

    /// <summary>
    /// Override to draw discrete progress bar rather than moving marker
    /// </summary>
    /// <param name="normalizedValue">Last triggered event time</param>
    protected sealed override void DrawMarker(float normalizedValue)
    {
        Rect markerBox = new Rect(dimensions.meterArea.xMin + 1, dimensions.meterMinorLineTop + 1, normalizedValue * (dimensions.meterArea.width - 1), dimensions.meterMinorLineHeight - 1);

        EditorGUI.DrawRect(markerBox, graphicColors.markerBackgroundColor);
    }
}
