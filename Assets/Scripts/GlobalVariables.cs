using UnityEngine;
using System;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables instance;

    [Header("Change these")]
    public float dropSpeed;
    public bool Beam;

    [Header("Don't change these")]
    public GameObject exclamationMark;
    public GameObject questionMark;
    [HideInInspector] public Camera mainCamera;

    [Header("Global Events")]
    public Action onCoffeeCompleted;
    public Action onCaught;
	//GlobalVariables.instance.onCaught += Next;
	//GlobalVariables.instance.onCaught?.Invoke();

    void Awake()
    {
        instance = this;
    }

	void Start()
	{
        mainCamera = Camera.main;
		
        if (mainCamera == null)
        {
            Debug.LogWarning("No main camera found in the scene!");
        }
	}
}
