using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
	public static Pathfinder instance;

	public Transform seeker, target;

	private Grid grid;

	private void Awake()
	{
		instance = this;

		grid = GetComponent<Grid>();
	}

	private void Update()
	{
		// FindPath(seeker.position, target.position);
	}

	public Node GetNodeFromWorldPos(Vector2 position, (int x, int y) nodeOffset)
	{
		return grid.GetNodeFromWorldPos(position, nodeOffset);
	}

	// endOffset is the number of nodes to the right and upwards the destination should be at (from end)
	public List<Node> FindPath(Vector2 start, Vector2 end, bool allowPathThroughReservedNodes = false)
	{
		Node startNode = grid.GetNodeFromWorldPos(start);
		Node endNode = grid.GetNodeFromWorldPos(end);

		// If destination is not walkable, use the closest node to destination that is walkable instead
		if (!endNode.walkable || endNode.ownerTroop != null)
		{
			endNode = grid.GetClosestWalkableNode(endNode);
		}

		MinHeap<Node> openSet = new MinHeap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node currNode = openSet.RemoveFirst();
			closedSet.Add(currNode);

			if (currNode == endNode)
			{
				grid.gizmosPath = RetracePath(startNode, endNode);
				return RetracePath(startNode, endNode);
			}

			foreach (Node neighbor in grid.GetNeighbors(currNode))
			{
				if (!neighbor.walkable || closedSet.Contains(neighbor)) // TODO: Probably check here for clearance based stuff I think?
					continue;

				if (!allowPathThroughReservedNodes && neighbor.ownerTroop != null)
					continue;

				int newMoveCost = currNode.gCost + GetDistance(currNode, neighbor);

				if (newMoveCost < neighbor.gCost || !openSet.Contains(neighbor))
				{
					neighbor.gCost = newMoveCost;
					neighbor.hCost = GetDistance(neighbor, endNode);
					neighbor.parent = currNode;

					if (!openSet.Contains(neighbor))
						openSet.Add(neighbor);
				}
			}
		}

		return null;
	}

	private List<Node> RetracePath(Node start, Node end)
	{
		List<Node> path = new List<Node>();
		Node curr = end;

		while (curr != start)
		{
			path.Add(curr);
			curr = curr.parent;
		}

		path.Reverse();

		return path;
	}

	private int GetDistance(Node a, Node b)
	{
		int dstX = Mathf.Abs(a.x - b.x);
		int dstY = Mathf.Abs(a.y - b.y);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		else
			return 14 * dstX + 10 * (dstY - dstX);
	}
}
