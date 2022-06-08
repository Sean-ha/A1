using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopGroup
{
	public SortedSet<ATroop> myTroops;
	public OrderedSet<ATroop> troopsInGroup;

	private Vector2 lastClickedPos;

	public TroopGroup(OrderedSet<ATroop> troopsInGroup)
	{
		this.troopsInGroup = troopsInGroup;
	}

	// Call when group has been updated to notify all remaining group members of their new indices
	public void NotifyTroopsOfIndexUpdate()
	{
		for (int i = 0; i < troopsInGroup.Count; i++)
		{
			troopsInGroup[i].SwapIndices(i);
		}
	}

	// Iterate over selected troops thrice: First time to release their nodes, second time to issue movement, then release nodes again
	// The ordering of operations is necessary here.
	public void IssueMoveOrder(InputAction action, Vector2 position)
	{
		lastClickedPos = position;

		for (int i = 0; i < troopsInGroup.Count; i++)
		{
			troopsInGroup[i].ReleaseCurrNode();
		}
		for (int i = 0; i < troopsInGroup.Count; i++)
		{
			ATroop troop = troopsInGroup[i];
			troop.IssueMove(position, this, i, action);
		}
		for (int i = 0; i < troopsInGroup.Count; i++)
		{
			troopsInGroup[i].ReleaseDestinationNode();
		}
	}
}
