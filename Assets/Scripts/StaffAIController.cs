using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class StaffAIController : MonoBehaviour
{
	NavMeshAgent navMeshAgent;
	Animator animator;
	PlayerController target;
	public float catchDistance = 1f;
	Vector3 lastPos;

	private void Awake()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		animator = transform.GetChild(0).GetComponent<Animator>();
	}

	private void Start()
	{
		target = GameObject.FindFirstObjectByType<PlayerController>();
		if (target == null)
		{
			Debug.LogError("Didn't find target");
			return;
		}
		Chase(true);
	}

	private void Update()
	{
		if (target.caught)
		{
			StopChasing();
		}
		else
		{
			if (chasing)
			{
				bool reachedTarget = Vector3.Distance(transform.position, target.transform.position) <= catchDistance;
				if (reachedTarget)
				{
					//Catch();
				}
				else
				{
					navMeshAgent.SetDestination(target.transform.position);
				}
				Rotate();
				animator.SetBool("isRunning", !reachedTarget);
			}
		}
		lastPos = transform.position;
	}

	void Rotate()
	{
		if (navMeshAgent.velocity.magnitude > 0.1f)
		{
			Quaternion lookRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
		}
	}

	private void StopChasing()
	{
		chasing = false;
		if (navMeshAgent.hasPath)
		{
			navMeshAgent.ResetPath();
		}
	}

	void Wander()
	{

	}

	bool chasing;
	void Chase(bool chase)
	{
		chasing = chase;
	}

	void Catch()
	{
		target.caught = true;
	}
}
