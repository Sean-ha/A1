using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopSelector : MonoBehaviour
{
	private BoxCollider2D myCollider;

	private List<ATroop> selectedList = new List<ATroop>();

	private bool active;

	private void Awake()
	{
		myCollider = GetComponent<BoxCollider2D>();

		LayerMask troopLayerMask = LayerMask.GetMask("Troop");
	}

	public List<ATroop> GetSelectedList()
	{
		return selectedList;
	}

	public void ClearList()
	{
		selectedList.Clear();
	}

	// Checks the given area for troops and highlights them. The area is the rectangle between the given points
	// Returns a list of selected troops. Note this list will not always be entirely up to date since we need
	// 1 frame for collision callbacks to be called
	public void UpdateArea(Vector2 startPos, Vector2 endPos)
	{
		gameObject.SetActive(true);
		active = true;

		Vector2 center = Vector2.Lerp(startPos, endPos, 0.5f);
		Vector2 size = new Vector2(Mathf.Abs(endPos.x - startPos.x), Mathf.Abs(endPos.y - startPos.y));

		myCollider.transform.position = center;
		myCollider.size = size;

		// return selectedList;

		// List<Collider2D> newCurrTroops = new List<Collider2D>();
		// int hitCount = Physics2D.OverlapBox(center, size, 0f, troopFilter, newCurrTroops);
	}

	public void Deactivate()
	{
		active = false;
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		ATroop selected = collision.GetComponent<ATroop>();
		selectedList.Add(selected);
		selected.OutlineTroop();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (active)
		{
			ATroop unselected = collision.GetComponent<ATroop>();
			selectedList.Remove(unselected);
			unselected.UnOutlineTroop();
		}
	}
}
