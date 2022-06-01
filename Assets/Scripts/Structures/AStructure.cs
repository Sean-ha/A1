using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class AStructure : MonoBehaviour
{
	[SerializeField] protected List<ActionItem> actionsList;
	[SerializeField] protected float range;
	[SerializeField] protected string structureName;

	// Cost to build one of these structures
	[SerializeField] public int shardCost;
	[SerializeField] public int dustCost;
	[SerializeField] public int energyCost;
	[SerializeField] public int foodCost;


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

	public List<Collider2D> CheckCollisionsInRange(LayerMask layerMask)
	{
		BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(layerMask);

		List<Collider2D> collisions = new List<Collider2D>();

		Physics2D.OverlapCircle(boxCol.bounds.center, range, filter, collisions);

		return collisions;
	}

	// Given a position, return the position on the range circle that is closest to that pos. Just returns pos is it's inside the range
	public Vector2 GetClosestPointOnRange(Vector2 pos)
	{
		BoxCollider2D boxCol = GetComponent<BoxCollider2D>();

		Vector2 center = boxCol.bounds.center;

		if (Vector2.Distance(center, pos) <= range)
			return pos;

		Vector2 dir = pos - center;

		return center + (dir.normalized * range);
	}
}
