using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	[SerializeField] private LayerMask unwalkableLayer;
	[SerializeField] private Vector2 gridSize;
	[SerializeField] private float nodeRadius;

	private Node[,] grid;

	private float nodeDiameter;
	private int gridWidth, gridHeight;

	private void Start()
	{
		nodeDiameter = 2 * nodeRadius;

		gridWidth = Mathf.RoundToInt(gridSize.x / nodeDiameter);
		gridHeight = Mathf.RoundToInt(gridSize.y / nodeDiameter);

		CreateGrid();
	}

	public int MaxSize
	{
		get { return gridWidth * gridHeight; }
	}

	private void CreateGrid()
	{
		grid = new Node[gridWidth, gridHeight];

		Vector2 bottomLeft = Vector2.zero - new Vector2(gridSize.x / 2, gridSize.y / 2);

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				Vector2 pos = bottomLeft + new Vector2(x * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius);
				bool walkable = Physics2D.OverlapCircle(pos, nodeRadius, unwalkableLayer) == null;
				grid[x, y] = new Node(pos, walkable, x, y);
			}
		}
	}

	public List<Node> GetNeighbors(Node node)
	{
		List<Node> neighbors = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) // This is the given node, don't need to check it
					continue;

				int checkX = node.x + x;
				int checkY = node.y + y;

				if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
				{
					neighbors.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbors;
	}

	public Node GetNodeFromWorldPos(Vector2 worldPos)
	{
		return GetNodeFromWorldPos(worldPos, (0, 0));
	}

	public Node GetNodeFromWorldPos(Vector2 worldPos, (int x, int y) nodeOffset)
	{
		float percentX = Mathf.Clamp01(worldPos.x / gridSize.x + 0.5f);
		float percentY = Mathf.Clamp01(worldPos.y / gridSize.y + 0.5f);

		int x = Mathf.FloorToInt(Mathf.Clamp(gridWidth * percentX, 0, gridWidth - 1));
		int y = Mathf.FloorToInt(Mathf.Clamp(gridHeight * percentY, 0, gridHeight - 1));

		// TODO: Out of bounds error here
		return grid[x + nodeOffset.x, y + nodeOffset.y];
	}

	public Node GetClosestWalkableNode(Node node)
	{
		if (node.walkable && !node.reservedByTroop)
			return node;

		Queue<Node> openSet = new Queue<Node>(MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Enqueue(node);
		
		while (openSet.Count > 0)
		{
			Node currNode = openSet.Dequeue();

			closedSet.Add(currNode);

			if (currNode.walkable && !currNode.reservedByTroop)
				return currNode;

			foreach (Node neighbor in GetNeighbors(currNode))
			{
				if (closedSet.Contains(neighbor))
					continue;

				if (!openSet.Contains(neighbor))
					openSet.Enqueue(neighbor);
			}
		}

		return null; // Should only occur if entire grid is unwalkable...
	}

	public List<Node> gizmosPath;
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector2(gridSize.x, gridSize.y));

		if (grid != null)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = n.walkable ? Color.gray : Color.red;

				if (gizmosPath != null && gizmosPath.Contains(n))
				{
					Gizmos.color = Color.black;
				}

				if (n.reservedByTroop)
					Gizmos.color = Color.green;

				Gizmos.DrawCube(n.position, Vector2.one * (nodeDiameter - 0.1f));
			}
		}
	}
}
