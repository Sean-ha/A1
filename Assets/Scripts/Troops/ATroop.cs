using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class ATroop : MonoBehaviour
{
	[SerializeField] private Sprite defaultSprite;
	[SerializeField] private Sprite outlineSprite;

	public bool stationary { get; set; }

	protected bool forceMove;

	public void OutlineTroop()
	{
		GetComponent<SpriteRenderer>().sprite = outlineSprite;
	}

	public void UnOutlineTroop()
	{
		GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}

	public virtual void HandleInput(InputAction action)
	{
		print("Handling input for " + gameObject.name);
		if (action == InputAction.Move)
		{
			stationary = false;
			StartCoroutine(ForceMove());
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.nextPosition = transform.position;
			agent.SetDestination(InputHandler.GetMousePos());
		}
	}

	private IEnumerator ForceMove()
	{
		forceMove = true;
		for (int i = 0; i < 12; i++)
		{
			yield return null;
		}
		forceMove = false;
	}
}
