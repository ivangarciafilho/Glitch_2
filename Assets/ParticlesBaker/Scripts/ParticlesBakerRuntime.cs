using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesBakerRuntime : MonoBehaviour
{
    public ParticlesBakerProfile profile;

    public ParticleSystem particleSystemTarget;
    public GameObject targetObject;
    public bool generateAtStart = false;

    private void Start()
    {
        if (generateAtStart)
            Generate();
    }

    public void Generate()
    {
        var targetParent = (targetObject == null ? gameObject : targetObject);

        if (particleSystemTarget == null)
        {
            Debug.LogError("You must assign a Particle System to be baked.");
            return;
        }

        ParticlesBaker.Bake(particleSystemTarget, targetParent, profile);      
    }
}
