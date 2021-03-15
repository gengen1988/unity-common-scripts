using Alsorunning.Common.Steering;
using SensorToolkit;
using UnityEngine;

public class Separation : GroupBehaviour
{
    public SteeringEntity self;

    public override Vector3 Steer()
    {
        var neighbors = GetNeighbors<Transform>();
        if (neighbors.Count > 0)
        {
            var steeringForce = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                var los = self.position - neighbor.position;
                steeringForce += los.normalized / los.magnitude;
            }

            return steeringForce;
        }

        return Vector3.zero;
    }

    public static Separation Create(SteeringEntity self, Sensor perception, float amount)
    {
        return new Separation
        {
            self = self,
            perception = perception,
            amount = amount
        };
    }
}