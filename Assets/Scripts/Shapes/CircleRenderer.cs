using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
	[SerializeField] private Material normalMaterial;
	[SerializeField] private Material dottedMaterial;

	private LineRenderer lr;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
	}

	public void DrawCircle(float radius, float lineWidth, bool dotted)
	{
		lr.numCapVertices = 6;

		int numberOfVertices = (int)radius * 12;

		lr.startWidth = lineWidth;
		lr.endWidth = lineWidth;
		lr.positionCount = numberOfVertices + 1;

		int pointCount = numberOfVertices + 1; // add extra point to make startpoint and endpoint the same to close the circle
		Vector3[] points = new Vector3[pointCount];

		for (int i = 0; i < pointCount; i++)
		{
			var rad = Mathf.Deg2Rad * (i * 360f / numberOfVertices);
			points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
		}
		points[pointCount - 1] = points[0];

		lr.SetPositions(points);

		if (dotted)
		{
			lr.material = dottedMaterial;
			lr.materials[0].mainTextureScale = Vector2.one;
			lr.textureMode = LineTextureMode.RepeatPerSegment;
		}
		else
		{
			lr.material = normalMaterial;
			lr.materials[0].mainTextureScale = Vector2.one;
		}
	}

	public void SetColor(Color color)
	{
		lr.startColor = color;
		lr.endColor = color;
	}
}
