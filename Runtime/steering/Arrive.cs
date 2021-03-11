using UnityEngine;

namespace Alsorunning.Common.Steering
{
    public class Arrive : SteeringBehaviour
    {
        public SteeringEntity self;
        public SteeringEntity target;

        public float maxSpeed;
        public float arrivalDistance;

        public override Vector3 Steer()
        {
            return SteeringUtil.Arrive(self.position, self.velocity, target.position, maxSpeed, arrivalDistance);
        }
    }
}