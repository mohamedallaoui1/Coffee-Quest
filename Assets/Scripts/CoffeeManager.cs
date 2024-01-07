using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoffeeManager : MonoBehaviour
{
	public Transform coffeeCup;
	public float startCoffeeAmount; //amount of seconds of drinking to empty cup
	public float startCoffeeHeat; //amount of seconds until coffee gets cold
	float coffeeAmount;
	float coffeeHeat; //min:0 max:100

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Start()
	{
		ShowCup(false);
		BuyCoffee();
		ConsumeDrink(0f);
	}

	private void Update()
	{
		if (coffeeHeat > 0)
		{
			coffeeHeat -= Time.deltaTime;
			UIManager.instance.heatSlider.value = coffeeHeat / startCoffeeHeat;
		}
	}

	void ShowCup(bool enable)
	{
		coffeeCup.gameObject.SetActive(enable);
	}

	void BuyCoffee()
	{
		ShowCup(true);
		coffeeAmount = startCoffeeAmount;
		coffeeHeat = startCoffeeHeat;
	}

	public void ConsumeDrink(float consumeAmount)
	{
		if (coffeeAmount > 0)
		{
			coffeeAmount -= consumeAmount;
			UIManager.instance.amountSlider.value = coffeeAmount / startCoffeeAmount;
		}
	}
	public bool DrinkAvailable()
	{
		return coffeeAmount > 0;
	}

	void DrinkFinished()
	{
		Debug.Log("Finished drinking coffee without getting caught! Won!");
	}
}
