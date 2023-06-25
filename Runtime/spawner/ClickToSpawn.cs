using UnityEngine;


public class ClickToSpawn : Spawner
{
    public float spawnRate = 10;
    public EventChannel onSpawnEvent;
    public int mouseButton;

    float spawnAt;

    void Update()
    {
        if (Input.GetMouseButton(mouseButton))
        {
            if (Time.time > spawnAt + 1 / spawnRate)
            {
                if (onSpawnEvent) onSpawnEvent.Emit();
                Spawn();
                spawnAt = Time.time;
            }
        }
    }
}