using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopDefault : ATroop
{
	private Collider2D myCol;
	

	private void Awake()
	{
		myCol = GetComponent<Collider2D>();
	}	

	private void Update()
	{

		/*if (flocking)
		{
			// Check neighbors
			ContactFilter2D filter = new ContactFilter2D();
			filter.SetLayerMask(LayerMask.GetMask("Troop"));

			List<Collider2D> collisions = new List<Collider2D>();
			List<ATroop> neighbors = new List<ATroop>();

			Physics2D.OverlapCircle(transform.position, 3f, filter, collisions);

			foreach (Collider2D col in collisions)
			{
				neighbors.Add(col.GetComponent<ATroop>());
			}

			Vector2 alignment = CalculateAlignment(neighbors);
			Vector2 cohesion = CalculateCohesion(neighbors);
			Vector2 separation = CalculateSeparation(neighbors);

			if (!temp)
			{
				velocity = agent.velocity;
			}
			else
			{
				temp = false;
			}

			velocity += alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight;
			velocity = velocity.normalized * agent.speed;

			agent.velocity = velocity;
		}*/


		/*if (!stationary)
		{
			if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
			{
				// Done
				stationary = true;
				print(gameObject.name + " is stationary");
			}
			else
			{
				
			}
		}*/
	}
}
