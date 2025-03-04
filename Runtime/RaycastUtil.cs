using System;
using System.Collections.Generic;
using UnityEngine;

public static class RaycastUtil
{
    private static readonly Dictionary<Collider2D, RaycastHit2D> WorkingMemoryDic = new();
    private static readonly List<RaycastHit2D> WorkingMemoryList = new();

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
        WorkingMemoryDic.Clear();
        for (var i = 0; i < rayCount; i++)
        {
            var rayOrigin = origin + normal * (spacing * i - halfWidth);
            Physics2D.Raycast(rayOrigin, direction, filter, results, distance); // this will clear results
            foreach (var hit in results)
            {
                var collider = hit.collider;
                if (WorkingMemoryDic.TryGetValue(collider, out var previousHit))
                {
                    if (previousHit.distance > hit.distance)
                    {
                        WorkingMemoryDic[collider] = hit;
                    }
                }
                else
                {
                    WorkingMemoryDic[collider] = hit;
                }
            }
        }

        // collect results
        results.Clear();
        results.AddRange(WorkingMemoryDic.Values);
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
        LayerMask mask,
        Func<RaycastHit2D, bool> criteria = null
    )
    {
        if (rayCount < 1)
        {
            Debug.LogWarning("raycast count less than 1, it hit nothing");
            return default;
        }

        if (Mathf.Approximately(castWidth, 0))
        {
            rayCount = 1;
        }

        var halfWidth = castWidth / 2;
        var spacing = rayCount == 1 ? 0 : castWidth / (rayCount - 1);
        var normal = Vector2.Perpendicular(direction).normalized;
        RaycastHit2D result = default;
        for (var i = 0; i < rayCount; i++)
        {
            var offset = rayCount == 1 ? Vector2.zero : normal * (spacing * i - halfWidth);
            var rayOrigin = origin + offset;
            if (criteria != null)
            {
                // find all and use custom criteria
                var filter = new ContactFilter2D
                {
                    useLayerMask = true,
                    layerMask = mask,
                };
                Physics2D.Raycast(rayOrigin, direction, filter, WorkingMemoryList, distance);
                foreach (var hit in WorkingMemoryList)
                {
                    if (!criteria.Invoke(hit))
                    {
                        continue;
                    }

                    if (!result || result.distance > hit.distance)
                    {
                        result = hit;
                    }
                }
            }
            else
            {
                // try find nearest one
                var hit = Physics2D.Raycast(rayOrigin, direction, distance, mask);
                if (!hit)
                {
                    continue;
                }

                if (!result || result.distance > hit.distance)
                {
                    result = hit;
                }
            }
        }

        if (result)
        {
            // adjust distance to be the projection along the ray direction
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
        LayerMask mask,
        Func<RaycastHit2D, bool> filter = null
    )
    {
        var actualWidth = castWidth - 2 * shellThickness;
        var actualOrigin = origin + direction.normalized * (extent - shellThickness);
        var actualDistance = distance + shellThickness;
        var hit = ParallelRaycast(actualOrigin, rayCount, actualWidth, direction, actualDistance, mask, filter);
        hit.distance -= shellThickness;
        return hit;
    }

    public static void
        DrawGizmosParallelRays(Vector3 center, int count, float width, Vector3 direction) // direction contains both direction and length
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