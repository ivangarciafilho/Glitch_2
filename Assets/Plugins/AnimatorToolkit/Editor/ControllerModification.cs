using UnityEngine;
using UnityEditor;

public class ControllerModification : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] i_Paths)
    {
        bool updatePreviewer = false;
        foreach (string path in i_Paths)
        {
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            if(controller != null)
            {
                updatePreviewer = true;
                ControllerData controllerData = AssetDatabase.LoadAssetAtPath<ControllerData>(path);
                if(controllerData != null)
                {
                    controllerData.AddAllStates(controller);
                }
            }
        }

        if(updatePreviewer)
        {
            AnimatorPreviewer.ForceUpdate();
        }

        return i_Paths;
    }
}