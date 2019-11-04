using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementTrack : MonoBehaviour, IColdable
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private AggroDetection aggroDetection;
    private Transform target;

    private bool disabled;
    private float meleeRange;
    private float rotationSpeed;

    // Cold slowing
    private float slowImpact = 1f; // 0 = Stopped, 1 = no impact
    private float baseSpeed;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navMeshAgent.speed;
        meleeRange = navMeshAgent.stoppingDistance;
        rotationSpeed = navMeshAgent.angularSpeed;
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += trackTarget;
    }

    private void trackTarget(Transform target)
    {
        this.target = target;
        navMeshAgent.SetDestination(target.position);
    }

    public void disable()
    {
        disabled = true;
        navMeshAgent.isStopped = true;
    }

    private void Update()
    {
        if (target != null && !disabled) {

            if (IsInMeleeRangeOf(target))
            {
                navMeshAgent.isStopped = true;
                animator.SetFloat("Speed", 0);
                RotateTowards(target);
            } else
            {
                // Move to target
                MoveTowards(target);
            }
        }
    }

    private bool IsInMeleeRangeOf(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < meleeRange;
    }

    private void MoveTowards(Transform target)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target.position);
        // Set speed on the animator based on velocity of the navMeshAgent
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * slowImpact);
    }

    public void TakeColdDamage(float slowAmount, float duration)
    {
        slowImpact = (1 - slowAmount);
        navMeshAgent.speed = baseSpeed * slowImpact;
        Invoke("removeColdEffect", duration);
    }

    private void removeColdEffect()
    {
        navMeshAgent.speed = baseSpeed;
        slowImpact = 1f;
    }
}
