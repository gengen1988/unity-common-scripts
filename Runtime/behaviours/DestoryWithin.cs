using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DestoryWithin : MonoBehaviour
{
    public GameObject[] Watching;

    private void FixedUpdate()
    {
        Tick();
    }

    private void Tick()
    {
        if (Watching.Any(go => go))
        {
            return;
        }

        Destroy(gameObject);
    }
}