using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUtils
{
    public static bool LayerMaskContains(int layer, LayerMask lm)
    {
        return (lm == (lm | (1 << layer)));
    }

    public static bool LayerMaskContains(Collider2D collider, LayerMask lm)
    {
        return LayerMaskContains(collider.gameObject.layer, lm);
    }

    public static bool LayerMaskContains(Collision2D collision, LayerMask lm)
    {
        return LayerMaskContains(collision.gameObject.layer, lm);
    }

    public static bool LayerMaskContains(Collider collider, LayerMask lm)
    {
        return LayerMaskContains(collider.gameObject.layer, lm);
    }

    public static bool LayerMaskContains(Collision collision, LayerMask lm)
    {
        return LayerMaskContains(collision.gameObject.layer, lm);
    }

    public static void SetEulerAngles(Transform t, Vector3 euler)
    {
        Quaternion q = t.rotation;
        q.eulerAngles = euler;
        t.rotation = q;
    }

    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static Vector3 MidPoint(Vector3 a, Vector3 b)
    {
        return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
    }

    public static float ClosestTo(IEnumerable<float> collection, float target)
    {
        return collection.OrderBy(x => Mathf.Abs(target - x)).First();
    }

    
    public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // public static  bool IsHitByMainCameraRay(Camera camera, Collider coll)
    // {
    //     Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
    //     RaycastHit hit;
    //     if (Physics.Raycast(ray, out hit, 100f, Game.Instance.BodyPartsLayer))
    //     {
    //         if (hit.collider == coll)
    //             return true;
    //     }

    //     return false;
    // }

    public static  bool IsHitByMainCameraRay(Camera camera, Collider[] colls, LayerMask mask)
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, mask))
        {
            foreach(var coll in colls)
                if (hit.collider == coll)
                    return true;
        }

        return false;
    }
}

public static class QuaterionExtensions
{
    public static Quaternion SetEulerAngleX(this Quaternion quat, float value)
    {
        Vector3 ea = quat.eulerAngles;
        ea.x = value;
        quat.eulerAngles = ea;
        return quat;
    }

    public static Quaternion SetEulerAngleY(this Quaternion quat, float value)
    {
        Vector3 ea = quat.eulerAngles;
        ea.y = value;
        quat.eulerAngles = ea;
        return quat;
    }

    public static Quaternion SetEulerAngleZ(this Quaternion quat, float value)
    {
        Vector3 ea = quat.eulerAngles;
        ea.z = value;
        quat.eulerAngles = ea;
        return quat;
    }
}
