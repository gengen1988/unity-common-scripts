﻿using UnityEngine;

namespace Alsorunning.Common.Steering
{
    public class SteeringAgent
    {
        public float maxSteeringForce;
        public SteeringBehaviour[] steeringBehaviours;

        public Vector3 DoSteering()
        {
            var steeringForce = Vector3.zero;

            // do steering with weight
            foreach (var behaviour in steeringBehaviours)
            {
                steeringForce += behaviour.Steer() * behaviour.amount;
            }

            // truncate
            Vector3.ClampMagnitude(steeringForce, maxSteeringForce);

            return steeringForce;
        }
    }

    public abstract class SteeringBehaviour
    {
        public float amount = 1f;

        public abstract Vector3 Steer();
    }

    public class SteeringEntity
    {
        public Vector3 position;
        public Vector3 velocity;
    }
}