using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformerRaycaster
{
    public Vector2 Size = Vector2.one;
    public Vector2 Offset;
    public float ShellThickness = 0.1f;
    public int HorizontalRayCount = 3;
    public int VerticalRayCount = 3;

    private readonly List<RaycastHit2D> _buffer = new();

    public void DrawGizmos(Vector2 center)
    {
        Vector2 point = center + Offset;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(point, Size);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(point, Size - Vector2.one * ShellThickness);
    }

    public bool Cast(Vector2 position, LayerMask layerMask, ref Vector2 displacement)
    {
        bool hit = false;
        Vector2 center = position + Offset;
        Vector2 horizontalExtent = Mathf.Max(Size.x / 2 - ShellThickness, 0) * Vector2.right;
        Vector2 verticalExtent = Mathf.Max(Size.y / 2 - ShellThickness, 0) * Vector2.up;
        float signX = Mathf.Sign(displacement.x);
        float signY = Mathf.Sign(displacement.y);

        // Vertical Raycast
        float distanceY = Mathf.Abs(displacement.y);
        if (distanceY > 0 && PerformRaycast(
                VerticalRayCount,
                center,
                horizontalExtent,
                signY * verticalExtent,
                layerMask,
                ref distanceY))
        {
            hit = true;
        }

        // Horizontal Raycast
        float distanceX = Mathf.Abs(displacement.x);
        if (distanceX > 0 && PerformRaycast(
                HorizontalRayCount,
                center,
                verticalExtent,
                signX * horizontalExtent,
                layerMask,
                ref distanceX))
        {
            hit = true;
        }

        if (hit)
        {
            displacement = new Vector2(signX * distanceX, signY * distanceY);
        }

        return hit;
    }

    // Helper method for raycasting
    private bool PerformRaycast(
        int count,
        Vector2 center,
        Vector2 range,
        Vector2 direction,
        LayerMask mask,
        ref float distance)
    {
        bool hit = false;
        ContactFilter2D contactFilter = new()
        {
            useLayerMask = true,
            layerMask = mask,
        };
        foreach (Vector3 delta in MathUtil.FormationLine(count, -range, range))
        {
            Vector2 origin = center + direction + (Vector2)delta;
            Physics2D.Raycast(origin, direction, contactFilter, _buffer, distance + ShellThickness);
            foreach (RaycastHit2D hitInfo in _buffer)
            {
                float actualDistance = hitInfo.distance - ShellThickness;
                if (actualDistance < distance)
                {
                    hit = true;
                    distance = actualDistance;
                }
            }
        }

        return hit;
    }
}