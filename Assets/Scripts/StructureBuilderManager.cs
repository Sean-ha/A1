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
	private CircleRenderer circleRenderer;
	
	private bool building;
	private Vector2 currColliderSize;
	private Color currColor;
	private GameObject currBuildGameObject;
	private int buildCollideLayer;


	private void Awake()
	{
		myTmp = GetComponent<TextMeshPro>();
		buildCollideLayer = LayerMask.GetMask("Structure");
		circleRenderer = transform.GetChild(0).GetComponent<CircleRenderer>();
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

		AStructure currStructure = currBuildGameObject.GetComponent<AStructure>();
		// Occurs when player doesn't have enough resources
		if (!ResourceManager.instance.HasEnoughResources(currStructure.shardCost, currStructure.dustCost, currStructure.energyCost, currStructure.foodCost))
		{
			yield break;
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

		// Set radius
		circleRenderer.gameObject.SetActive(true);
		circleRenderer.DrawCircle(currStructure.GetRangeRadius(), 0.5f, true);

		building = true;

		while (building)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				CancelBuild();

			Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newPos.z = 0f;
			transform.position = newPos;

			Collider2D hit = Physics2D.OverlapBox(transform.position, currColliderSize, 0f, buildCollideLayer);

			// Not overlapping anything, is buildable
			if (hit == null)
			{
				myTmp.color = currColor;
				circleRenderer.SetColor(currColor);

				if (Input.GetMouseButtonDown(0))
				{
					// Build structure here
					Instantiate(currBuildGameObject, transform.position, Quaternion.identity);

					currStructure = currBuildGameObject.GetComponent<AStructure>();
					ResourceManager.instance.UseResources(currStructure.shardCost, currStructure.dustCost, currStructure.energyCost, currStructure.foodCost);

					CancelBuild();
				}
				else if (Input.GetMouseButtonDown(1))
				{
					// Cancel build here
					CancelBuild();
				}
			}
			// Is overlapping another structure, is not buildable
			else
			{
				myTmp.color = currColor * new Color(0.8f, 0.3f, 0.3f, 1f);
				circleRenderer.SetColor(currColor * new Color(0.8f, 0.3f, 0.3f, 1f));
			}

			yield return null;
		}
	}

	private void CancelBuild()
	{
		myTmp.text = "";
		building = false;
		circleRenderer.gameObject.SetActive(false);
	}
}
