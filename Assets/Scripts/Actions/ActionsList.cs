using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionsList : MonoBehaviour
{
	[SerializeField] private GameObject actionTextTemplate;

	private TextMeshProUGUI actionListTitleText;

	// All currently available actions (list of tuples of (input key, display string))
	private List<ActionItem> actionsList = new List<ActionItem>();

	private void Awake()
	{
		actionListTitleText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		SetDefaultBuildActions();
	}

	public void SetDefaultBuildActions()
	{
		actionListTitleText.text = "build";

		actionsList = new List<ActionItem>
		{
			new ActionItem(KeyCode.Alpha1, InputAction.BuildExtractor, "1-extractor", () => ResourceManager.instance.HasEnoughResources(5, 0, 1, 0)),
			new ActionItem(KeyCode.Alpha2, InputAction.BuildGenerator, "2-generator", () => ResourceManager.instance.HasEnoughResources(10, 0, 0, 0)),
			new ActionItem(KeyCode.Alpha3, InputAction.BuildDepot, "3-depot", () => ResourceManager.instance.HasEnoughResources(10, 0, 1, 0))
		};

		UpdateUI();
	}

	public void SetDefaultTroopActions()
	{
		actionListTitleText.text = "troops";

		actionsList = new List<ActionItem>
		{
			new ActionItem(KeyCode.Mouse1, InputAction.Move, "rmb-move", () => true),
			new ActionItem(KeyCode.Alpha2, InputAction.BuildGenerator, "a+rmb-attack", () => true),
		};

		UpdateUI();
	}

	private void UpdateUI()
	{
		// Destroy all previous actions from UI list
		for (int i = 1; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		// Add all new actions to UI list
		foreach (ActionItem item in actionsList)
		{
			GameObject actionTxt = Instantiate(actionTextTemplate, transform, false);
			TextMeshProUGUI actionTextTmp = actionTxt.GetComponent<TextMeshProUGUI>();
			actionTextTmp.text = item.displayName;

			if (!item.actionable.Invoke())
			{
				actionTextTmp.color = new Color(0.8f, 0.25f, 0.25f);
			}
		}
	}

	// Called by InputHandler. Uses our list state to handle inputs for current actions. Returns 
	public InputAction HandleInput()
	{
		foreach (ActionItem item in actionsList)
		{
			if (Input.GetKeyDown(item.keyCode))
			{
				return item.inputAction;
			}
		}

		return InputAction.None;
	}

	public void UpdateActionsList(List<ActionItem> newActions, string title)
	{
		actionListTitleText.text = title;
		actionsList = newActions;
		UpdateUI();
	}

	public void EvaluateActionability()
	{
		for (int i = 0; i < actionsList.Count; i++)
		{
			if (actionsList[i].actionable.Invoke()) // Is actionable
			{
				transform.GetChild(i + 1).GetComponent<TextMeshProUGUI>().color = Color.white;
			}
			else // Is not actionable
			{
				transform.GetChild(i + 1).GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0.25f, 0.25f);
			}
		}
	}
}
