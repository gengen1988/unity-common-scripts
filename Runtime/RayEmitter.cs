using UnityEditor;
using UnityEngine;


public class RayEmitter : MonoBehaviour
{
    public float length;
    public LayerMask blockBy;

    LineRenderer line;

    [HideInInspector] public Vector3 aim;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        var ray = GetRay();
        var end = ray.GetPoint(length);
        Gizmos.DrawLine(ray.origin, end);
        Handles.Label(ray.origin, "ray origin");
#endif
    }

    void Awake()
    {
        TryGetComponent(out line);
    }

    public void FixedUpdate()
    {
        var ray = GetRay();
        var hit = Physics2D.Raycast(ray.origin, ray.direction, length, blockBy);
        line.SetPosition(0, ray.origin);
        if (hit)
        {
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, ray.GetPoint(length));
        }
    }
    
    public Ray GetRay()
    {
        return new Ray(transform.position, transform.right);
    }

    public Ray GetRay2()
    {
        var origin = transform.position;
        var los = aim - origin;
        
        if (aim == origin)
        {
            Debug.LogWarning("aim is same as origin, cause unsure direction");
            return new Ray(origin, Vector3.up);
        }

        return new Ray(origin, los);
    }
}