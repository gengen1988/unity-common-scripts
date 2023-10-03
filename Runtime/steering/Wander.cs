using Alsorunning.Common.Steering;
using UnityEngine;

public class Wander : SteeringBehaviour
{
    public Transform self;
    public float wanderJitter;
    public float wanderRadius;
    public float wanderDistance;

    Vector3 wanderTarget;

    public override Vector3 Steer()
    {
        if (wanderTarget.magnitude == 0)
        {
            wanderTarget = RandomUtil.RandomPointInRectangle(Vector3.zero, Vector3.one * wanderRadius, Quaternion.identity);
        }

        return SteeringUtil.Wander(self, wanderDistance, wanderRadius, wanderJitter, ref wanderTarget);
    }

    public static Wander Create(Transform self, float distance, float radius, float jitter, float amount)
    {
        return new Wander
        {
            self = self,
            wanderDistance = distance,
            wanderRadius = radius,
            wanderJitter = jitter,
            amount = amount
        };
    }
}