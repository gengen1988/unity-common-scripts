using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/**
 * notes that these methods are not thread safe
 */
// public static class ActorUtil
// {
//     private static readonly List<Collider2D> _queryBuffer = new();
//
//     public static bool IsAlive(this Actor actor)
//     {
//         return actor && actor.StateMachine.CurrentState is Actor.Neutral;
//     }
//
//     public static bool IsKilled(this Actor actor)
//     {
//         if (!actor)
//         {
//             return true;
//         }
//
//         if (actor.StateMachine.CurrentState is null)
//         {
//             return true;
//         }
//
//         if (actor.StateMachine.CurrentState is Actor.Despawning)
//         {
//             return true;
//         }
//
//         return false;
//     }
//
//     public static void Kill(this Actor actor)
//     {
//         actor?.StateMachine.Send(Actor.MSG_KILL);
//     }
//
//     // public static bool TryGetActor(this Collider2D col, out Actor actor)
//     // {
//     //     if (col.attachedRigidbody)
//     //     {
//     //         return col.attachedRigidbody.TryGetComponent(out actor);
//     //     }
//     //     else
//     //     {
//     //         return col.TryGetComponent(out actor);
//     //     }
//     // }
//
//     public static int FindOtherActorCircle(this Actor actor, float radius, List<Actor> results)
//     {
//         return FindOtherActorArc(actor, Vector2.right, 360f, radius, results);
//     }
//
//     public static int FindOtherActorArc(this Actor selfActor,
//         Vector2 from,
//         float angle,
//         float radius,
//         List<Actor> results)
//     {
//         var selfCenter = selfActor.CenterPosition;
//
// #if UNITY_EDITOR && !DISABLE_DEBUG_DRAW
//         DebugUtil.DrawWireArc2D(selfCenter, from, angle, radius, Color.yellow);
// #endif
//
//         var targetLayer = CustomLayer.Volume;
//         var filter = new ContactFilter2D
//         {
//             useLayerMask = true,
//             useTriggers = true,
//             layerMask = 1 << (int)targetLayer,
//         };
//
//         results.Clear();
//         Physics2D.OverlapCircle(selfCenter, radius, filter, _queryBuffer);
//         foreach (var found in _queryBuffer)
//         {
//             if (!found.TryGetComponent(out Actor otherActor))
//             {
//                 var enumName = Enum.GetName(typeof(CustomLayer), targetLayer);
//                 Debug.LogAssertion($"{enumName} layer collider should with actor root", found);
//                 continue;
//             }
//
//             // other should be alive
//             if (!otherActor.IsAlive())
//             {
//                 continue;
//             }
//
//             // remove not in angle range
//             var otherCenter = otherActor.CenterPosition;
//             var los = otherCenter - selfCenter;
//             if (!MathUtil.IsInAngle(los, from, angle))
//             {
//                 continue;
//             }
//
//             results.Add(otherActor);
//
// #if UNITY_EDITOR && !DISABLE_DEBUG_DRAW
//             DebugUtil.DrawCross(otherCenter, Color.red);
// #endif
//         }
//
//         return results.Count;
//     }
//
//     public static Actor GetActorInHierarchy(this Transform from, bool includeSelf = false)
//     {
//         var actor = from.GetComponentInParent<Actor>();
//         if (!actor)
//         {
//             return null;
//         }
//
//         if (includeSelf)
//         {
//             return actor;
//         }
//
//         if (from.IsChildOf(actor.transform))
//         {
//             return null;
//         }
//
//         return actor;
//     }
// }