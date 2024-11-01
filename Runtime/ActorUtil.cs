using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

/**
 * notes that these methods are not thread safe
 */
public static class ActorUtil
{
    private static readonly List<Collider2D> _queryBuffer = new();

    public static bool TryGetActor(this Collider2D col, out Actor actor)
    {
        Component from = col;
        if (col.attachedRigidbody)
        {
            from = col.attachedRigidbody;
        }

        return from.TryGetComponent(out actor);
    }

    public static int FindActorCircle(this Actor actor, float radius, List<Actor> results)
    {
        return FindActorArc(actor, Vector2.right, 360f, radius, results);
    }

    public static int FindActorArc(this Actor actor, Vector2 from, float angle, float radius, List<Actor> results)
    {
        results.Clear();
        var sqrRadius = radius * radius;
        var actorTransform = actor.transform;
        var center = actorTransform.position;
        var filter = new ContactFilter2D
        {
            // useLayerMask = true,
            // layerMask = LayerMask.GetMask("Default")
        };
#if UNITY_EDITOR && !DISABLE_DEBUG_DRAW
        DebugUtil.DrawWireArc2D(center, from, angle, radius, Color.yellow);
#endif
        Physics2D.OverlapCircle(center, radius, filter, _queryBuffer);
        foreach (var found in _queryBuffer)
        {
            // remove self
            if (found.transform.IsChildOf(actorTransform))
            {
                continue;
            }

            // remove non actor
            if (!found.TryGetActor(out var other))
            {
                continue;
            }

            // remove too far
            var otherCenter = other.transform.position;
            var los = otherCenter - center;
            if (los.sqrMagnitude > sqrRadius)
            {
                continue;
            }

            // remove not in angle range
            if (!MathUtil.IsInAngle(los, from, angle))
            {
                continue;
            }

            results.Add(other);
#if UNITY_EDITOR && !DISABLE_DEBUG_DRAW
            DebugUtil.DrawCross(otherCenter, Color.red);
#endif
        }

        return results.Count;
    }

    [MenuItem("GameObject / Actor Util / Create Actor with Template")]
    private static void FabricateGameObject(MenuCommand menuCommand)
    {
        var gameObject = menuCommand.context as GameObject;
        if (!gameObject)
        {
            return;
        }

        var rb = gameObject.EnsureComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.EnsureComponent<Actor>();
        var brainTrans = gameObject.transform.EnsureChild("Brain");
        brainTrans.EnsureComponent<Brain>();

        EditorUtility.SetDirty(gameObject);
    }
}