using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
	public static float GetRAngleTowards(Vector2 startPos, Vector2 endPos)
	{
		Vector2 diff = endPos - startPos;

		return Mathf.Atan2(diff.y, diff.x);
	}

	// Returns the angle in degrees from startPos to endPos [-180, 180]
	public static float GetDAngleTowards(Vector2 startPos, Vector2 endPos)
	{
		return GetRAngleTowards(startPos, endPos) * Mathf.Rad2Deg;
	}

	// Returns the angle in radians from startPos to endPos
	// Range: [0, 2pi]
	public static float GetRAngleTowardsPositive(Vector2 startPos, Vector2 endPos)
	{
		Vector2 diff = endPos - startPos;

		float rangle = Mathf.Atan2(diff.y, diff.x);
		return (rangle + (2 * Mathf.PI)) % (2 * Mathf.PI);
	}

	public static float GetDangleTowardsPositive(Vector2 startPos, Vector2 endPos)
	{
		return GetRAngleTowardsPositive(startPos, endPos) * Mathf.Rad2Deg;
	}
}
