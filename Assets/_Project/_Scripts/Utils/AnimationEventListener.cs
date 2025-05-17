using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using UnityEngine.Events;

[System.Serializable]
public class AnimationEventListenerEntry
{
    public string name;
    public UltEvent AnimEvent = new UltEvent();
}

public class AnimationEventListener : MonoBehaviour
{
    
    public List<AnimationEventListenerEntry> listeners = new List<AnimationEventListenerEntry>();
    public bool debugLog = false;

    public void TickEvent(string name)
    {
        var listener = listeners.Find(it => it.name == name);
        if (listener != null)
        {
            
            listener.AnimEvent.InvokeSafe();
            if (debugLog)
            {
                Debug.Log("[Animation Event Listener:] Event " + name, gameObject);
                // System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                // Debug.Log(t.ToString());
            }
        }
    }

	public void ListenToEvent(string name, System.Action action)
	{
		var listener = listeners.Find(it => it.name == name);
        if (listener != null)
        {
            listener.AnimEvent.PersistentCalls += action;
        }
	}
}
