using System.Collections.Generic;
using UnityEngine;

/**
 * if enabled, it decompose targets on start. otherwise you will need invoke it manually
 */
public class Decompose : MonoBehaviour
{
	public Transform[] Children;

	private GameObject[] _instances;

	private void Start()
	{
		Execute();
	}

	private void OnDestroy()
	{
		if (_instances == null)
		{
			return;
		}

		foreach (GameObject go in _instances)
		{
			if (go)
			{
				Destroy(go);
			}
		}
	}

	public void Execute()
	{
		if (Children == null)
		{
			return;
		}

		if (_instances != null)
		{
			Debug.LogError("can not decompose twice", this);
			return;
		}

		Transform self = transform;
		List<GameObject> instances = new List<GameObject>();
		foreach (Transform child in Children)
		{
			if (!child.IsChildOf(self))
			{
				Debug.LogWarning($"{child.name} is not child of this transform, should not be decomposed", this);
				continue;
			}

			if (!child.gameObject.activeSelf)
			{
				Debug.LogWarning($"{child.name} is deactivated, do not decompose", this);
				continue;
			}

			// instantiate and hide
			Transform instance = Instantiate(child, self.parent);
			child.gameObject.SetActive(false);

			// index
			instances.Add(instance.gameObject);
		}

		_instances = instances.ToArray();
	}
}