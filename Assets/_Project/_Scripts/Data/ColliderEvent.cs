using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

[System.Serializable]
public class ColliderEvent : UltEvent<ColliderEventData> { }

public class ColliderEventData
{
    public GameObject sender;
    public Collider collider;
    public Transform transform;
    public GameObject gameObject;
}
