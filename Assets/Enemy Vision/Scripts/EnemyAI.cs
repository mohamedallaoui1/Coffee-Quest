using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        None = 0,
        Patrol = 1,
        Alert = 2,
        Chase = 3,
        Confused = 4, //AI scrubs his head in confusion before returning to patrol
        AlertedPatrol = 5,
    }
	
	[SerializeField] float walkSpeed = 2f;
	[SerializeField] float runSpeed = 3.5f;
    [SerializeField] float stoppingDistance = 1f;

	
	[Header("State")]
	public EnemyState state = EnemyState.Patrol;
	private float stateTimer = 0f;
	[SerializeField] float alertWaitTime = 0.5f;
	[SerializeField] float confusedWaitTime = 0.5f;
	[SerializeField] float noSightChaseTime = 4; //time to keep chasing accurately after losing sight
	[SerializeField] float forgetTime = 4f; //or 5? //time to keep following player after losing sight

	float lostSightTimer = 0f;
	float noSightChaseTimer;
    Vector3 destination;
    Vector3 startPos;
	Vector3 lastSeenPos;
    Quaternion lastRotation;
	bool posAroundPlayerOrSelf = true;

    float smoothRotationTime = 5f;

    [SerializeField] FieldOfView fieldOfView;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    private PlayerController target;

    private void Awake()
    {
		target = FindObjectOfType<PlayerController>();
        startPos = transform.position;
		destination = startPos;
		lastSeenPos = startPos;
		Debug.Log("posAroundPlayerOrSelf: " + posAroundPlayerOrSelf);
		//StartPatrol();
    }

    private void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetDirection(transform.forward);
		
		stateTimer += Time.deltaTime;

		if (state == EnemyState.Patrol)
		{
			UpdatePatrol();
		}
		else if (state == EnemyState.Alert)
		{
			UpdateAlert();
		}
		else if (state == EnemyState.Chase)
		{
			UpdateChase();
		}
		else if (state == EnemyState.Confused)
		{
			UpdateConfused();
		}
		else if (state == EnemyState.AlertedPatrol)
		{
			UpdateAlertedPatrol();
		}

		if(fieldOfView.IsTarget && target.IsDrinking())
		{
			GameManager.instance.GameOver();
		}

		animator.SetBool("Run", state == EnemyState.Chase);
		animator.SetBool("Walk", state == EnemyState.Patrol);
		Rotation();
    }

	void Rotation()
    {
		if (agent.remainingDistance <= .1f)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, lastRotation, Time.deltaTime * smoothRotationTime);
		}
		else
		{
			var turnTowardNavSteeringTarget = agent.steeringTarget;
			Vector3 direction;
			if(state == EnemyState.Alert)
			{
				direction = (target.transform.position - transform.position).normalized;
			}
			else if(state == EnemyState.Confused)
			{
				direction = (lastSeenPos - transform.position).normalized;
			}
			else
			{
				direction = (turnTowardNavSteeringTarget - transform.position).normalized;
			}
			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothRotationTime);
		}
		lastRotation = transform.rotation;
    }

	void ChangeState(EnemyState state)
	{
		if(state == EnemyState.Confused)
		{
			animator.SetTrigger("Confused");
		}else if(state == EnemyState.Alert)
		{
			animator.SetTrigger("Surprised");
		}
		this.state = state;
		stateTimer = 0f;
		ChangeFOVMat();
		Debug.Log("Changed state to: " + state.ToString());
	}

	EnemyAI.EnemyState lastState;
	public void ChangeFOVMat()
	{
		if (lastState == state)
		{
			return;
		}
		if (state == EnemyState.Patrol)
		{
			fieldOfView.ChangeFOVMat(fieldOfView.greenFOVMat);
		}
		else if (state == EnemyState.Alert)
		{
			fieldOfView.ChangeFOVMat(fieldOfView.redFOVMat);
		}
		else if (state == EnemyState.Chase)
		{
			fieldOfView.ChangeFOVMat(fieldOfView.redFOVMat);
		}
		else if (state == EnemyState.Confused)
		{
			fieldOfView.ChangeFOVMat(fieldOfView.greenFOVMat);
		}
		lastState = state;
	}

	float patrolDestinationTimer = 0f;
	float patrolDestinationWaitTime = 5f;
	void UpdatePatrol()
	{
		patrolDestinationTimer += Time.deltaTime;
		if (fieldOfView.IsTarget)
		{
	        agent.isStopped	= true;
			//Show exclamation mark above head and perform surprised animation
			Instantiate(GlobalVariables.instance.exclamationMark, transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);		
			lastSeenPos = target.transform.position;
			ChangeState(EnemyState.Alert);
			return;
		}
		if(agent.remainingDistance <= stoppingDistance || (!posAroundPlayerOrSelf && patrolDestinationTimer >= patrolDestinationWaitTime))
		{
			destination = RandomPos();
			PreviewDestination(destination);
			patrolDestinationTimer = 0;
		}

		//one thing to consider is the processing cost of updating the destination of the agents too frequently.
		//It takes some effort to calculate a new destination and path,
		//and if there's other things happening in the game or on your PC at the same time the path calculation chokes a bit.
		if(destination != agent.destination) //comment this if a bug occurs
	        agent.SetDestination(destination);

		//perform patrol logic
	}

	void UpdateAlert()
	{
		if (stateTimer > alertWaitTime)
		{
			animator.SetTrigger("Reset");
			StartChase();
		}
	}

	GameObject sphere;
	void UpdateChase()
	{
		//follow player accurately for a period after starting chase, after noSightChaseTimer only follow lastseenpos
		//destination = target.transform.position;
		if (fieldOfView.IsTarget)
		{
			lostSightTimer = 0;
			noSightChaseTimer = 0;
			lastSeenPos = target.transform.position;
			destination = target.transform.position;
			PreviewDestination(destination);
		}
		else
		{
			noSightChaseTimer += Time.deltaTime;
			if (noSightChaseTimer < noSightChaseTime)
			{
				destination = target.transform.position;
				PreviewDestination(destination);
			}
			else
			{
				//if (!sphere)
				//{
				//	sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				//	sphere.transform.position = destination;
				//	sphere.transform.localScale = Vector3.one * 0.3f;
				//	sphere.GetComponent<SphereCollider>().enabled = false;
				//}
				//else
				//{
				//	DOTween.Kill(sphere.transform);
				//	sphere.transform.DOMove(destination, 0.5f);
				//}

				lostSightTimer += Time.deltaTime;
				if (lostSightTimer > forgetTime || agent.remainingDistance <= stoppingDistance)
				{
					noSightChaseTimer = 0;
					lostSightTimer = 0;
					destination = transform.position;
					PreviewDestination(destination);
					agent.stoppingDistance = 0;
					agent.isStopped	= true;
					//Show question mark above head and perform confused animation (scratch head)
					Instantiate(GlobalVariables.instance.questionMark, transform.position + Vector3.up * 2f, Quaternion.identity, transform);
					ChangeState(EnemyState.Confused);
				}
			}
		}
		if(destination != agent.destination) //comment this if a bug occurs
	        agent.SetDestination(destination);
	}

	void UpdateConfused()
	{
		if (stateTimer > confusedWaitTime)
		{
			animator.SetTrigger("Reset");
			StartPatrol();
			return;
		}
		if (fieldOfView.IsTarget)
		{
			//Show exclamation mark above head, probably also a surprised animation
			Instantiate(GlobalVariables.instance.exclamationMark, transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);
			StartChase();
		}
	}

	void UpdateAlertedPatrol()
	{
		//perform alerted patrol logic
	}

	Vector3 RandomPos()
	{
		int radius = 6;
		Vector3 randomDirection = Random.insideUnitSphere * radius;

		posAroundPlayerOrSelf = !posAroundPlayerOrSelf;
		if(posAroundPlayerOrSelf)
			randomDirection += target.transform.position;
		else
			randomDirection += (transform.position - transform.up);

		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
		
		return hit.position;
	}

	GameObject destCube;
	void PreviewDestination(Vector3 destination)
	{
		//Debug.Log("destination: " + destination);
		//if(!destCube)
		//{
		//	destCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//	destCube.transform.position = destination;
		//	destCube.transform.localScale = Vector3.one * 0.3f;
		//	destCube.GetComponent<BoxCollider>().enabled = false;
		//}else
		//{
		//	DOTween.Kill(destCube.transform);
		//	destCube.transform.DOMove(destination, 0.5f);
		//}
	}

	void StartChase()
	{
		destination = lastSeenPos;
		PreviewDestination(destination);
		agent.stoppingDistance = stoppingDistance;
		agent.speed = runSpeed;
		agent.isStopped	= false;
		ChangeState(EnemyState.Chase);
	}

	void StartPatrol()
	{
		destination = RandomPos();
		PreviewDestination(destination);
		agent.stoppingDistance = 0;
		agent.speed = walkSpeed;
		agent.isStopped	= false;
		ChangeState(EnemyState.Patrol);
	}
}
