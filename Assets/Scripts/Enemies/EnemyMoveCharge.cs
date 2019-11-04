using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementCharge : MonoBehaviour, IColdable
{
    private PlayerAnimationController animator;
    private NavMeshAgent navMeshAgent;
    private AggroDetection aggroDetection;
    private Transform target;

    private bool disabled;
    private float rotationSpeed;

    // Cold slowing
    private float slowImpact = 1f; // 0 = Stopped, 1 = no impact
    private float baseSpeed;

    // Attacking variables
    [SerializeField]
    private float attackRefreshRate = 3f;
    [SerializeField]
    private int attackDamage = 1;
    [SerializeField]
    private float attackRange = 20f;
    [SerializeField]
    private float tellTime;
    public float chargeSpeed = 3f;

    private float attackTimer;
    private Health healthTarget;
    //private Transform target;
    private bool attacking;
    private int obstacleMask;
    public int[] obstacleLayers;
    public float obstacleCheckHeight = 1f;
    private bool charging;

    private void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimationController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navMeshAgent.speed;
        rotationSpeed = navMeshAgent.angularSpeed;
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += trackTarget;

        // Bit shift the index of the layers to get a bit mask
        foreach (int layer in obstacleLayers)
        {
            obstacleMask += (1 << layer);
        }

    }

    private void trackTarget(Transform target)
    {
        this.target = target;
        navMeshAgent.SetDestination(target.position);
        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            healthTarget = health;
        }
    }

    public void disable()
    {
        disabled = true;
        navMeshAgent.isStopped = true;
    }

    private void Update()
    {
        if (target != null && !disabled) {
            // If we're attacking, we should be charging towards the target /  
            if (attacking)
            {

            } else
            {
                attackTimer += Time.deltaTime * slowImpact;
                if (CanAttack())
                {
                    StartAttack();
                } else
                {
                    // Move to target
                    MoveTowards(target);
                }
            }
        }
    }

    private bool CanAttack()
    {
        if (attackTimer < attackRefreshRate)
            return false;
        // Is there something in the way?
        if (PositionHelper.IsTargetObscured(transform, target.transform, obstacleCheckHeight, obstacleMask))
            return false;
        return true;
    }

    private void StartAttack()
    {
        if (slowImpact > 0)
        {
            attacking = true;
            navMeshAgent.SetDestination(target.position);
            // Stop moving
            navMeshAgent.speed = 0;
            attackTimer = 0;
            animator.AnimateAttack(true);
            if (tellTime > 0)
                Invoke("StartCharge", tellTime / slowImpact);
            else
                StartCharge();
        }

    }

    private void StartCharge()
    {
        charging = true;
        // Set speed on the animator based on velocity of the navMeshAgent
        navMeshAgent.speed = chargeSpeed;
        animator.AnimateSpeed(chargeSpeed);
        // Check that the target is still in range
        if (healthTarget != null)
        {
            // Has the target moved out of range?
            if (Math.Abs(healthTarget.transform.position.x - transform.position.x) <= attackRange &&
                Math.Abs(healthTarget.transform.position.z - transform.position.z) <= attackRange)
                healthTarget.TakeDamage(attackDamage);
            //else
            //healthTarget = null;
        }
    }

    private void MoveTowards(Transform target)
    {
        navMeshAgent.SetDestination(target.position);
        // Set speed on the animator based on velocity of the navMeshAgent
        animator.AnimateSpeed(navMeshAgent.velocity.magnitude);
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
