using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{
    public static float TriangularOffset(float delta) => (Random.value - Random.value) * delta;
    public static T Pick<T>(IList<T> list) => list[Random.Range(0, list.Count)];

    public static Vector3 Position(Vector3 center, Vector3 extends, Quaternion rotation)
    {
        var x = Random.Range(-extends.x, extends.x);
        var y = Random.Range(-extends.y, extends.y);
        var z = Random.Range(-extends.z, extends.z);
        return center + rotation * new Vector3(x, y, z);
    }
}