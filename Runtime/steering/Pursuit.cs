using UnityEngine;

namespace Alsorunning.Common.Steering
{
    public class Pursuit : SteeringBehaviour
    {
        public SteeringEntity self;
        public SteeringEntity target;

        public float maxSpeed;

        public override Vector3 Steer()
        {
            var los = target.position - self.position;
            return SteeringUtil.Pursue(los, self.velocity, target.velocity, maxSpeed);
        }
    }
}