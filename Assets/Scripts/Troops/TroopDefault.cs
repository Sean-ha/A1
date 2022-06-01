using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopDefault : ATroop
{
	private NavMeshAgent agent;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
		agent.updatePosition = false;
	}

	private void Update()
	{
		if (!stationary)
		{
			if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
			{
				// Done
				stationary = true;
				print(gameObject.name + " is stationary");
			}
			else
			{
				// Update position using NavMesh
				Vector3 newPos = agent.nextPosition;
				newPos.z = 0;
				transform.position = newPos;
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (stationary || forceMove)
			return;

		if (collision.gameObject.layer == 7) // Troop layer
		{
			if (collision.gameObject.GetComponent<ATroop>().stationary)
			{
				agent.SetDestination(transform.position);
				stationary = true;
				print(gameObject.name + " is stationary");
			}
		}
	}
}
