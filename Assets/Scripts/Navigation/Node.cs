using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
	public Vector2 position;
	public bool walkable;

	public int x;
	public int y;

	public int gCost;
	public int hCost;
	public int fCost
	{
		get { return gCost + hCost; }
	}

	public bool reservedByTroop;

	public Node parent;

	private int heapIndex;

	public Node(Vector2 position, bool walkable, int x, int y)
	{
		this.position = position;
		this.walkable = walkable;
		this.x = x;
		this.y = y;
	}

	public int HeapIndex
	{
		get { return heapIndex; }
		set { heapIndex = value; }
	}

	public int CompareTo(Node other)
	{
		int compare = fCost.CompareTo(other.fCost);

		if (compare == 0) // hCost is the tiebreaker when fCosts are the same
		{
			compare = hCost.CompareTo(other.hCost);
		}

		return -compare; // Int CompareTo returns 1 if this > other. We want -1 if this > other
	}
}
