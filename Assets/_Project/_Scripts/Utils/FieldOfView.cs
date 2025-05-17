using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public float maskCutawayDst = .1f;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        //StartCoroutine("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public bool IsInsideFOV(Transform target)
    {
        var mag = (transform.position - target.position).magnitude;
        if (mag <= viewRadius)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                return true;
            }

        }

        return false;
    }
   

    public bool IsInsideFOV_WithObstacle(Transform target)
    {
        var mag = (transform.position - target.position).magnitude;
        if (mag <= viewRadius)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
	            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget + 10f, obstacleMask, QueryTriggerInteraction.Ignore))
                {

                    return true;
                }

            }

        }

        return false;
    }
    
	public bool IsInsideFOV_WithObstacle_IgnoreRadius(Transform target)
	{
		//var mag = (transform.position - target.position).magnitude;
		//if (mag <= viewRadius)
		{
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget + 10f, obstacleMask, QueryTriggerInteraction.Ignore))
				{

					return true;
				}

			}

		}

		return false;
	}


	public bool IsInsideFOV_WithObstacle_CastOnly(Transform target)
	{
		//var mag = (transform.position - target.position).magnitude;
		//if (mag <= viewRadius)
		{
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			//if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget + 10f, obstacleMask, QueryTriggerInteraction.Ignore))
				{

					return true;
				}

			}

		}

		return false;
	}


    public bool IsInsideFOV_IgnoreRadius(Transform target)
    {
        //var mag = (transform.position - target.position).magnitude;
        //if (mag <= viewRadius)
        {
            //Vector3 dirToTarget = (target.position - transform.position).normalized;

            if(PointInCameraView(target.position))
            //if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                return true;
            }

        }

        return false;
    }

    public bool IsInsideFOV_IgnoreRadius(Vector3 pos)
    {
        //var mag = (transform.position - pos).magnitude;
        //if (mag <= viewRadius)
        {
            //Vector3 dirToTarget = (pos - transform.position).normalized;

            if(PointInCameraView(pos))
           // if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                return true;
            }

        }

        return false;
    }

    public bool IsInsideFOV(Vector3 pos)
    {
        var mag = (transform.position - pos).magnitude;
        if (mag <= viewRadius)
        {
            Vector3 dirToTarget = (pos - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                return true;
            }

        }

        return false;
    }


    List<int> _triangles = new List<int>();
    void DrawFieldOfView()
    {
        if (!viewMeshFilter.gameObject.activeInHierarchy) return;

        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];

        _triangles.Clear();
        
       // int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * maskCutawayDst;

            if (i < vertexCount - 2)
            {
                if(_triangles.Count <= i * 3)
                    _triangles.Add(0);
                
                 if(_triangles.Count <= i * 3 + 1)
                    _triangles.Add(i + 1);

                if(_triangles.Count <= i * 3 + 2)
                    _triangles.Add(i + 2);

               // triangles[i * 3] = 0;
               // triangles[i * 3 + 1] = i + 1;
               // triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = _triangles.ToArray();
        viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public bool PointInCameraView(Vector3 point)
    {
        Vector3 viewport = Camera.main.WorldToViewportPoint(point);
        bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
        bool inFrontOfCamera = viewport.z > 0;

//        RaycastHit depthCheck;
        bool objectBlockingPoint = false;

        Vector3 directionBetween = point - Camera.main.transform.position;
        directionBetween = directionBetween.normalized;

        float distance = Vector3.Distance(Camera.main.transform.position, point);

        //if (Physics.Raycast(Camera.main.transform.position, directionBetween, out depthCheck, distance + 0.05f))
        //{
        //    if (depthCheck.point != point)
        //    {
        //        objectBlockingPoint = true;
        //    }
        //}

        return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
    }

    public bool Is01(float a)
    {
        return a > 0 && a < 1;
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}