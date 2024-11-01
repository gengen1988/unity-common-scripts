using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public PositionProvider Provider;
    public GameObject ToBeSpawn;
    public int MaxAmount = 3;
    public float SpawnTime = 1f;

    private float _cooldownTime;
    private readonly List<GameObject> spawned = new List<GameObject>();

    private void FixedUpdate()
    {
        // count check
        spawned.RemoveAll(instance => !instance);
        if (spawned.Count >= MaxAmount)
        {
            return;
        }

        // produce
        if (_cooldownTime <= 0)
        {
            Vector3 position = Provider.GetRandomPosition();
            GameObject newObj = Instantiate(ToBeSpawn, position, Quaternion.identity);
            spawned.Add(newObj);
            _cooldownTime += SpawnTime;
        }

        _cooldownTime -= Time.deltaTime;
    }
}