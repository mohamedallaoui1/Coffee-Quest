using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;
	public Slider heatSlider;
	public Slider amountSlider;
	private void Awake()
	{
		instance = this;
	}
}
