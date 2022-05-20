using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StructureExtractor : AStructure
{
	[SerializeField] private LineRenderer connection;

	[SerializeField] private GameObject extractPiece;


	private BoxCollider2D myCollider;

	private bool connected = false;
	private int buildingLayer;

	private Coroutine extractCR;
	private Vector3 extractPieceOriginalScale;

	private void Awake()
	{
		myCollider = GetComponent<BoxCollider2D>();
		buildingLayer = LayerMask.GetMask("Structure");

		extractPieceOriginalScale = extractPiece.transform.localScale;
	}

	public override List<ActionItem> GetActionList()
	{
		if (connected)
		{
			return new List<ActionItem>
			{
				new ActionItem(KeyCode.Alpha1, InputAction.Connect, "1-connect", () => true),
				new ActionItem(KeyCode.Alpha2, InputAction.Disconnect, "2-disconnect", () => true),
			};
		}
		else
		{
			return new List<ActionItem>
			{
				new ActionItem(KeyCode.Alpha1, InputAction.Connect, "1-connect", () => true),
			};
		}
	}

	public override IEnumerator HandleInput(InputAction action)
	{
		if (action == InputAction.Connect)
		{
			connection.gameObject.SetActive(true);
			bool onCrystal = false;
			Collider2D hover = null;
			while (!Input.GetMouseButtonDown(0))
			{
				Vector2 mousePos = InputHandler.GetMousePos();

				Vector2 startPoint = GetConnectionPointOnCollider(mousePos);

				hover = InputHandler.CheckMouseHover(buildingLayer);

				Vector2 endPoint;
				// Not hovering over anything, endpoint is just mousePos
				if (hover == null)
				{
					endPoint = mousePos;
				}
				// Hovering over something...
				else
				{
					AStructure structure = hover.GetComponent<AStructure>();

					if (structure.GetName() == "crystal") // Hovering over a crystal
					{
						startPoint = GetConnectionPointOnCollider(structure.GetCenter());
						endPoint = structure.GetConnectionPointOnCollider(startPoint);
						onCrystal = true;
					}
					else // Hovering over non-crystal structure
						endPoint = mousePos;

				}

				connection.SetPosition(0, startPoint);
				connection.SetPosition(1, endPoint);

				yield return null;
			}

			// User clicked
			if (onCrystal) // Clicked on crystal, create successful connection
			{
				connected = true;
				extractCR = StartCoroutine(ExtractCrystal(hover));
			}
			else // Clicked on anything non-crystal, connection not successful
			{
				connection.gameObject.SetActive(false);
				connected = false;
			}
		}
		else if (action == InputAction.Disconnect)
		{
			connection.gameObject.SetActive(false);
			connected = false;
		}

		yield return null;
	}

	private IEnumerator ExtractCrystal(Collider2D crystalCollider)
	{
		while (true)
		{
			extractPiece.transform.position = connection.GetPosition(1);
			extractPiece.SetActive(true);

			extractPiece.transform.localScale = new Vector3(0f, 0f, 1f);
			extractPiece.transform.DOScale(extractPieceOriginalScale, 0.15f);

			yield return new WaitForSeconds(0.15f);

			extractPiece.transform.DOMove(connection.GetPosition(0), 0.6f).OnComplete(() =>
			{
				extractPiece.transform.DOScale(new Vector3(0f, 0f, 1f), 0.15f).OnComplete(() =>
				{
					extractPiece.SetActive(false);
					ResourceManager.instance.GainShards(4);
				});
			});

			yield return new WaitForSeconds(1.5f);
		}
	}
}
