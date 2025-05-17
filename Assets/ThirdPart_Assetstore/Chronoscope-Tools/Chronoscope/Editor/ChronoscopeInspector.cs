using UnityEngine;
using UnityEditor;
using ChronoscopeTools;
using ChronoscopeToolsInternal;

[CustomEditor(typeof(Chronoscope))]
public class ChronoscopeInspector : Editor
{
    protected ChronoscopeColorSet graphicColors = new ChronoscopeColorSet(); 
    protected ChronoscopeDimensionSet dimensions = new ChronoscopeDimensionSet();
    protected GUIStyle graphicTextStyle;

    protected Rect oldPosition = new Rect(0, 0, 0, 0);    

    void OnEnable()
    {
        SetRepaint();  
    } 
    
    /// <summary>
    /// Used for smoothly drawing the timer graphic, only called in editor mode
    /// </summary>
    protected virtual void SetRepaint()
    {
        ((Chronoscope)target).WantRepaint += this.Repaint;
    }

    public override void OnInspectorGUI()
    {
        // Not sure why this needs to be here exactly...?
        graphicTextStyle = new GUIStyle(GUI.skin.label);
        serializedObject.Update();        
        ShowGUI();
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draws the inspector
    /// </summary>
    protected void ShowGUI()
    {
        float contextWidth = EditorGUIUtility.fieldWidth;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Separator();

        DrawGraphic(contextWidth);
        ShowTimerOptions(contextWidth);

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Dispalys the timer options foldout in the inspector
    /// </summary>
    /// <param name="contextWidth">The width of the available inspector space</param>
    protected void ShowTimerOptions(float contextWidth)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Separator();
        serializedObject.FindProperty("_showSettings").boolValue = EditorGUILayout.Foldout(serializedObject.FindProperty("_showSettings").boolValue, "Settings", true);
        if (serializedObject.FindProperty("_showSettings").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_name"), new GUIContent("Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_duration"), new GUIContent("Duration"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_loop"), new GUIContent("Loop"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pingPong"), new GUIContent("Ping-Pong"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_optimise"), new GUIContent("Optimise"));
            if (serializedObject.FindProperty("_optimise").boolValue)
            {
                EditorGUILayout.HelpBox("Changes made to Loop and Ping-Pong during play mode will have no effect while Optimise is enabled.", MessageType.Info);
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_runOnAwake"), new GUIContent("Run On Awake"));
            ShowFixedUpdateOption();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_source"), new GUIContent("Source"));            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorScheme"), new GUIContent("Color Scheme"));
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
    }    

    /// <summary>
    /// Show fixed override option, some child classes may wish to override this as it will not always be applicable
    /// </summary>
    protected virtual void ShowFixedUpdateOption()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_fixedUpdate"), new GUIContent("Fixed Update"));
    }

    /// <summary>
    /// Draws the timer graphic in the inspector
    /// </summary>
    /// <param name="contextWidth">The width if the available inspector space</param>
    protected virtual void DrawGraphic(float contextWidth)
    {
        Rect position = GetGraphicRect(contextWidth);

        DetermineMajorDivisions(position.width);
        if (oldPosition != position) CalcValues(position);
                
        GetColors(serializedObject);        

        EditorGUI.DrawRect(position, graphicColors.areaColor);
        DisplayComponents();
        DrawBorder();
    }

    /// <summary>
    /// Reserves a Rect for the timer graphic
    /// </summary>
    /// <param name="contextWidth">The width of the available inspector space</param>
    /// <returns>A Rect for the timer graphic</returns>
    protected Rect GetGraphicRect(float contextWidth)
    {
        Rect position = GUILayoutUtility.GetRect(contextWidth, ChronoscopeCommon.graphicHeight);
        position.x += GUI.skin.box.margin.left;
        position.width -= (GUI.skin.box.margin.left + GUI.skin.box.margin.right);
        return position;
    }

    /// <summary>
    /// Displays the individual components of the graphic
    /// </summary>
    protected virtual void DisplayComponents()
    {
        float normalizedValue = serializedObject.FindProperty("_normalisedValue").floatValue;
        DrawLines(serializedObject.FindProperty("_duration").floatValue);
        DrawMarker(normalizedValue);
        DrawHeader(normalizedValue, serializedObject.FindProperty("_name").stringValue);
        DrawFooter(serializedObject.FindProperty("_loop").boolValue, serializedObject.FindProperty("_runOnAwake").boolValue,
                   serializedObject.FindProperty("_running").boolValue, serializedObject.FindProperty("_pingPong").boolValue);
        DrawEvents(serializedObject.FindProperty("_discreteListenerTimes"));
    }

    #region DrawingFunctions
    /// <summary>
    /// Draws a border around the graphic
    /// </summary>
    protected void DrawBorder()
    {
        EditorGUI.DrawRect(dimensions.leftBorder, graphicColors.borderColor);
        EditorGUI.DrawRect(dimensions.rightBorder, graphicColors.borderColor);
        EditorGUI.DrawRect(dimensions.topBorder, graphicColors.borderColor);
        EditorGUI.DrawRect(dimensions.bottomBorder, graphicColors.borderColor);
    }

    /// <summary>
    /// Draws the graphic header
    /// </summary>
    /// <param name="normalizedValue">Timer percentage</param>
    /// <param name="name">Timer descriptive name</param>
    protected void DrawHeader(float normalizedValue, string name)
    {
        EditorGUI.DrawRect(dimensions.headerSeparator, graphicColors.headerColor);

        graphicTextStyle.normal.textColor = graphicColors.headerColor;
        graphicTextStyle.fontSize = ChronoscopeCommon.headerFontSize;
        graphicTextStyle.fontStyle = FontStyle.Bold;
        EditorGUI.LabelField(new Rect(dimensions.graphicArea.xMin, dimensions.headerTextTop, dimensions.graphicArea.width, dimensions.headerFooterHeight), string.Format("{0}", name), graphicTextStyle);
        graphicTextStyle.fontStyle = FontStyle.Normal;

        string timerPercentage = string.Format("{0}%", Mathf.RoundToInt(normalizedValue * 100));
        var textDimension = graphicTextStyle.CalcSize(new GUIContent(timerPercentage));
        EditorGUI.LabelField(new Rect(dimensions.graphicArea.xMin + dimensions.graphicArea.width - textDimension.x, dimensions.headerTextTop, dimensions.graphicArea.width, dimensions.headerFooterHeight),
                             timerPercentage, graphicTextStyle);
        graphicTextStyle.normal.textColor = Color.white;
    }

    /// <summary>
    /// Draws the graphic footer
    /// </summary>
    /// <param name="loop">Timer loop setting</param>
    /// <param name="runOnAwake">Timer runOnAwake setting</param>
    /// <param name="running">Timer running flag</param>
    /// <param name="pingPong">Timer ping-pong setting</param>
    protected void DrawFooter(bool loop, bool runOnAwake, bool running, bool pingPong)
    {
        EditorGUI.DrawRect(dimensions.footerSeparator, graphicColors.headerColor);

        string runningText = running ? "RUNNING" : "STOPPED";
        string runOnAwakeText = runOnAwake ? "AWAKE / " : "";
        string loopText = loop ? "LOOP" : "ONCE";

        if (pingPong) loopText += " / P-P";

        string optionsText = string.Format("{0}{1}", runOnAwakeText, loopText);

        var textDimension = graphicTextStyle.CalcSize(new GUIContent(optionsText));

        graphicTextStyle.normal.textColor = graphicColors.headerColor;
        graphicTextStyle.fontSize = ChronoscopeCommon.headerFontSize;
        EditorGUI.LabelField(new Rect(dimensions.graphicArea.xMin, dimensions.footerTextTop, dimensions.graphicArea.width, 
                             dimensions.headerFooterHeight), string.Format("{0}", runningText), graphicTextStyle);
        EditorGUI.LabelField(new Rect(dimensions.graphicArea.xMin + dimensions.graphicArea.width - textDimension.x, dimensions.footerTextTop, 
                                      textDimension.x, dimensions.headerFooterHeight), optionsText, graphicTextStyle);

        graphicTextStyle.normal.textColor = Color.white;
    }
    
    /// <summary>
    /// Determines how many major divisions there should be based on graphic width
    /// </summary>
    /// <param name="width">Width of graphic</param>
    private void DetermineMajorDivisions(float width)
    {
        if (width > ChronoscopeCommon.tickMarkThresholdWidth)
        {
            dimensions.meterDivisions = ChronoscopeCommon.largeMeterDivisions;
            dimensions.majorDivisionEvery = ChronoscopeCommon.largeMajorDivisions;
        }
        else
        {
            dimensions.meterDivisions = ChronoscopeCommon.smallMeterDivisions;
            dimensions.majorDivisionEvery = ChronoscopeCommon.smallMajorDivisions;
        }
    }

    /// <summary>
    /// Draws the lines of the graphic; header, footer, meter, time labels etc
    /// </summary>
    /// <param name="maxDuration">The duration of the timer</param>
    protected virtual void DrawLines(float maxDuration)
    {
        int percentCount = 0;
        Rect majorDash = new Rect(0.0f, dimensions.meterMajorLineTop, 1, dimensions.meterMajorLineHeight);
        Rect minorDash = new Rect(0.0f, dimensions.meterMinorLineTop, 1, dimensions.meterMinorLineHeight);
        Rect normalLabelPos = new Rect(0.0f, dimensions.meterNormalizedLabelTop, 100, ChronoscopeCommon.meterLabelHeight);
        Rect timeLabelPos = new Rect(0.0f, dimensions.meterMajorLineTop + dimensions.meterMajorLineHeight, 100, ChronoscopeCommon.meterLabelHeight);

        graphicTextStyle = new GUIStyle(GUI.skin.label);
        graphicTextStyle.normal.textColor = graphicColors.valueColor;
        graphicTextStyle.fontSize = ChronoscopeCommon.headerFontSize;

        for (int i = 0; i <= dimensions.meterDivisions; i++)
        {
            if (i % dimensions.majorDivisionEvery == 0)
            {
                DrawMajorDivision(i, percentCount, majorDash, normalLabelPos, timeLabelPos, maxDuration);
                percentCount++;
            }
            else
            {
                DrawMinorDivision(i, minorDash);
            }
        }

        DrawMeterArea();
    }

    /// <summary>
    /// Draws a major division tick with label
    /// </summary>
    /// <param name="index">The current division count</param>
    /// <param name="percentCount">The percent complete of the timer drawing</param>
    /// <param name="majorDash">The Rect containing the major dash dimensions</param>
    /// <param name="normalLabelPos">The Rect containing the normalized label dimensions</param>
    /// <param name="timeLabelPos">The Rect containing the actual time label dimensions</param>
    /// <param name="maxDuration">The duration of the timer</param>
    protected virtual void DrawMajorDivision(int index, int percentCount, Rect majorDash, Rect normalLabelPos, Rect timeLabelPos, float maxDuration)
    {
        majorDash.x = dimensions.meterArea.xMin + (index * dimensions.meterDivisionWidth);
        EditorGUI.DrawRect(majorDash, graphicColors.majorColor);

        // Draw Normalized Time Label
        int percent = Mathf.RoundToInt(100 * (((float)percentCount) / (dimensions.meterDivisions / dimensions.majorDivisionEvery)));
        string labelText = string.Format("{0}%", percent);
        var textDimensions = graphicTextStyle.CalcSize(new GUIContent(labelText));
        normalLabelPos.y = dimensions.meterMajorLineTop - (textDimensions.y);
        normalLabelPos.x = majorDash.x - (textDimensions.x / 2);
        EditorGUI.LabelField(normalLabelPos, labelText, graphicTextStyle);

        // Draw Actual Time Label
        float time = (((float)percent) / 100) * maxDuration;
        labelText = string.Format("{0:0.##}s", time);
        textDimensions = graphicTextStyle.CalcSize(new GUIContent(labelText));
        timeLabelPos.x = majorDash.x - (textDimensions.x / 2);
        EditorGUI.LabelField(timeLabelPos, labelText, graphicTextStyle);
    }

    /// <summary>
    /// Draws a minor division tick
    /// </summary>
    /// <param name="index">The current division count</param>
    /// <param name="minorDash">The Rect containing the minor dash dimensions</param>
    protected virtual void DrawMinorDivision(int index, Rect minorDash)
    {
        minorDash.x = dimensions.meterArea.xMin + (index * dimensions.meterDivisionWidth);
        EditorGUI.DrawRect(minorDash, graphicColors.minorColor);
    }

    /// <summary>
    /// Draws the central meter area
    /// </summary>
    protected virtual void DrawMeterArea()
    {
        EditorGUI.DrawRect(dimensions.centreLine, graphicColors.majorColor);
        EditorGUI.DrawRect(dimensions.centerMask, graphicColors.eventAreaBackground);
        EditorGUI.DrawRect(dimensions.eventAreaTopLine, graphicColors.majorColor);
        EditorGUI.DrawRect(dimensions.eventAreaBottomLine, graphicColors.majorColor);
    }

    /// <summary>
    /// Draws the current timer value marker
    /// </summary>
    /// <param name="normalizedValue">Normalized value of the timer</param>
    protected virtual void DrawMarker(float normalizedValue)
    {
        float markerCenter = dimensions.meterArea.xMin + (dimensions.meterArea.width * normalizedValue);
        Rect markerBox = new Rect(markerCenter - (dimensions.markerWidth / 2), dimensions.meterMinorLineTop + 1, dimensions.markerWidth, dimensions.meterMinorLineHeight - 1);
        Rect markerLine = new Rect(markerCenter, dimensions.meterMinorLineTop + 1, 1, dimensions.meterMinorLineHeight - 1);

        if (markerBox.xMin < dimensions.meterArea.xMin) markerBox.xMin = dimensions.meterArea.xMin;
        if (markerBox.xMax > (dimensions.meterArea.xMin + dimensions.meterArea.width)) markerBox.xMax = (dimensions.meterArea.xMin + dimensions.meterArea.width);

        EditorGUI.DrawRect(markerBox, graphicColors.markerBackgroundColor);
        EditorGUI.DrawRect(markerLine, graphicColors.markerLineColor);
    }

    /// <summary>
    /// Draws the events onto the meter area
    /// </summary>
    /// <param name="discreteTimesList">The list of event times from the timer</param>
    protected virtual void DrawEvents(SerializedProperty discreteTimesList)
    {
        for (int i = 0; i < discreteTimesList.arraySize; i++)
        {
            DrawEventMarker(discreteTimesList.GetArrayElementAtIndex(i).floatValue);
        }
    }

    /// <summary>
    /// Draws an event marker at the specified time
    /// </summary>
    /// <param name="normalizedEventTime">The normalized event time</param>
    protected virtual void DrawEventMarker(float normalizedEventTime)
    {
        dimensions.eventMarker.x = dimensions.meterArea.xMin + (normalizedEventTime * dimensions.meterArea.width) - (dimensions.eventMarker.width / 2);
        EditorGUI.DrawRect(dimensions.eventMarker, graphicColors.eventMarkerColor);
    }

    /// <summary>
    /// Retrieves the color scheme of the timer graphic
    /// </summary>
    /// <param name="property"></param>
    protected virtual void GetColors(SerializedObject property)
    {
        graphicColors = ChronoscopeColorPresets.Presets[(ChronoscopeColorPresets.PresetSchemes)property.FindProperty("_colorScheme").enumValueIndex];
    }
#endregion DrawingFunctions

    #region CalculateDimensions
    /// <summary>
    /// Determines the dimensions / positions of the graphic when the inspector has changed
    /// </summary>
    /// <param name="position">The Rect reserved for the graphic</param>
    protected virtual void CalcValues(Rect position)
    {
        oldPosition = new Rect(position);

        dimensions.graphicArea = new Rect(position.xMin, position.yMin, position.width, ChronoscopeCommon.graphicHeight);
        dimensions.headerFooterHeight = ChronoscopeCommon.graphicHeight * ChronoscopeCommon.graphicToHeaderFooterHeightRatio;
        dimensions.meterArea = new Rect(position.xMin + ChronoscopeCommon.graphicSideMarginWidth, position.yMin + dimensions.headerFooterHeight,
                                        dimensions.graphicArea.width - (ChronoscopeCommon.graphicSideMarginWidth * 2), dimensions.graphicArea.height - (2 * dimensions.headerFooterHeight));

        GenerateMeterDimensions();
        GenerateHeaderFooterDimensions();
        GenerateBorderRects(position);        
    }    

    /// <summary>
    /// Calculates the header / footer dimensions
    /// </summary>
    protected void GenerateHeaderFooterDimensions()
    {
        dimensions.headerSeparator = new Rect(dimensions.graphicArea.xMin, dimensions.graphicArea.yMin + dimensions.headerFooterHeight, dimensions.graphicArea.width, 1);
        dimensions.footerSeparator = new Rect(dimensions.graphicArea.xMin, dimensions.graphicArea.yMin + dimensions.headerFooterHeight + dimensions.meterArea.height, dimensions.graphicArea.width, 1);
        
        dimensions.headerTextTop = dimensions.graphicArea.yMin + (dimensions.headerFooterHeight / 2) - (graphicTextStyle.CalcSize(new GUIContent("TEST")).y / 2);
        dimensions.footerTextTop = dimensions.graphicArea.yMin + dimensions.headerFooterHeight + dimensions.meterArea.height +
                                   (dimensions.headerFooterHeight / 2) - (graphicTextStyle.CalcSize(new GUIContent("TEST")).y / 2);
    }

    /// <summary>
    /// Calculates the dimensions of the meter
    /// </summary>
    protected void GenerateMeterDimensions()
    {
        dimensions.meterDivisionWidth = dimensions.meterArea.width / dimensions.meterDivisions;

        dimensions.meterMajorLineHeight = dimensions.meterArea.height * ChronoscopeCommon.meterAreaToMajorLineHeightRatio;
        dimensions.meterMajorLineTop = dimensions.meterArea.center.y - (dimensions.meterMajorLineHeight / 2);

        dimensions.meterMinorLineHeight = dimensions.meterArea.height * ChronoscopeCommon.meterAreaToMinorLineHeightRatio;
        dimensions.meterMinorLineTop = dimensions.meterArea.center.y - (dimensions.meterMinorLineHeight / 2);

        dimensions.meterNormalizedLabelTop = dimensions.meterMajorLineTop - ChronoscopeCommon.meterLabelHeight;

        dimensions.markerWidth = ChronoscopeCommon.graphicSideMarginWidth;

        dimensions.centerMask = new Rect(dimensions.meterArea.xMin + 1, dimensions.meterMinorLineTop, dimensions.meterArea.width - 1, dimensions.meterMinorLineHeight);
        dimensions.eventAreaTopLine = new Rect(dimensions.meterArea.xMin, dimensions.meterMinorLineTop, dimensions.meterArea.width, 1);
        dimensions.eventAreaBottomLine = new Rect(dimensions.meterArea.xMin, dimensions.meterMinorLineTop + dimensions.meterMinorLineHeight, dimensions.meterArea.width, 1);

        dimensions.eventMarker = new Rect(0, dimensions.meterMinorLineTop + 1, 1, dimensions.meterMinorLineHeight - 1);
        dimensions.centreLine = new Rect(dimensions.meterArea.xMin, dimensions.meterArea.center.y, dimensions.meterArea.width, 1);
    }

    /// <summary>
    /// Calculates the dimensions of the graphic border
    /// </summary>
    /// <param name="position">The Rect reserved for the graphic</param>
    protected virtual void GenerateBorderRects(Rect position)
    {
        dimensions.leftBorder = new Rect(position.x, position.y, 1, position.height);
        dimensions.rightBorder = new Rect(position.x + position.width - 1, position.y, 1, position.height);
        dimensions.topBorder = new Rect(position.x, position.y, position.width, 1);
        dimensions.bottomBorder = new Rect(position.x, position.y + position.height, position.width, 1);
    }
#endregion CalculateDimensions    
}
