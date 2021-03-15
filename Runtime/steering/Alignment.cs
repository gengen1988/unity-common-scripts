using Alsorunning.Common.Steering;
using SensorToolkit;
using UnityEngine;

public class Alignment : GroupBehaviour
{
    public SteeringEntity self;

    public override Vector3 Steer()
    {
        var neighbors = GetNeighbors<BoidSteering>();

        if (neighbors.Count > 0)
        {
            var averageVelocity = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                var entity = neighbor.self;
                averageVelocity += entity.velocity;
            }

            averageVelocity /= neighbors.Count;

            return averageVelocity - self.velocity;
        }

        return Vector3.zero;
    }

    public static Alignment Create(SteeringEntity self, Sensor perception, float amount)
    {
        return new Alignment
        {
            self = self,
            perception = perception,
            amount = amount
        };
    }
}