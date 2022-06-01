using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleRenderer : MonoBehaviour
{
	[SerializeField] private Material defaultMaterial;
	[SerializeField] private Material dottedMaterial;

	private LineRenderer lr;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
	}

	public void DrawRectangle(Vector2 point1, Vector2 point2, float lineWidth, bool dotted)
	{
		lr.numCapVertices = 6;
		lr.loop = true;

		lr.startWidth = lineWidth;
		lr.endWidth = lineWidth;

		float minX;
		float maxX;
		if (point1.x <= point2.x)
		{
			minX = point1.x;
			maxX = point2.x;
		}
		else
		{
			minX = point2.x;
			maxX = point1.x;
		}

		float minY;
		float maxY;
		if (point1.y <= point2.y)
		{
			minY = point1.y;
			maxY = point2.y;
		}
		else
		{
			minY = point2.y;
			maxY = point1.y;
		}

		lr.positionCount = 8;

		// Bottom left
		lr.SetPosition(0, new Vector2(minX, minY));
		// Bottom middle
		lr.SetPosition(1, new Vector2((minX + maxX) / 2, minY));
		// Bottom right
		lr.SetPosition(2, new Vector2(maxX, minY));
		// Right middle
		lr.SetPosition(3, new Vector2(maxX, (minY + maxY) / 2));
		// Top right
		lr.SetPosition(4, new Vector2(maxX, maxY));
		// Top middle
		lr.SetPosition(5, new Vector2((minX + maxX) / 2, maxY));
		// Top left
		lr.SetPosition(6, new Vector2(minX, maxY));
		// Left middle
		lr.SetPosition(7, new Vector2(minX, (minY + maxY) / 2));

		if (dotted)
		{
			lr.material = dottedMaterial;
			lr.materials[0].mainTextureScale = Vector2.one;
			lr.textureMode = LineTextureMode.Tile;
		}
		else
		{
			lr.material = defaultMaterial;
			lr.materials[0].mainTextureScale = Vector2.one;
		}
	}

	public void SetColor(Color color)
	{
		lr.startColor = color;
		lr.endColor = color;
	}
}
