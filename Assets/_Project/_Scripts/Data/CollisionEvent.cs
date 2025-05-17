using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

[System.Serializable]
public class CollisionEvent : UltEvent<CollisionEventData> { }

public class CollisionEventData
{
    public GameObject sender;
    public Collider collider;
    public Transform transform;
    public GameObject gameObject;
}
