using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/**
 * notes that these methods are not thread safe
 */
public static class ActorUtil
{
    private static readonly List<Collider2D> _queryBuffer = new();

    public static bool IsAlive(this Actor actor)
    {
        return actor && actor.StateMachine.CurrentState is Actor.Normal;
    }

    public static bool IsKilled(this Actor actor)
    {
        if (!actor)
        {
            return true;
        }

        if (actor.StateMachine.CurrentState is null)
        {
            return true;
        }

        if (actor.StateMachine.CurrentState is Actor.Despawning)
        {
            return true;
        }

        return false;
    }

    public static Actor Spawn(this Actor prefab, Vector3 position, Quaternion rotation)
    {
        return PoolUtil.Spawn(prefab, position, rotation);
    }

    public static void Kill(this Actor actor)
    {
        actor?.StateMachine.Send(Actor.MSG_KILL);
    }

    public static bool TryGetActor(this Collider2D col, out Actor actor)
    {
        if (col.attachedRigidbody)
        {
            return col.attachedRigidbody.TryGetComponent(out actor);
        }
        else
        {
            return col.TryGetComponent(out actor);
        }
    }

    public static int FindOtherActorCircle(this Actor actor, float radius, List<Actor> results)
    {
        return FindOtherActorArc(actor, Vector2.right, 360f, radius, results);
    }

    public static int FindOtherActorArc(this Actor actor, Vector2 from, float angle, float radius, List<Actor> results)
    {
        var sqrRadius = radius * radius;
        var actorTransform = actor.transform;
        var center = actorTransform.position;
        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Visualbox")
        };

#if UNITY_EDITOR && !DISABLE_DEBUG_DRAW
        DebugTools.DrawWireArc2D(center, from, angle, radius, Color.yellow);
#endif

        results.Clear();
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

            // other should be alive
            if (!other.IsAlive())
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
            DebugTools.DrawCross(otherCenter, Color.red);
#endif
        }

        return results.Count;
    }

#if UNITY_EDITOR
    [MenuItem("GameObject / Actor Util / Create Actor")]
    private static void FabricateGameObject(MenuCommand menuCommand)
    {
        var gameObject = menuCommand.context as GameObject;
        if (!gameObject)
        {
            gameObject = new GameObject("NewActor");
        }

        // setup actor object
        var rb = gameObject.EnsureComponent<Rigidbody2D>();
        gameObject.EnsureComponent<ArcadeMovement>();
        gameObject.EnsureComponent<Actor>();
        gameObject.EnsureComponent<Pawn>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // visual
        var modelTrans = gameObject.transform.EnsureChild("Model");
        var spriteTrans = modelTrans.EnsureChild("Sprite");
        var spriteRenderer = spriteTrans.EnsureComponent<SpriteRenderer>();
        var trianglePath = AssetDatabase.GUIDToAssetPath("75f5f34dc1b5347e0b8351032682f224"); // triangle in unity 2d package
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(trianglePath);
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.green;
        spriteTrans.localRotation = Quaternion.Euler(0, 0, -90f);
        spriteTrans.localScale = new Vector3(.5f, 1f, 1f);

        // control
        var brainTrans = gameObject.transform.EnsureChild("Brain");
        brainTrans.EnsureComponent<Brain>();
        brainTrans.EnsureComponent<BrainInput>();

        EditorUtility.SetDirty(gameObject);
    }
#endif
}