using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class ParticlesBakerContextMenu
{
    static int counterSaved = 0;

    static void GenerateBake()
    {

    }

    public static void GenerateFromContext(ParticleSystem ps)
    {
        Transform t = ps.transform;

        ParticlesBakerProfile bakerProfile = ParticlesBakerProfileEditor.GetDefault();

        string path = "";
        string key = t.gameObject.name + "_" + counterSaved.ToString() + DateTime.Now.ToString("MMddyyyy_hhmmsstt");
        {
            path = Application.dataPath + "/GeneratedMeshes/";
            if (bakerProfile != null && bakerProfile.exportPath == ParticlesBakerSettingsExportPath.ExportToFixedPath)
                path = Application.dataPath + "/" + bakerProfile.fixedPath + "/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = "Assets/GeneratedMeshes";
            if (bakerProfile != null && bakerProfile.exportPath == ParticlesBakerSettingsExportPath.ExportToFixedPath)
                path = "Assets/" + bakerProfile.fixedPath;
        }

        string objPath = path + "/" + key + ".prefab";
        if (bakerProfile != null && bakerProfile.exportPath == ParticlesBakerSettingsExportPath.AskWhere)
        {
            objPath = EditorUtility.SaveFilePanel(
                "Choose a path to save the Baked Particles",
                "",
                key + ".prefab",
                "prefab");
        }
 
        var objPathSplitted = objPath.Split('/');
        var index = Array.FindIndex(objPathSplitted, it => it == "Assets");
        objPath = "";
        for (int i = index; i < objPathSplitted.Length; i++)
        {
            objPath += objPathSplitted[i] + (i < objPathSplitted.Length - 1 ? "/" : "");
        }

        GameObject wholeObj = new GameObject();
        GameObject p = PrefabUtility.SaveAsPrefabAssetAndConnect(wholeObj, objPath, InteractionMode.AutomatedAction);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var ingredients = ParticlesBaker.Bake(ps, wholeObj, bakerProfile);


        //Editor Post Processing       
        if (!bakerProfile.mergeMeshesWithSimilarMaterials)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                ParticleIngredient pi = ingredients[i];
                
                if (pi.mesh != null && (bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.All || bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.ParticlesOnly))
                    AssetDatabase.AddObjectToAsset(pi.mesh, AssetDatabase.GetAssetPath(p));

                if (pi.trailsMesh != null && (bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.All || bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.TrailsOnly))
                    AssetDatabase.AddObjectToAsset(pi.trailsMesh, AssetDatabase.GetAssetPath(p));
            
            }
        }
        else
        {
            MeshFilter[] allMeshFilters = wholeObj.GetComponentsInChildren<MeshFilter>();
            foreach (var mFilter in allMeshFilters)
            {
                AssetDatabase.AddObjectToAsset(mFilter.sharedMesh, AssetDatabase.GetAssetPath(p));
            }
        }
        

        PrefabUtility.ApplyPrefabInstance(wholeObj, InteractionMode.AutomatedAction);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameObject.DestroyImmediate(wholeObj);
    }

    [MenuItem("CONTEXT/ParticleSystem/Bake Into Mesh")]
    static void BakePS(MenuCommand command)
    {
        ParticleSystem ps = (ParticleSystem)command.context;
        GenerateFromContext(ps);
    }
}
