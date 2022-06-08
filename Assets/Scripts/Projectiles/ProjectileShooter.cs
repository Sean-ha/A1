using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProjectileShooter : MonoBehaviour
{
	public float time;
	public float mySpeed;

	private float speed;

	public void SetProjectile(float dangle, float damage, float speed)
	{
		this.speed = 0;
		
		transform.localScale = new Vector3(0f, 2f, 1f);

		transform.DOScale(new Vector3(2.5f, 0.3f, 1f), time).SetEase(Ease.Linear).OnComplete(() =>
		{
			this.speed = speed;
			transform.DOScale(new Vector3(10f, 0.25f, 1f), 0.1f);
		});
	}

	private void FixedUpdate()
	{
		transform.position = new Vector2(transform.position.x + (speed * Time.fixedDeltaTime), transform.position.y);
	}

	private void Start()
	{
		StartCoroutine(Test());
	}

	private IEnumerator Test()
	{
		while (true)
		{
			transform.position = new Vector3(-4.5f, -3.5f, 0f);
			SetProjectile(0f, 0f, mySpeed);
			yield return new WaitForSeconds(1f);
		}
	}
}
