using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StructureBuilderManager : MonoBehaviour
{
	[SerializeField] private GameObject extractorBuilding;
	[SerializeField] private GameObject generatorBuilding;
	[SerializeField] private GameObject depotBuilding;


	private TextMeshPro myTmp;
	
	private bool building;
	private Vector2 currColliderSize;
	private Color currColor;
	private GameObject currBuildGameObject;
	private int buildCollideLayer;


	private void Awake()
	{
		myTmp = GetComponent<TextMeshPro>();
		buildCollideLayer = LayerMask.GetMask("Structure");
	}

	public IEnumerator BeginBuilding(InputAction buildAction)
	{
		switch (buildAction)
		{
			case InputAction.BuildExtractor:
				currBuildGameObject = extractorBuilding;
				break;
			case InputAction.BuildGenerator:
				currBuildGameObject = generatorBuilding;
				break;
			case InputAction.BuildDepot:
				currBuildGameObject = depotBuilding;
				break;

			default:
				Debug.LogError("Invalid action provided... shouldn't happen ever in theory... (" + buildAction.ToString() + ")");
				break;
		}

		TextMeshPro buildingTmp = currBuildGameObject.GetComponent<TextMeshPro>();

		// Copy the important values so the structure looks the exact same
		myTmp.text = buildingTmp.text;
		currColor = buildingTmp.color;
		currColor.a = 0.7f;
		myTmp.color = currColor;
		myTmp.fontSize = buildingTmp.fontSize;
		myTmp.characterSpacing = buildingTmp.characterSpacing;
		myTmp.wordSpacing = buildingTmp.wordSpacing;

		BoxCollider2D buildingCollider = currBuildGameObject.GetComponent<BoxCollider2D>();
		currColliderSize = buildingCollider.size;

		building = true;

		while (building)
		{
			Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newPos.z = 0f;
			transform.position = newPos;

			Collider2D hit = Physics2D.OverlapBox(transform.position, currColliderSize, 0f, buildCollideLayer);

			// Not overlapping anything, is buildable
			if (hit == null)
			{
				myTmp.color = currColor;

				if (Input.GetMouseButtonDown(0))
				{
					// Build structure here
					print("Build structure");

					Instantiate(currBuildGameObject, transform.position, Quaternion.identity);
					CancelBuild();
				}
				else if (Input.GetMouseButtonDown(1))
				{
					// Cancel build here
					print("Cancel build");
					CancelBuild();
				}
			}
			// Is overlapping another structure, is not buildable
			else
			{
				myTmp.color = currColor * new Color(0.8f, 0.3f, 0.3f, 1f);
			}

			yield return null;
		}
	}

	private void CancelBuild()
	{
		myTmp.text = "";
		building = false;
	}
}
