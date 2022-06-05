using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ATroop : MonoBehaviour
{
	[SerializeField] private Sprite defaultSprite;
	[SerializeField] private Sprite outlineSprite;

	[SerializeField] private float moveSpeed;

	// Ultimate destination (where troop stops)
	private Node myDestinationNode;
	// Current node (where troop currently is on grid)
	private Node myCurrNode;

	private Coroutine moveCR;

	public void OutlineTroop()
	{
		GetComponent<SpriteRenderer>().sprite = outlineSprite;
	}

	public void UnOutlineTroop()
	{
		GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}

	public void LeadTroops(Vector2 destination)
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.nextPosition = transform.position;
		agent.SetDestination(destination);
	}

	public void ReleaseCurrNode()
	{
		if (myCurrNode != null)
			myCurrNode.reservedByTroop = false;
	}

	public void ReleaseDestinationNode()
	{
		if (myDestinationNode != null) // Don't release if troop is already at destination
			myDestinationNode.reservedByTroop = false;

		myDestinationNode = null;
	}

	// myIndex is this unit's position in the group array. Used to get the offset of this unit from destination
	public void IssueMove(Vector2 destination, ATroop[] group, int myIndex)
	{
		Pathfinder pathfinder = Pathfinder.instance;

		(int x, int y) myOffset;
		if (group.Length > 0)
		{
			int formationWidth = Mathf.CeilToInt(Mathf.Sqrt(group.Length));

			myOffset = (myIndex % formationWidth, myIndex / formationWidth);
		}
		else
			myOffset = (0,0);

		Node destinationNode = pathfinder.GetNodeFromWorldPos(destination, myOffset);
		List<Node> waypoints = pathfinder.FindPath(transform.position, destinationNode.position);

		if (myCurrNode != null)
			myCurrNode.reservedByTroop = true;

		if (waypoints.Count == 0) // If the path is just to the exact same position troop currently is at, do nothing
			return;
		else
			myDestinationNode = waypoints[waypoints.Count - 1];

		if (myDestinationNode != null)
			myDestinationNode.reservedByTroop = true;

		if (waypoints.Count > 0)
		{
			if (moveCR != null)
				StopCoroutine(moveCR);
			moveCR = StartCoroutine(MoveAlongWaypoints(waypoints, destination, group, myIndex));
		}

		// velocity = (destination - (Vector2)transform.position).normalized;
	}

	private IEnumerator MoveAlongWaypoints(List<Node> waypoints, Vector2 clickedPos, ATroop[] group, int myIndex)
	{
		for (int i = 0; i < waypoints.Count; i++)
		{
			Node localDestination = waypoints[i];

			// If unwalkable for timeUntilNewPath seconds, then find new path
			float timeUntilNewPath = 0.6f;
			while (!localDestination.walkable || (localDestination.reservedByTroop && localDestination != myCurrNode))
			{
				timeUntilNewPath -= Time.deltaTime;

				if (timeUntilNewPath <= 0)
				{
					IssueMove(clickedPos, group, myIndex);
					ReleaseDestinationNode();
					yield break;
				}

				yield return null;
			}

			if (myCurrNode != null)
				myCurrNode.reservedByTroop = false;

			myCurrNode = localDestination;

			myCurrNode.reservedByTroop = true;

			// destination = waypoints[i].position + Random.insideUnitCircle * 0.25f;

			while ((Vector2)transform.position != localDestination.position)
			{
				transform.position = Vector3.MoveTowards(transform.position, myCurrNode.position, moveSpeed * Time.deltaTime);
				
				yield return null;
			}
		}
	}

	public virtual void HandleInput(InputAction action)
	{
		print("Handling input for " + gameObject.name);
		if (action == InputAction.Move)
		{
			// StartCoroutine(ForceMove());
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.nextPosition = transform.position;
			Vector2 dest = InputHandler.GetMousePos();
			agent.SetDestination(dest);
			// velocity = (dest - (Vector2)transform.position).normalized * agent.speed;
		}
	}
}
