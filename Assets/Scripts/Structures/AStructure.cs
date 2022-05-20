using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class AStructure : MonoBehaviour
{
	[SerializeField] protected List<ActionItem> actionsList;
	[SerializeField] protected float range;
	[SerializeField] protected string structureName;


	// The action list that will show up when this structure is clicked
	public virtual List<ActionItem> GetActionList()
	{
		return actionsList;
	}

	public virtual float GetRangeRadius()
	{
		return range;
	}

	public virtual string GetName()
	{
		return structureName;
	}

	public abstract IEnumerator HandleInput(InputAction action);



	// Returns the point on the collider that a connection point should be at to connect to a point at "pos"
	public Vector2 GetConnectionPointOnCollider(Vector2 pos)
	{
		BoxCollider2D boxCol = GetComponent<BoxCollider2D>();

		Bounds bounds = boxCol.bounds;
		
		float dangle = HelperFunctions.GetDAngleTowards(bounds.center, pos);

		// Top point
		if (dangle >= 30f && dangle <= 150f)
		{
			return new Vector2(bounds.center.x, bounds.max.y);
		}
		// Left point
		else if (dangle > 150f || dangle < -150f)
		{
			return new Vector2(bounds.min.x, bounds.center.y);
		}
		// Bottom point
		else if (dangle >= -150f && dangle <= -30f)
		{
			return new Vector2(bounds.center.x, bounds.min.y);
		}
		// Right point
		else if (dangle < 30f && dangle > -30f)
		{
			return new Vector2(bounds.max.x, bounds.center.y);
		}

		// Shouldn't ever occur, but just in case, default to the top position
		return new Vector2(bounds.center.x, bounds.min.y);
	}

	public Vector2 GetCenter()
	{
		BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
		return boxCol.bounds.center;
	}
}
