using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEventOnCollide : MonoBehaviour
{
	public bool useTagFilter = false;
	public bool useLayerFilter = false;

	public List<string> tagFilter = new List<string>();

	public LayerMask layerFilter;

	public CollisionEvent onEnter = new CollisionEvent();
	public CollisionEvent onStay = new CollisionEvent();
	public CollisionEvent onExit = new CollisionEvent();

	CollisionEventData data = new CollisionEventData();

	private void OnCollisionEnter(Collision other) => Send(other, onEnter);

	private void OnCollisionStay(Collision other) => Send(other, onStay);

	private void OnCollisionExit(Collision other) => Send(other, onExit);

	protected virtual void Send(Collision other, CollisionEvent _evtCaller)
	{
		if(useLayerFilter)
		{
			if(!GameUtils.LayerMaskContains(other.gameObject.layer, layerFilter))
			{
				return;
			}
		}

		if(useTagFilter)
		{
		if (!tagFilter.Contains(other.gameObject.tag))
			return;
		}

        

		data.sender = gameObject;
		data.collider = other.collider;
		data.gameObject = other.gameObject;
		data.transform = other.transform;

		_evtCaller.InvokeSafe(data);
	}
}
