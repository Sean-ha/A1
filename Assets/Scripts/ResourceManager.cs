using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
	public static ResourceManager instance;

	[SerializeField] ActionsList actionsList;

	[SerializeField] private TextMeshProUGUI shardsText;
	[SerializeField] private TextMeshProUGUI dustText;
	[SerializeField] private TextMeshProUGUI energyText;
	[SerializeField] private TextMeshProUGUI foodText;

	private int currShards;
	private int currDust;

	private int currEnergy;
	private int maxEnergy = 10;

	private int currFood;
	private int maxFood = 10;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		shardsText.text = currShards.ToString();
		dustText.text = currDust.ToString();
		energyText.text = currEnergy + "/" + maxEnergy;
		foodText.text = currFood + "/" + maxFood;

		actionsList.EvaluateActionability();
	}

	public bool HasEnoughResources(int shardsNeeded, int dustNeeded, int energyNeeded, int foodNeeded)
	{
		bool hasMaterials = shardsNeeded <= currShards && dustNeeded <= currDust;
		bool hasSupplies = currEnergy + energyNeeded <= maxEnergy && currFood + foodNeeded <= maxFood;

		return hasMaterials && hasSupplies;
	}

	public void GainShards(int shards)
	{
		currShards += shards;
		UpdateUI();
	}

	public void GainDust(int dust)
	{
		currDust += dust;
		UpdateUI();
	}

	public void GainMaxEnergy(int energyGained)
	{
		maxEnergy += energyGained;
		UpdateUI();
	}

	public void IncreaseUsedEnergy(int amount)
	{
		currEnergy += amount;
		UpdateUI();
	}

	public void GainMaxFood(int foodGained)
	{
		maxFood += foodGained;
		UpdateUI();
	}

	public void IncreaseUsedFood(int amount)
	{
		currFood += amount;
		UpdateUI();
	}

	public void UseResources(int shards, int dust, int energy, int food)
	{
		if (!HasEnoughResources(shards, dust, energy, food))
			return;

		currShards -= shards;
		currDust -= dust;
		currEnergy += energy;
		currFood += food;

		UpdateUI();
	}
}
