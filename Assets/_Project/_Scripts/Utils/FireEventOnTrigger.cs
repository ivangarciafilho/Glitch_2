using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEventOnTrigger : MonoBehaviour
{
    public bool oneTimeUse = false;
    bool usedOnce;

    public bool useTagFilter = false;
    public bool useLayerFilter = false;

    public List<string> tagFilter = new List<string>();

    public LayerMask layerFilter;

    public ColliderEvent onEnter = new ColliderEvent();
    public ColliderEvent onStay = new ColliderEvent();
    public ColliderEvent onExit = new ColliderEvent();

    ColliderEventData data = new ColliderEventData();

    private void OnTriggerEnter(Collider other) => Send(other, onEnter);

    private void OnTriggerStay(Collider other) => Send(other, onStay);

    private void OnTriggerExit(Collider other) => Send(other, onExit);

    private void OnEnable()
    {
        if(oneTimeUse && usedOnce)
        {
            Debug.Log(gameObject.name + " - Used ONCE and will not enable again");
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        usedOnce = true;
    }

    protected virtual void Send(Collider other, ColliderEvent _evtCaller)
    {
        if(useLayerFilter)
        {
            if(!GameUtils.LayerMaskContains(other.gameObject.layer, layerFilter))
            {
                return;
            }
        }

        if (!tagFilter.Contains(other.gameObject.tag))
            return;

        

        data.sender = gameObject;
        data.collider = other;
        data.gameObject = other.gameObject;
        data.transform = other.transform;

        _evtCaller.InvokeSafe(data);
    }
}
