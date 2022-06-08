using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class InputHandler : MonoBehaviour
{
	public GameObject target;

	[SerializeField] private StructureBuilderManager structureBuilder;
	[SerializeField] private ActionsList actionsList;

	[SerializeField] private CircleRenderer rangeCircleObject;
	[SerializeField] private RectangleRenderer troopSelectRectangle;

	[SerializeField] private TroopSelector troopSelector;

	private TextMeshPro currHoveredBuilding;
	private int buildCollideLayer;

	private TextMeshPro currSelectedBuilding;

	private int troopLayerMask;
	private List<ATroop> currDraggedTroops;
	private OrderedSet<ATroop> currSelectedTroops = new OrderedSet<ATroop>();
	private TroopGroup currSelectedTroopGroup;

	private bool clickDown;
	private Vector2 clickDownPos;

	private void Awake()
	{
		buildCollideLayer = LayerMask.GetMask("Structure");
		troopLayerMask = LayerMask.GetMask("Troop");
	}

	private void Start()
	{
		StartCoroutine(HandleDefaultInput());

		currDraggedTroops = troopSelector.GetSelectedList();
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

		Collider2D hit = Physics2D.OverlapBox(mousePos, new Vector2(0.001f, 0.001f), 0f, layerMask);

		return hit;
	}

	// CR that handles default inputs (nothing selected)
	private IEnumerator HandleDefaultInput()
	{
		while (true)
		{
			Vector2 mousePos = GetMousePos();

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


			// HANDLE CLICK BEHAVIOR ===
			if (Input.GetMouseButtonDown(0))
			{
				clickDown = true;
				clickDownPos = mousePos;

				DeselectTroops();
				troopSelector.ClearList();
			}

			if (clickDown)
			{
				troopSelectRectangle.gameObject.SetActive(true);
				troopSelectRectangle.DrawRectangle(clickDownPos, mousePos, 0.15f, true);

				troopSelector.UpdateArea(clickDownPos, mousePos);
			}

			if (Input.GetMouseButtonUp(0))
			{
				clickDown = false;
				troopSelectRectangle.gameObject.SetActive(false);
				troopSelector.Deactivate();

				Vector2 clickUpPos = mousePos;
				// If dragged a sufficient distance, or if any troops were selected, then do troop selection
				// HANDLE SELECTING TROOPS ===
				if (Vector2.Distance(clickDownPos, clickUpPos) > 0.15f || currDraggedTroops.Count > 0)
				{
					DeselectBuilding();
					// Lock the currently selected troops
					currSelectedTroops = new OrderedSet<ATroop>();
					foreach (ATroop troop in currDraggedTroops)
						currSelectedTroops.Add(troop);
					currSelectedTroopGroup = new TroopGroup(currSelectedTroops);

					if (currDraggedTroops.Count == 0)
						actionsList.SetDefaultBuildActions();
					else
						actionsList.SetDefaultTroopActions();
				}

				// If not dragged a sufficient distance and no troops were selected, then do a single selection
				else
				{
					// Not hovering over building, so select a troop if possible
					if (currHoveredBuilding == null)
					{
						Collider2D hoverTroop = CheckMouseHover(troopLayerMask);

						if (hoverTroop != null)
						{
							DeselectBuilding();
							ATroop troop = hoverTroop.GetComponent<ATroop>();
							currSelectedTroops = new OrderedSet<ATroop> { troop };
							currSelectedTroopGroup = new TroopGroup(currSelectedTroops);
							troop.OutlineTroop();
							actionsList.SetDefaultTroopActions();
						}
						else // Hovering over neither building nor troop
						{
							DeselectAll();
						}
					}
					// HANDLE CLICKING STRUCTURES ===
					// Select the currently hovered building (and unselect previous one)
					else
					{
						if (currSelectedBuilding != null)
							currSelectedBuilding.fontStyle = FontStyles.Normal;

						currSelectedBuilding = currHoveredBuilding;
						AStructure structure = currHoveredBuilding.gameObject.GetComponent<AStructure>();

						actionsList.UpdateActionsList(structure.GetActionList(), structure.GetName());

						// Show range
						rangeCircleObject.transform.position = currSelectedBuilding.transform.position;
						rangeCircleObject.gameObject.SetActive(true);
						rangeCircleObject.DrawCircle(structure.GetRangeRadius(), .5f, true);
						rangeCircleObject.SetColor(currSelectedBuilding.color);
					}
				}
			}

			// Deselect currently selected building or troops
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				DeselectAll();
			}


			// HANDLE ACTION INPUT ===
			// Don't handle input if player is currently dragging
			if (!clickDown)
			{
				InputAction action = actionsList.HandleInput();

				if (action != InputAction.None)
				{
					if (currHoveredBuilding != null && currHoveredBuilding != currSelectedBuilding)
						currHoveredBuilding.fontStyle = FontStyles.Normal;

					// Nothing selected, do a build
					if (currSelectedBuilding == null && currSelectedTroopGroup == null)
					{
						yield return structureBuilder.BeginBuilding(action);
					}
					// Building is selected, tell it to handle the input
					else if (currSelectedBuilding != null)
					{
						AStructure selectedStructure = currSelectedBuilding.GetComponent<AStructure>();
						yield return selectedStructure.HandleInput(action);

						// Update actionsList after input has been handled in case it is now different
						actionsList.UpdateActionsList(selectedStructure.GetActionList(), selectedStructure.GetName());
					}
					// Troops are selected, tell them to handle input
					else if (currSelectedTroopGroup != null)
					{
						target.transform.DOKill();
						target.transform.position = mousePos;

						target.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

						target.transform.DOScale(0f, 0.4f);

						currSelectedTroopGroup.IssueMoveOrder(action, mousePos);
					}
				}
			}

			yield return null;
		}
	}

	private void DeselectBuilding()
	{
		if (currSelectedBuilding != null)
			currSelectedBuilding.fontStyle = FontStyles.Normal;

		currSelectedBuilding = null;
		rangeCircleObject.gameObject.SetActive(false);
	}

	private void DeselectTroops()
	{
		// Deselect troops
		if (currSelectedTroops.Count > 0)
		{
			foreach (ATroop troop in currSelectedTroops)
			{
				troop.UnOutlineTroop();
			}
			currSelectedTroops = new OrderedSet<ATroop>();
			currSelectedTroopGroup = null;
		}
	}

	private void DeselectAll()
	{
		DeselectBuilding();

		DeselectTroops();

		actionsList.SetDefaultBuildActions();
	}
}
