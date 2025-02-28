using System.Collections.Generic;
using UnityEngine;

public static class RaycastUtil
{
    private static readonly Dictionary<Collider2D, RaycastHit2D> WorkingMemory = new();

    /**
     * used in bullets with width
     */
    public static int ParallelRaycastAll(
        Vector2 origin,
        int rayCount,
        float castWidth,
        Vector2 direction,
        ContactFilter2D filter,
        List<RaycastHit2D> results,
        float distance
    )
    {
        if (Mathf.Approximately(castWidth, 0))
        {
            rayCount = 1;
        }

        if (rayCount < 1)
        {
            rayCount = 1;
        }

        if (rayCount == 1)
        {
            return Physics2D.Raycast(origin, direction, filter, results, distance);
        }

        // Perform a series of parallel raycasts and store the results
        var halfWidth = castWidth / 2;
        var spacing = castWidth / (rayCount - 1);
        var normal = Vector2.Perpendicular(direction).normalized;

        // group by collider
        WorkingMemory.Clear();
        for (var i = 0; i < rayCount; i++)
        {
            var rayOrigin = origin + normal * (spacing * i - halfWidth);
            Physics2D.Raycast(rayOrigin, direction, filter, results, distance); // this will clear results
            foreach (var hit in results)
            {
                var collider = hit.collider;
                if (WorkingMemory.TryGetValue(collider, out var previousHit))
                {
                    if (previousHit.distance > hit.distance)
                    {
                        WorkingMemory[collider] = hit;
                    }
                }
                else
                {
                    WorkingMemory[collider] = hit;
                }
            }
        }

        // collect results
        results.Clear();
        results.AddRange(WorkingMemory.Values);
        return results.Count;
    }

    /**
     * return nearest hit
     */
    public static RaycastHit2D ParallelRaycast(
        Vector2 origin,
        int rayCount,
        float castWidth,
        Vector2 direction,
        float distance,
        LayerMask mask
    )
    {
        if (Mathf.Approximately(castWidth, 0))
        {
            rayCount = 1;
        }

        if (rayCount < 1)
        {
            rayCount = 1;
        }

        if (rayCount == 1)
        {
            return Physics2D.Raycast(origin, direction, distance, mask);
        }

        var halfWidth = castWidth / 2;
        var spacing = castWidth / (rayCount - 1);
        var normal = Vector2.Perpendicular(direction).normalized;
        RaycastHit2D result = default;
        for (var i = 0; i < rayCount; i++)
        {
            var rayOrigin = origin + normal * (spacing * i - halfWidth);
            var hit = Physics2D.Raycast(rayOrigin, direction, distance, mask);

#if UNITY_EDITOR && DEBUG_DRAW
            Color debugColor = hit ? Color.red : Color.green;
            float debugDistance = hit ? hit.distance : distance;
            Vector2 debugVector = direction.normalized * debugDistance;
            // DebugUtil.DrawCross(rayOrigin, Color.blue);
            Debug.DrawLine(rayOrigin, rayOrigin + debugVector, debugColor);
#endif

            if (!hit)
            {
                continue;
            }

            if (!result || result.distance > hit.distance)
            {
                result = hit;
            }
        }

        if (result)
        {
            result.distance = Vector2.Dot(result.point - origin, direction);
        }

        return result;
    }

    public static RaycastHit2D PlatformerRaycast(
        Vector2 origin,
        int rayCount,
        float castWidth,
        float extent,
        float shellThickness,
        Vector2 direction, // usually up or right
        float distance,
        LayerMask mask
    )
    {
        var actualWidth = castWidth - 2 * shellThickness;
        var actualOrigin = origin + direction.normalized * (extent - shellThickness);
        var actualDistance = distance + shellThickness;
        var hit = ParallelRaycast(actualOrigin, rayCount, actualWidth, direction, actualDistance, mask);
        hit.distance -= shellThickness;
        return hit;
    }

    public static void DrawGizmosParallelRays(Vector3 center, int count, float width, Vector3 direction) // direction contains both direction and length
    {
        if (count < 1)
        {
            count = 1;
        }

        if (count == 1)
        {
            Gizmos.DrawLine(center, center + direction);
            return;
        }

        var spacing = width / (count - 1);
        var normal = Vector3.Cross(direction, Vector3.forward).normalized;
        var halfWidth = width / 2;

        for (var i = 0; i < count; i++)
        {
            var rayOrigin = center + normal * (spacing * i - halfWidth);
            Gizmos.DrawLine(rayOrigin, rayOrigin + direction);
        }
    }
}