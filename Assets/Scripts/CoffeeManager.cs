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
	void Awake()
	{
		ShowCup(false);
		BuyCoffee();
		Drink(0);
	}

	private void Update()
	{
		if (coffeeHeat > 0)
		{
			coffeeHeat -= Time.deltaTime;
			UIManager.instance.HeatSlider.value = coffeeHeat / startCoffeeHeat;
		}
	}

	public void Drink(float drinkAmount)
	{
		coffeeAmount -= drinkAmount;
		UIManager.instance.AmountSlider.value = coffeeAmount / startCoffeeAmount;
	}

	private void BuyCoffee()
	{
		ShowCup(true);
		coffeeAmount = startCoffeeAmount;
		coffeeHeat = startCoffeeHeat;
	}

	void ShowCup(bool enable)
	{
		coffeeCup.gameObject.SetActive(enable);
	}
}
