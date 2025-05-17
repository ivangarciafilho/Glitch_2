using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireEvent_OnkeyDown : MonoBehaviour
{
    public  UnityEvent triggeredEvent;
    public KeyCode triggeringkey;

    public void Update()
    {
        if ( Input.GetKeyDown (triggeringkey) )
            triggeredEvent?.Invoke ();
    }
}
