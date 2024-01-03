﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	private Quaternion lastRotation;
	private Vector3 camDirection;
	private Vector3 targetDirection;
	private Rigidbody rb;
	private Vector2 direction;
	private bool isMoving = false;
	private bool canMove = true;
	private float distToGround;
	private float runSpeed = 6f;
	private float animMultiplier;
	private Animator animator;
	CoffeeManager coffeeManager;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		animator = transform.GetChild(0).GetComponent<Animator>();
		coffeeManager = GetComponent<CoffeeManager>();
	}

	private void Start()
	{
		distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
	}

	private void Update()
	{
		if (true) //replace true by "game running boolean" and uncomment else statement
		{
			direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			isMoving = !(direction.x == 0 && direction.y == 0);
		}
		//else
		//{
		//    isMoving = false;
		//    //added 0.00001 to prevent "Look Rotation Viewing Vector Is Zero" error on console because it consumes a lot of FPS
		//    lastRotation = Quaternion.LookRotation(Vector3.zero + new Vector3(0, 0, 0.00001f));
		//}
		ChooseDirection();
		ChooseRotation();
		if (Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightCommand))
		{
			Drink();
		}
		//Keep falling commented until replaced with drinking coffee
		if (IsDrinking())
		{
			coffeeManager.Drink(Time.deltaTime);
		}
		Move();
	}

	//Provides the movement of the player
	private void Move()
	{
		if (IsGrounded())
		{
			Vector3 v = rb.velocity;
			v.x = targetDirection.x * runSpeed;
			v.z = targetDirection.z * runSpeed;
			rb.velocity = v;

			animMultiplier = rb.velocity.magnitude / runSpeed;
			// if (rb.velocity.magnitude < 3) animMultiplier = 0.3f;
			// animator.SetFloat("runMultiplier", animMultiplier);
			if (isMoving)
			{
				animator.SetBool("isRunning", true);
				animator.SetBool("Drinking", false);
			}
			else
			{
				animator.SetBool("isRunning", false);
			};
		}
		else
		{
			rb.velocity = new Vector3(0, rb.velocity.y, 0);
			// animator.SetFloat("runMultiplier", 1);
			animator.SetBool("isRunning", false);
		}
	}
	
	void Drink()
	{
		if (!isMoving && !IsDrinking())
		{
			Debug.Log("Started Drinking...");
			animator.SetBool("Drinking", true);
			animator.SetTrigger("Drink");
			isMoving = false;
		}
	}

	// Decide which direction to go
	private void ChooseDirection()
	{
		//lines below need refining sinceforward camera rotation affects player speed
		// camDirection = Camera.main.transform.rotation * direction; //This takes all 3 axes (good for something flying in 3d space)    
		// targetDirection = new Vector3(camDirection.x, 0, camDirection.z); //This line removes the "space ship" 3D flying effect. We take the cam direction but remove the y axis value

		targetDirection = new Vector3(direction.x, 0, direction.y).normalized; //Irrelevant to the Camera, Uses only input
	}

	// Decide where to look
	private void ChooseRotation()
	{
		if (isMoving && canMove)
		{
			//added 0.00001 to prevent "Look Rotation Viewing Vector Is Zero" error on console because debug messages are bad for performance
			transform.rotation = Quaternion.LookRotation(-targetDirection + new Vector3(0, 0, 0.00001f));
			lastRotation = transform.rotation;
		}
		else
		{
			transform.rotation = lastRotation;
		}
	}

	bool IsDrinking()
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName("Start Drinking") || animator.GetCurrentAnimatorStateInfo(0).IsName("Continue Drinking");
	}

	private bool IsGrounded()
	{
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.0001f);
	}
}