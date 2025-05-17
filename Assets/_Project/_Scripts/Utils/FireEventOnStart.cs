using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class FireEventOnStart : MonoBehaviour
{
    public float delay = 1f;
    public UltEvent evt = new UltEvent();

    void Start()
    {
        if(delay <= 0f)
        {
             evt.InvokeSafe();
        }
        else 
        {
            StartCoroutine(Routine());   
        }
    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(delay);
        evt.InvokeSafe();
    }
}
