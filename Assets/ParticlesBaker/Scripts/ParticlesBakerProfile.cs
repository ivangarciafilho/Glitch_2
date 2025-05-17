using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticlesBakerSettingsExportPath
{
    ExportToFixedPath,
    AskWhere
}

public enum ParticlesBakerProfileHierarchyOptions
{
    BakeWithHierarchy,
    BakeWithoutHierarchy
}

public enum ParticlesBakerProfileRenderingOptions
{
    All,
    ParticlesOnly,
    TrailsOnly
}

[CreateAssetMenu(fileName = "ParticlesBaker New Profile", menuName = "Particles Baker/New Particles Baker Profile")]
public class ParticlesBakerProfile : ScriptableObject
{
    public ParticlesBakerProfileHierarchyOptions hierarchyOptions = ParticlesBakerProfileHierarchyOptions.BakeWithHierarchy;
    public ParticlesBakerProfileRenderingOptions renderingOptions = ParticlesBakerProfileRenderingOptions.All;

    public ParticlesBakerSettingsExportPath exportPath = ParticlesBakerSettingsExportPath.ExportToFixedPath;
    public string fixedPath = "GeneratedParticleBaker";
    public bool mergeMeshesWithSimilarMaterials = false;

    public bool mainProfile = false;

    public ParticlesBakerProfile()
    {
        mainProfile = false;
    }
}
