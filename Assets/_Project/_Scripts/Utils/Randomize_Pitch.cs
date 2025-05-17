using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize_Pitch : MonoBehaviour
{
    public AudioSource source;
    public Vector2 pitchRange;

    public void Randomize()
        => source.pitch = Random.Range(pitchRange.x,    pitchRange.y);
}
