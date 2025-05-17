using UnityEditor;
using UnityEngine;

public static class SetIcon
{
    private static void TestSetIconForNewScript()
    {
        Selection.selectionChanged += SelectionChanged;
        ProjectWindowUtil
            .CreateScriptAssetFromTemplateFile(
                "Assets/ProjectData/MissingScriptChecker/ToolsMissingScript/Script.cs",
                "Assets/ProjectData/MissingScriptChecker/ToolsMissingScript/Script/TeatSetIcon.cs");
    }

    private static void SelectionChanged()
    {
        Selection.selectionChanged -= SelectionChanged;
        if (Selection.activeObject is not MonoScript newMonoScript) return;
        if (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(newMonoScript)) is not MonoImporter monoImporter) return;
     
        var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ProjectData/MissingScriptChecker/ToolsMissingScript/Script/Icon.png");
        monoImporter.SetIcon(icon);
        monoImporter.SaveAndReimport();
    }
}