using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopShooter : ATroop
{
	[SerializeField] private GameObject myProjectile;
	[SerializeField] private RangeChecker myRangeChecker;
	[SerializeField] private float attackCooldown;
	[SerializeField] private float attackDamage;

	private float currAttackCooldown;

	private void Update()
	{
		if (currAttackCooldown > 0)
			currAttackCooldown -= Time.deltaTime;
	}

	protected override IEnumerator AttackFunction() // Constantly running during attack move
	{
		while (true)
		{
			yield return new WaitUntil(() => myRangeChecker.enemiesInRange.Count > 0);

			stoppedToAttack = true;

			while (myRangeChecker.enemiesInRange.Count > 0)
			{
				if (currAttackCooldown <= 0)
				{
					print("Do attack");
					currAttackCooldown = attackCooldown;
				}

				yield return null;
			}

			stoppedToAttack = false;

			yield return null;
		}
	}
}
