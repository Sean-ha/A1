using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	[SerializeField] private StructureBuilder structureBuilder;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			structureBuilder.BeginBuilding();
		}
	}
}
