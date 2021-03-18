using System.Linq;
using UnityEngine;

namespace Alsorunning.Common.Steering
{
    public class PathFollowing : SteeringBehaviour
    {
        public SteeringEntity self;
        public Vector3[] path;

        // preference
        public float waypointSeekDistance;
        public float maxSpeed;

        int currentWaypointIndex;

        public override Vector3 Steer()
        {
            var currentWaypoint = GetCurrentWaypoint();
            var distance = Vector3.Distance(self.position, currentWaypoint);
            if (distance < waypointSeekDistance)
            {
                SetNextWaypoint();
            }

            if (!Finished())
            {
                return SteeringUtil.Seek(self.position, self.velocity, currentWaypoint, maxSpeed);
            }
            else
            {
                return Vector3.zero;
            }
        }

        bool Finished()
        {
            if (path == null) return true;
            
            return currentWaypointIndex > path.Length;
        }

        void SetNextWaypoint()
        {
            currentWaypointIndex++;
        }

        Vector3 GetCurrentWaypoint()
        {
            if (path == null) return Vector3.zero;

            if (currentWaypointIndex >= path.Length)
            {
                return path.Last();
            }

            return path[currentWaypointIndex];
        }

        public void SetCourse(Vector3[] course)
        {
            path = course;
            currentWaypointIndex = 0;
        }
    }
}