using UnityEditor;
using UnityEngine;


public class RayEmitter : MonoBehaviour
{
    public float length;
    LineRenderer line;

    [HideInInspector] public Vector3 direction;

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

    public void Update()
    {
        var ray = GetRay();
        line.SetPosition(0, ray.origin);
        line.SetPosition(1, ray.GetPoint(length));
    }

    public Ray GetRay()
    {
        var origin = transform.position;
        var los = direction - origin;
        return new Ray(origin, los);
    }
}