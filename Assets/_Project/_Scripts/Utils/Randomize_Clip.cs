using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize_Clip : MonoBehaviour
{
    public AudioClip[] variants;
    public AudioSource source;

    public void Randomize()
        => source.clip = variants[Random.Range(0, variants.Length ) ];
}
