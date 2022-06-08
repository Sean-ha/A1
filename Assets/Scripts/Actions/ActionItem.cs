using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ActionItem
{
	public Func<bool> actionCondition;
	public InputAction inputAction;
	public string displayName;
	public Func<bool> actionable;

	// Actionable is a function that when evaluated, will determine if the action can be done or not
	public ActionItem(Func<bool> aC, InputAction i, string n, Func<bool> a)
	{
		actionCondition = aC;
		inputAction = i;
		displayName = n;
		actionable = a;
	}
}
