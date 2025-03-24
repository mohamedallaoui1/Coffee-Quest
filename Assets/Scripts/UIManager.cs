using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;
	public Slider heatSlider;
	public Slider amountSlider;
	public Button restartButton;

	private void Awake()
	{
		instance = this;
		restartButton.onClick.AddListener(() =>
		{
			GameManager.instance.RestartGame();
		});
		restartButton.gameObject.SetActive(false);
	}
}
