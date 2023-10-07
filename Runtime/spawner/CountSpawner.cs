using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountSpawner : Spawner
{
	public int maxNumber = 3;
	public float produceTime = 1f;

	private readonly List<GameObject> spawned = new List<GameObject>();

	private void Start()
	{
		StartCoroutine(SpawnTask());
	}

	private IEnumerator SpawnTask()
	{
		while (true)
		{
			while (!enabled) yield return null;

			// count check
			spawned.RemoveAll(instance => !instance);
			if (spawned.Count >= maxNumber)
			{
				yield return null;
				continue;
			}

			// produce
			if (produceTime > 0)
			{
				yield return new WaitForSeconds(produceTime);
			}

			var newObj = Spawn();
			spawned.Add(newObj);
		}
	}
}