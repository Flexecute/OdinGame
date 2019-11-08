using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovementHide : MonoBehaviour, IColdable
{
    // Hiding attributes
    public float movementSpeed = 2f;
    public float maxDistance = 30f;
    public float hideDistance = 5f;
    public float obstacleViewRadius = 40f;
    public int[] obstacleLayers;
    public float obstacleCheckHeight=1f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private AggroDetection aggroDetection;
    private Transform target;

    // Tracking attributes
    private Vector3 destinationPosition;
    private Vector3 destinationDirection;
    NavMeshPath path;
    int currentCorner;

    // Cold slowing
    private float slowImpact=1f; // 0 = Stopped, 1 = no impact

    private float rotationSpeed;
    private Collider obstacle;
    private bool hidden;
    private bool havePath;
    private int obstacleMask;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = movementSpeed;
        rotationSpeed = navMeshAgent.angularSpeed;
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += avoidTarget;
        // Bit shift the index of the layers to get a bit mask
        foreach (int layer in obstacleLayers)
        {
            obstacleMask += (1 << layer);
        }
        path = new NavMeshPath();

        // Factor movement speed according to difficulty
        int difficultyLevel = PlayerData.Instance.difficultyLevel;
        if (difficultyLevel > 0)
            navMeshAgent.speed = navMeshAgent.speed * (difficultyLevel * PlayerData.difficultySpeedFactor);
    }

    private void avoidTarget(Transform target)
    {
        this.target = target;
        // Wait for next update
    }

    private void Update()
    {
        // Don't do anything if we don't have a target
        if (target == null)
            return;

        RotateTowards(target);

        // Are we far enough away to give up?
        if (Vector3.Distance(transform.position, target.transform.position) >= maxDistance)
        {
            animator.SetFloat("Speed", 0);
            return;
        }
        // Do we have an obstacle to hide behind?
        if (obstacle == null)
        {
            // Look for an obstacle
            obstacle = FindObstacle();
        }
        // Are we hidden ?
        if (obstacle != null && PositionHelper.IsTargetObscured(transform, target, obstacleCheckHeight, obstacleMask))
        {
            hidden = true;
            havePath = false;
            animator.SetFloat("Speed", 0);
            obstacle = null;
            return;
        }
        hidden = false;

        // Do we already have a path to hide?
        if (!havePath)
        {
            FindNewHidingPosition(target);
        } else {
            if (ReachedDestination() == true)
            {
                animator.SetFloat("Speed", 0);
                // We have reached our destination
                havePath = false;
            }
        }
    }

    /**
     * Finds a new place to hide
     */
    private void FindNewHidingPosition(Transform target)
    {
        // Do we have an obstacle to hide behind?
        if (obstacle != null)
        {
            // Try to move such that the obstacle will be in between us and the target
            Vector3 direction = (obstacle.transform.position - target.position).normalized;
            destinationPosition = obstacle.transform.position + direction * hideDistance;
        }
        else
        {
            // Try to find an obstacle while retreating
            Vector3 direction = (transform.position - target.position).normalized;
            destinationPosition = transform.position + direction * hideDistance;
        }
        // Use navMeshAgent to find a path to the destination
        navMeshAgent.CalculatePath(destinationPosition, path);
        if (path.corners.Length > 1)
        {
            navMeshAgent.SetDestination(destinationPosition);
            animator.SetFloat("Speed", movementSpeed);
            havePath = true;
        } else
        {
            havePath = false;
        }
    }

    /**
     * Returns true if NavMesh has reached its destination
     *
     * */
    private bool ReachedDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }



    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * slowImpact);
    }

    private Collider FindObstacle()
    {
        // Look for any colliders within range
        Collider[] obstacleColliders = Physics.OverlapSphere(transform.position, obstacleViewRadius, obstacleMask);
        if (obstacleColliders.Length > 0)
        {
            float minDistance = obstacleViewRadius;
            Collider closestCollider = null;
            // Find the obstacle which is closest to us
            foreach (Collider collider in obstacleColliders)
            {
                float tmpDistance = (collider.transform.position - transform.position).magnitude;
                if (tmpDistance < minDistance)
                {
                    closestCollider = collider;
                    minDistance = tmpDistance;
                }
            }
            // Just take the first one
            return closestCollider;
        }
        return null;
    }

    public void TakeColdDamage(float slowAmount, float duration)
    {
        slowImpact = (1-slowAmount);
        navMeshAgent.speed = navMeshAgent.speed*slowImpact;
        Invoke("removeColdEffect", duration);
    }

    private void removeColdEffect()
    {
        slowImpact = 1f;
        navMeshAgent.speed = movementSpeed;
    }
}
