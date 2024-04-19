using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // Public variable to hold the target
    public Transform patrolPoint; // Public variable to hold the patrol point
    public enum EnemyState { Idle, Patrol, Chase, Attack }; // Enum to represent different states
    public EnemyState enemyState; // Variable to hold the current state

    private NavMeshAgent ai; // Private variable to hold the NavMeshAgent component
    private Animator anim; // Private variable to hold the Animator component
    private float distanceToTarget; // Private variable to hold the distance to the target
    private Coroutine idleToPatrol; // Private variable to hold the coroutine reference

    void Start()
    {
        // Initialize enemyState to Idle
        enemyState = EnemyState.Idle;

        // Get the NavMeshAgent component attached to the same GameObject
        ai = GetComponent<NavMeshAgent>();

        // Get the Animator component attached to the same GameObject
        anim = GetComponent<Animator>();

        // Calculate the initial distance to the target
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));
    }

    void Update()
    {
        // Update the distance to the target
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));

        // Switch statement to handle different enemy states
        switch (enemyState)
        {
            case EnemyState.Idle:
                // Call SwitchState to update animation state
                SwitchState(0);
                // Set destination to target's position
                ai.SetDestination(transform.position);
                // Start patrolling if idleToPatrol is null
                if (idleToPatrol == null)
                    idleToPatrol = StartCoroutine(SwitchToPatrol());
                break;

            case EnemyState.Patrol:
                // Calculate distance to patrol point
                float distanceToPatrolPoint = Mathf.Abs(Vector3.Distance(patrolPoint.position, transform.position));
                // Check if distance to patrol point is greater than 2
                if (distanceToPatrolPoint > 2)
                {
                    // Call SwitchState to update animation state
                    SwitchState(1);
                    // Set destination to patrol point's position
                    ai.SetDestination(patrolPoint.position);
                }
                else
                {
                    // Call SwitchState to update animation state
                    SwitchState(0);
                }
                // Transition to Chase state if target is within range
                if (distanceToTarget <= 15)
                    enemyState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                // Call SwitchState to update animation state
                SwitchState(2);
                // Move towards the target
                ai.SetDestination(target.position);
                // Transition to Attack state if target is within attack range
                if (distanceToTarget <= 5)
                    enemyState = EnemyState.Attack;
                // Transition to Idle state if target is out of chase range
                else if (distanceToTarget > 15)
                    enemyState = EnemyState.Idle;
                break;

            case EnemyState.Attack:
                // Call SwitchState to update animation state
                SwitchState(3);
                // If target is out of attack range but within chase range, chase
                if (distanceToTarget > 5 && distanceToTarget <= 15)
                    enemyState = EnemyState.Chase;
                // Transition to Idle state if target is out of chase range
                else if (distanceToTarget > 15)
                    enemyState = EnemyState.Idle;
                break;

            default:
                Debug.LogError("Unhandled enemy state!");
                break;
        }
    }

    IEnumerator SwitchToPatrol()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5);
        // Change state to Patrol
        enemyState = EnemyState.Patrol;
        // Reset idleToPatrol coroutine reference
        idleToPatrol = null;
    }

    void SwitchState(int newState)
    {
        // Check if the animation parameter State does not equal newState
        if (anim.GetInteger("State") != newState)
        {
            // Set the animation parameter State to newState
            anim.SetInteger("State", newState);
        }
    }
}


