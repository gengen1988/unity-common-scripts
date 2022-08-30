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
            return SteeringUtil.Pursue(self.position, self.velocity, target.position, target.velocity, maxSpeed);
        }
    }
}