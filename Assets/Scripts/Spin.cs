using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
	[SerializeField] private float spinsPerSecond;

	private void Update()
	{
		transform.localRotation = Quaternion.Euler(0f, 0f, transform.localRotation.eulerAngles.z + spinsPerSecond * 360 * Time.deltaTime);
	}
}
