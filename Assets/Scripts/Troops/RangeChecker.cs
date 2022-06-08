using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecker : MonoBehaviour
{
	public HashSet<Collider2D> enemiesInRange { get; private set; } = new HashSet<Collider2D>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		enemiesInRange.Add(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		enemiesInRange.Remove(collision);
	}
}
