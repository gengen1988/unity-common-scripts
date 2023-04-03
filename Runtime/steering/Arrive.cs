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
            var los = target.position - self.position;
            return SteeringUtil.Arrive(los, self.velocity, maxSpeed, arrivalDistance);
        }
    }
}