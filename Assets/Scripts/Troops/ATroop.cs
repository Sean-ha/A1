using System;
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

	private List<Node> myWaypoints;

	private TroopGroup myGroup;
	private int myIndex;
	private InputAction lastAction;

	private Coroutine moveCR;
	private Coroutine attackCR;

	private bool moving; // Whether the unit is in motion or stopped (reached its destination)

	protected bool stoppedToAttack; // Whether the unit has paused movement in order to attack

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

	public void GiveWaypoints(List<Node> incoming, Vector2 clickedPos)
	{
		myWaypoints = incoming;

		if (moveCR != null)
			StopCoroutine(moveCR);
		moveCR = StartCoroutine(MoveAlongWaypoints(clickedPos));
	}

	public int SwapIndices(int incomingIndex)
	{
		int temp = myIndex;
		myIndex = incomingIndex;
		return temp;
	}

	public void ReleaseCurrNode()
	{
		if (myCurrNode != null)
			myCurrNode.ownerTroop = null;
	}

	public void ReleaseDestinationNode()
	{
		if (myDestinationNode != null) // Don't release if troop is already at destination
			myDestinationNode.ownerTroop = null;

		myDestinationNode = null;
	}

	// myIndex is this unit's position in the group array. Used to get the offset of this unit from destination
	public void IssueMove(Vector2 destination, TroopGroup group, int myIndex, InputAction action)
	{
		if (myGroup != null && myGroup != group) // Troop is part of a new group, remove self from previous group
		{
			myGroup.troopsInGroup.Remove(this);
			myGroup.NotifyTroopsOfIndexUpdate();
		}

		myGroup = group;
		this.myIndex = myIndex;
		lastAction = action;

		Pathfinder pathfinder = Pathfinder.instance;

		(int x, int y) myOffset;
		if (group.troopsInGroup.Count > 0)
		{
			int formationWidth = Mathf.CeilToInt(Mathf.Sqrt(group.troopsInGroup.Count));

			myOffset = (myIndex % formationWidth - (formationWidth / 2), myIndex / formationWidth - (formationWidth / 2));
		}
		else
			myOffset = (0,0);

		Node destinationNode = pathfinder.GetNodeFromWorldPos(destination, myOffset);
		myWaypoints = pathfinder.FindPath(transform.position, destinationNode.position);

		if (myWaypoints == null)
		{
			print(gameObject.name + " is trying to go to an island right now");
			if (moveCR != null)
				StopCoroutine(moveCR);

			myWaypoints = pathfinder.FindPath(transform.position, destinationNode.position, allowPathThroughReservedNodes: true);
		}


		// TODO: Handling error when there is an island surrounded by structures / unwalkables (and not reserved spaces)


		// HANDLE TROOP ATTACKING
		if (attackCR != null)
			StopCoroutine(attackCR);
		if (action == InputAction.AttackMove)
		{
			attackCR = StartCoroutine(AttackFunction());
		}
		else
		{
			stoppedToAttack = false;
		}

		// HANDLE TROOP MOVEMENT
		if (myWaypoints.Count == 0) // If the path is just to the exact same position troop currently is at, do nothing
		{
			if (myCurrNode != null)
				myCurrNode.ownerTroop = this;

			// TODO: Something when doing attack move here
			return;
		}
		else
			myDestinationNode = myWaypoints[myWaypoints.Count - 1];

		if (myDestinationNode != null)
			myDestinationNode.ownerTroop = this;

		if (myWaypoints.Count > 0)
		{
			if (moveCR != null)
				StopCoroutine(moveCR);
			moveCR = StartCoroutine(MoveAlongWaypoints(destination));
		}

		// velocity = (destination - (Vector2)transform.position).normalized;
	}

	protected abstract IEnumerator AttackFunction();

	private IEnumerator MoveAlongWaypoints(Vector2 clickedPos)
	{
		moving = true;

		yield return null;

		while (myWaypoints.Count > 0)
		{
			yield return new WaitWhile(() => stoppedToAttack);

			Node localDestination = myWaypoints[0];
			myWaypoints.RemoveAt(0);

			// If unwalkable for timeUntilNewPath seconds, then find new path
			float timeUntilNewPath = 0.08f;
			while (!localDestination.walkable || (localDestination.ownerTroop != null && localDestination != myCurrNode))
			{
				yield return new WaitWhile(() => stoppedToAttack); // Not sure about this being here

				timeUntilNewPath -= Time.deltaTime;

				if (timeUntilNewPath <= 0)
				{
					ATroop ownerTroop = localDestination.ownerTroop;
					if (!localDestination.walkable) // Some unwalkable thing is blocking you
					{
						IssueMove(clickedPos, myGroup, myIndex, lastAction);
						ReleaseDestinationNode();
						yield break;
					}
					else if (ownerTroop != null && !ownerTroop.moving && localDestination.ownerTroop != this) // Some not moving troop is blocking you
					{
						if (myGroup.troopsInGroup.Contains(ownerTroop)) // Blocking unit is in your group, tell it to move
						{
							ownerTroop.GiveWaypoints(myWaypoints, clickedPos);
							int myOldIndex = myIndex;
							myIndex = ownerTroop.SwapIndices(myIndex);
							myGroup.troopsInGroup.SwapElements(myOldIndex, myIndex);

							myWaypoints = new List<Node> { localDestination };
							timeUntilNewPath = 7f;
							// print("I am " + gameObject.name + ", I gave my waypoints to " + ownerTroop.name);
							continue;
						}
						else // Blocking unit is not in your group, just stop here
						{
							print("ISLAND: Can't move further (" + gameObject.name + ")");

							if (myCurrNode != null)
								myCurrNode.ownerTroop = this;

							moving = false;
							yield break;
						}
					}					
				}

				yield return null;
			}

			if (myCurrNode != null && myCurrNode.ownerTroop == this)
				myCurrNode.ownerTroop = null;

			myCurrNode = localDestination;

			myCurrNode.ownerTroop = this;

			// destination = waypoints[i].position + Random.insideUnitCircle * 0.25f;

			while ((Vector2)transform.position != localDestination.position)
			{
				transform.position = Vector3.MoveTowards(transform.position, myCurrNode.position, moveSpeed * Time.deltaTime);
				
				yield return null;
			}
		}

		moving = false;
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
