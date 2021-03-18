using UnityEngine;

namespace Alsorunning.Common.Steering
{
    public class Seek : SteeringBehaviour
    {
        public SteeringEntity self;
        public SteeringEntity target;

        public float maxSpeed;

        public override Vector3 Steer()
        {
            return SteeringUtil.Seek(self.position, self.velocity, target.position, maxSpeed);
        }

        public static SteeringBehaviour Create(SteeringEntity self, SteeringEntity target, float maxSpeed, float amount)
        {
            return new Seek
            {
                self = self,
                target = target,
                maxSpeed = maxSpeed,
                amount = amount
            };
        }
    }
}