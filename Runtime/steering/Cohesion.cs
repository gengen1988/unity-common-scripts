using System.Linq;
using Alsorunning.Common.Steering;
using SensorToolkit;
using UnityEngine;

public class Cohesion : GroupBehaviour
{
    public SteeringEntity self;
    public float maxSpeed;

    public override Vector3 Steer()
    {
        var neighbors = GetNeighbors<Transform>();
        if (neighbors.Count > 0)
        {
            var neighborPositions = neighbors.Select(neighbor => neighbor.position).ToArray();
            var centerOfMass = MathUtil.CenterOfMass(neighborPositions);
            return SteeringUtil.Seek(self.position, self.velocity, centerOfMass, maxSpeed);
        }

        return Vector3.zero;
    }

    public static Cohesion Create(SteeringEntity self, Sensor perception, float maxSpeed, float amount)
    {
        return new Cohesion
        {
            self = self,
            perception = perception,
            maxSpeed = maxSpeed,
            amount = amount
        };
    }
}