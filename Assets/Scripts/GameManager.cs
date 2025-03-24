using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		gameStarted = true;
	}

	public void GameOver()
	{
		gameEnded = true;
		Debug.Log("Game Over");
		UIManager.instance.restartButton.gameObject.SetActive(true);
	}
	
	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
