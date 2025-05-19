using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleIngredient
{
    public Transform transform;
    public ParticleIngredient parent;

    public Material material;
    public Material trailsMaterial;
    public Mesh mesh;
    public Mesh trailsMesh;

    public GameObject generatedObject;
}

public class ParticlesBaker
{
    public static List<ParticleIngredient> Bake(ParticleSystem particleSystem, GameObject targetObject, ParticlesBakerProfile bakerProfile)
    {
        var ingredients = ParticlesBaker.Generate(particleSystem);
        Dictionary<ParticleIngredient, Transform> transformByParticleIngredient = new Dictionary<ParticleIngredient, Transform>();

        for (int i = 0; i < ingredients.Count; i++)
        {
            ParticleIngredient pi = ingredients[i];

            GameObject firstObj = null;

            if (pi.mesh != null && (bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.All || bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.ParticlesOnly))
            {
                GameObject obj = ParticlesBaker.NewObjectForMesh(pi.mesh, pi.material);
                obj.transform.SetParent(targetObject.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                obj.name = pi.transform.gameObject.name + "_Particles";
                firstObj = obj;
            }

            if (pi.trailsMesh != null && (bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.All || bakerProfile.renderingOptions == ParticlesBakerProfileRenderingOptions.TrailsOnly))
            {
                GameObject trailObj = ParticlesBaker.NewObjectForMesh(pi.trailsMesh, pi.trailsMaterial);
                trailObj.name = pi.transform.gameObject.name + "_Trails";

                if (firstObj != null)
                    trailObj.transform.SetParent(firstObj.transform);
                else
                {
                    firstObj = trailObj;
                    trailObj.transform.SetParent(targetObject.transform);
                }

                trailObj.transform.localPosition = Vector3.zero;
                trailObj.transform.localRotation = Quaternion.identity;
            }

            pi.generatedObject = firstObj;
            if (firstObj != null) transformByParticleIngredient.Add(pi, firstObj.transform);
        }

        if (bakerProfile.hierarchyOptions == ParticlesBakerProfileHierarchyOptions.BakeWithHierarchy)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                ParticleIngredient pi = ingredients[i];
                if (pi.parent != null)
                {
                    if (transformByParticleIngredient.ContainsKey(pi) && transformByParticleIngredient.ContainsKey(pi.parent))
                    {
                        pi.generatedObject.transform.SetParent(transformByParticleIngredient[pi.parent]);
                    }
                }
            }
        }

        if (bakerProfile.mergeMeshesWithSimilarMaterials)
        {
            Dictionary<Material, List<MeshFilter>> meshesByMaterial = new Dictionary<Material, List<MeshFilter>>();

            for (int i = 0; i < ingredients.Count; i++)
            {
                ParticleIngredient pi = ingredients[i];
                MeshRenderer[] allRenderers = pi.generatedObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var rend in allRenderers)
                {
                    if (!meshesByMaterial.ContainsKey(rend.sharedMaterial))
                        meshesByMaterial.Add(rend.sharedMaterial, new List<MeshFilter>());

                    meshesByMaterial[rend.sharedMaterial].Add(rend.gameObject.GetComponent<MeshFilter>());
                }
            }

            int c = 0;
            foreach (var shared in meshesByMaterial)
            {
                var combinedMesh = new Mesh();
                CombineInstance[] instances = new CombineInstance[shared.Value.Count];
                for (int i = 0; i < instances.Length; i++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = shared.Value[i].sharedMesh;
                    ci.transform = shared.Value[i].transform.localToWorldMatrix;
                    instances[i] = ci;
                }

                combinedMesh.CombineMeshes(instances);

                combinedMesh.name = "CombinedMesh_" + c;
                GameObject combinedObj = ParticlesBaker.NewObjectForMesh(combinedMesh, shared.Key);
                combinedObj.name = "CombinedMesh_" + c;

                combinedObj.transform.SetParent(targetObject.transform);

                c++;
            }

            for (int i = 0; i < ingredients.Count; i++)
            {
                ParticleIngredient pi = ingredients[i];
                GameObject.DestroyImmediate(pi.generatedObject);
            }
        }

        return ingredients;
    }

    public static List<ParticleIngredient> Generate(ParticleSystem particleSystem)
    {
        List<ParticleIngredient> ingredients = new List<ParticleIngredient>();

        Transform t = particleSystem.transform;
        InvestigateTransform(ingredients, t, true);

        return ingredients;
    }

    static ParticleIngredient InvestigateTransform(List<ParticleIngredient> ingredients, Transform t, bool first = false)
    {
        ParticleIngredient ingredient = null;

        ParticleSystemRenderer renderer = t.GetComponent<ParticleSystemRenderer>();

        if (renderer && renderer.enabled)
        {
            ParticleSystem ps = t.GetComponent<ParticleSystem>();

            try
            {
                ingredient = new ParticleIngredient();
                ingredient.transform = t;

                if (renderer.renderMode != ParticleSystemRenderMode.None)
                {
                    Mesh newMesh = new Mesh();
                    newMesh.name = "_Mesh";
                    renderer.BakeMesh(newMesh, true);
                    if (newMesh.vertexCount > 0)
                    {
                        ingredient.mesh = newMesh;
                        ingredient.material = renderer.sharedMaterial;
                    }
                }

                if (ps.trails.enabled)
                {
                    Mesh trailsMesh = new Mesh();
                    trailsMesh.name = "_TrailsMesh";
                    renderer.BakeTrailsMesh(trailsMesh, true);
                    if (trailsMesh.vertexCount > 0)
                    {
                        ingredient.trailsMesh = trailsMesh;
                        ingredient.trailsMaterial = renderer.trailMaterial;
                    }
                }


                ingredients.Add(ingredient);
            }
            catch (Exception e)
            {
                Debug.Log("Error building mesh for " + t.gameObject.name + ", but the error was handled and the baking process continues. \n" + e.Message + "\n" + e.StackTrace);
            }
        }

        for (int i = 0; i < t.childCount; i++)
        {
            var childIngredient = InvestigateTransform(ingredients, t.GetChild(i));

            if (ingredient != null && childIngredient != null)
                childIngredient.parent = ingredient;
        }

        return ingredient;
    }

    public static GameObject NewObjectForMesh(Mesh m, Material mat)
    {
        GameObject newObject = new GameObject();

        MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = m;

        MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;

        return newObject;
    }
}
