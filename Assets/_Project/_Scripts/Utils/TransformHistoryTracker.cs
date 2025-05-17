using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHistoryTracker : MonoBehaviour
{
    public int maxHistorySlots = 3;
    public List<Transform> posCache;

    public float savePosThreshold = 1.2f;

    public Color gizmosColor = Color.red;
    public bool drawGizmos = true;

    List<Vector3> _posCacheRaw = new List<Vector3>();

    Vector3 prevPos;

    void Start()
    {
	    prevPos = transform.position;
        
	    foreach (var pos in posCache)
	    {
		    pos.SetParent(null);
	    }
    }

    public void SetThreshold(float value)
    {
        savePosThreshold = value;
    }

    void LateUpdate()
    {
        if ((prevPos - transform.position).magnitude > savePosThreshold)
        {
            _posCacheRaw.Add(transform.position);

            if (_posCacheRaw.Count > maxHistorySlots)
                _posCacheRaw.RemoveAt(0);

            prevPos = transform.position;
        }

        for (int i = 0; i < _posCacheRaw.Count; i++)
        {
            posCache[i].position = _posCacheRaw[i];
        }
    }
    
	public Vector3 GetLastPos()
	{
		return posCache[0].position;
	}

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;
            foreach (var pos in posCache)
            {
	            Gizmos.DrawCube(pos.position, Vector3.one * .3f);
	            Gizmos.DrawLine(pos.position, pos.position + Vector3.up);
            }
            
	        //Gizmos.color = Color.blue;
	        //Gizmos.DrawCube(GetLastPos(), Vector3.one * .3f);
	        //Gizmos.DrawLine(GetLastPos(), GetLastPos() + Vector3.up * 2);
        }
    }
}
