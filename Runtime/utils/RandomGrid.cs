using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RandomGrid
{
	[Header("Metric")]
	public Vector2 Size = new Vector2(12, 9);
	public int HorizontalCount = 3, VerticalCount = 2;

	[Range(-.5f, .5f)]
	public float EvenOffsetRow;
	[Range(-.5f, .5f)]
	public float EvenOffsetColumn;

	[Header("Randomness")]
	public Vector2 MarginOuter;
	public Vector2 MarginInner;
	public float MinDistance;
	public float PushForce;

	private Vector2 CellSize => new Vector2(Size.x / HorizontalCount, Size.y / VerticalCount);

	public void DrawGizmos(Vector3 center)
	{
		Gizmos.DrawWireCube(center, Size);
		Vector2 outerSize = CellSize - MarginOuter;
		Vector2 innerSize = MarginInner;
		foreach (Vector3 point in Anchors(center))
		{
			Gizmos.color = new Color(1, 0, 0, 1f);
			Gizmos.DrawCube(point, innerSize);

			Gizmos.color = new Color(0, 1, 0, .333f);
			Gizmos.DrawCube(point, outerSize);

			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(point, outerSize);
		}
	}

	public IEnumerable<Vector2> RandomPoints(Vector2 center)
	{
		Vector2 outerSize = CellSize - MarginOuter;
		Vector2 innerSize = MarginInner;
		Vector2[] points = Anchors(center)
			.Select(point => point + RandomInCell(outerSize, innerSize))
			.ToArray();

		ForceDirection.Execute(points, MinDistance, PushForce);
		return points;
	}

	/**
	 * return random point between outer and inner
	 */
	public Vector2 RandomInCell(Vector2 outerSize, Vector2 innerSize)
	{
		float lineRad = Random.Range(0, Mathf.PI / 2);
		Vector2 extentOuter = outerSize / 2;
		Vector2 extentInner = innerSize / 2;
		Vector2 pointInner = LineBoxIntersect(lineRad, extentInner);
		Vector2 pointOuter = LineBoxIntersect(lineRad, extentOuter);
		float distance = Vector2.Distance(pointInner, pointOuter);
		float delta = Random.Range(0, distance);
		float length = pointInner.magnitude + delta;
		Vector2 point = Quaternion.Euler(0, 0, lineRad * Mathf.Rad2Deg) * Vector2.right * length;

		point.x *= Random.value > .5f ? 1 : -1;
		point.y *= Random.value > .5f ? 1 : -1;
		return point;
	}

	private Vector2 LineBoxIntersect(float lineRad, Vector2 box)
	{
		float cornerRad = Mathf.Atan2(box.y, box.x);
		float a = Mathf.Tan(lineRad);
		if (lineRad < cornerRad)
		{
			float y = a * box.x;
			return new Vector2(box.x, y);
		}
		else
		{
			float x = box.y / a;
			return new Vector2(x, box.y);
		}
	}

	private IEnumerable<Vector2> Anchors(Vector2 center)
	{
		float cellWidth = CellSize.x;
		float cellHeight = CellSize.y;

		float left = center.x - Size.x / 2;
		float bottom = center.y - Size.y / 2;
		float paddingX = cellWidth / 2;
		float paddingY = cellHeight / 2;
		float evenPaddingX = cellWidth * EvenOffsetRow;
		float evenPaddingY = cellHeight * EvenOffsetColumn;

		for (int i = 0; i < VerticalCount; ++i)
		{
			for (int j = 0; j < HorizontalCount; ++j)
			{
				float x = left + paddingX + j * cellWidth;
				float y = bottom + paddingY + i * cellHeight;

				if (i % 2 == 0)
				{
					x += evenPaddingX;
				}

				if (j % 2 == 0)
				{
					y += evenPaddingY;
				}

				yield return new Vector2(x, y);
			}
		}
	}
}