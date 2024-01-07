using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public bool gameStarted;
	public bool gameEnded;
	private void Awake()
	{
		instance = this;
	}

	void Start()
	{
		gameStarted=true;
	}
}
