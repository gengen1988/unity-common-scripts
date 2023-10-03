using UnityEngine;

public class AreaSpawner : CountSpawner
{
	public Vector3 extends = new Vector3(1, 1, 0);

	void OnDrawGizmos()
	{
		var scale = transform.localScale;
		var size = new Vector3(
			extends.x / scale.x,
			extends.y / scale.y,
			extends.z / scale.z
		) * 2;

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = new Color(0, 0, 0, .25f);
		Gizmos.DrawCube(Vector3.zero, size);
	}

	void OnDrawGizmosSelected()
	{
		var scale = transform.localScale;
		var size = new Vector3(
			extends.x / scale.x,
			extends.y / scale.y,
			extends.z / scale.z
		) * 2;

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(Vector3.zero, size);
	}

	protected override GameObject Spawn()
	{
		var rotation = transform.rotation;
		var pos = RandomUtil.RandomPointInBox(transform.position, extends, rotation);
		return Spawn(pos, rotation);
	}
}