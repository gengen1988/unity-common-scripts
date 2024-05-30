using System.Collections.Generic;
using UnityEngine;

public static class ForceDirection
{
    /**
     * move points to min distance fairly
     */
    public static bool Execute(IList<Vector2> items, float minDistance, float pushForce)
    {
        return items.IterateSolve(DistanceCheck, MoveAway) >= 0;

        bool DistanceCheck(Vector2 item1, Vector2 item2)
        {
            return Vector2.Distance(item1, item2) < minDistance;
        }

        void MoveAway(IList<Vector2> list, int index1, int index2)
        {
            Vector2 item1 = list[index1];
            Vector2 item2 = list[index2];

            Vector2 los = item1 - item2;
            Vector2 direction = los.normalized;
            Vector2 force = direction * pushForce;

            item1 += force;
            item2 -= force;

            list[index1] = item1;
            list[index2] = item2;
        }
    }
}