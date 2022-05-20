using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{
	[SerializeField] private StructureBuilderManager structureBuilder;
	[SerializeField] private ActionsList actionsList;

	private TextMeshPro currHoveredBuilding;
	private int buildCollideLayer;

	private TextMeshPro currSelectedBuilding;

	private void Awake()
	{
		buildCollideLayer = LayerMask.GetMask("Structure");
	}

	private void Start()
	{
		StartCoroutine(HandleDefaultInput());
	}

	public static Vector2 GetMousePos()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return mousePos;
	}

	public static Collider2D CheckMouseHover(int layerMask)
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0f;

		Collider2D hit = Physics2D.OverlapBox(mousePos, new Vector2(0.01f, 0.01f), 0f, layerMask);

		return hit;
	}

	// CR that handles default inputs (nothing selected)
	private IEnumerator HandleDefaultInput()
	{
		while (true)
		{
			// HANDLE HOVERING OVER STRUCTURES ===

			Collider2D hit = CheckMouseHover(buildCollideLayer);

			// Hovering over a building
			if (hit != null)
			{
				TextMeshPro hitTmp = hit.GetComponent<TextMeshPro>();
				// Hovering over a new building
				if (currHoveredBuilding != null && currHoveredBuilding != hitTmp && currHoveredBuilding != currSelectedBuilding)
				{
					currHoveredBuilding.fontStyle = FontStyles.Normal;
				}

				currHoveredBuilding = hitTmp;
				currHoveredBuilding.fontStyle = FontStyles.Underline;
			}
			// Not hovering over a building
			else
			{
				if (currHoveredBuilding != null)
				{
					if (currHoveredBuilding != currSelectedBuilding)
						currHoveredBuilding.fontStyle = FontStyles.Normal;
					currHoveredBuilding = null;
				}
			}


			// HANDLE CLICKING STRUCTURES ===
			if (Input.GetMouseButtonDown(0))
			{				
				// Select the currently hovered building (and unselect previous one)
				if (currHoveredBuilding != null)
				{
					if (currSelectedBuilding != null)
						currSelectedBuilding.fontStyle = FontStyles.Normal;

					currSelectedBuilding = currHoveredBuilding;
					AStructure structure = currHoveredBuilding.gameObject.GetComponent<AStructure>();

					actionsList.UpdateActionsList(structure.GetActionList(), structure.GetName());
				}
				// Unselect selected building
				else
				{
					if (currSelectedBuilding != null)
						currSelectedBuilding.fontStyle = FontStyles.Normal;

					currSelectedBuilding = null;
					actionsList.SetDefaultBuildActions();
				}
			}


			// HANDLE ACTION INPUT ===
			InputAction action = actionsList.HandleInput();

			if (action != InputAction.None)
			{
				if (currHoveredBuilding != null && currHoveredBuilding != currSelectedBuilding)
					currHoveredBuilding.fontStyle = FontStyles.Normal;

				// Nothing selected, do a build
				if (currSelectedBuilding == null)
				{
					yield return structureBuilder.BeginBuilding(action);
				}
				// Something is selected, tell it to handle the input
				else
				{
					AStructure selectedStructure = currSelectedBuilding.GetComponent<AStructure>();
					yield return selectedStructure.HandleInput(action);

					// Update actionsList after input has been handled in case it is now different
					actionsList.UpdateActionsList(selectedStructure.GetActionList(), selectedStructure.GetName());
				}
			}

			yield return null;
		}
	}
}
