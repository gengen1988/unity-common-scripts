using System.Collections.Generic;
using System.Linq;
using Alsorunning.Common.Steering;
using SensorToolkit;

public abstract class GroupBehaviour : SteeringBehaviour
{
    public Sensor perception;

    protected List<T> GetNeighbors<T>()
    {
        return perception.DetectedObjects
            .Select(neighbor => neighbor.GetComponent<T>())
            .Where(instance => instance != null)
            .ToList();
    }
}