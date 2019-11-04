using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovementHide_Translate : MonoBehaviour, IColdable
{
    // Hiding attributes
    public float movementSpeed = 2f;
    public float maxDistance = 20f;
    public float hideDistance = 10f;
    public float closeEnough = 0.1f;
    public float obstacleViewRadius = 20f;
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
        // Don’t update position automatically
        navMeshAgent.updatePosition = false;
        rotationSpeed = navMeshAgent.angularSpeed;
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += avoidTarget;
        // Bit shift the index of the layers to get a bit mask
        foreach (int layer in obstacleLayers)
        {
            obstacleMask += (1 << layer);
        }
        path = new NavMeshPath();
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
        if ((transform.position - target.transform.position).magnitude >= maxDistance)
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
        if (obstacle != null && !havePath && isTargetObscured(transform, target, obstacle))
        {
            hidden = true;
            havePath = false;
            animator.SetFloat("Speed", 0);
            return;
        }
        hidden = false;

        // Do we already have a path to hide?
        if (!havePath)
        {
            FindNewHidingPosition(target);
        }
        // If a target position has been identified, move towards it
        if (havePath)
        {
            if (MoveToDestination(Time.deltaTime) ==true)
            {
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
        navMeshAgent.nextPosition = transform.position;
        navMeshAgent.CalculatePath(destinationPosition, path);
        //navMeshAgent.SetDestination(destinationPosition);
        if (path.corners.Length > 1)
        {
            currentCorner = 1; // First corner = current position
            havePath = true;
        } else
        {
            havePath = false;
        }
    }

    /**
     * Moves the transform along the current path to its destination
     * Returns true once it has reached its destination
     *
     * */
    private bool MoveToDestination(float deltaTime)
    {
        //transform.Translate((navMeshAgent.nextPosition - transform.position).normalized * movementSpeed * deltaTime, Space.World);
        Vector3 vectorToCorner = path.corners[currentCorner] - transform.position;
        // Are we close enough to our current corner?
        if (vectorToCorner.magnitude <= closeEnough)
        {
            currentCorner++;
            if (currentCorner >= path.corners.Length)
            {
                // We've made it to to our destination
                animator.SetFloat("Speed", 0);
                return true;
            }
            vectorToCorner = path.corners[currentCorner] - transform.position;
        }
        //transform.position = navMeshAgent.nextPosition;
        // Try to move to the next corner
        transform.Translate(vectorToCorner.normalized * movementSpeed * slowImpact * deltaTime, Space.World);
        // Set speed on the animator based on movementSpeed
        animator.SetFloat("Speed", movementSpeed);
        return false;
    }


    /**
     * Check if a straight line between origin and target would be blocked by obstacle
     **/
    private bool isTargetObscured(Transform origin, Transform target, Collider obstacle)
    {
        Vector3 direction = new Vector3(target.position.x - origin.position.x, 0, target.position.z - origin.position.z);
        //Debug.DrawRay(new Vector3(origin.position.x, obstacleCheckHeight, origin.position.z), direction);
        return Physics.Raycast(new Vector3(origin.position.x, obstacleCheckHeight, origin.position.z), direction, direction.magnitude, obstacleMask);
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
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
        Invoke("removeColdEffect", duration);
    }

    private void removeColdEffect()
    {
        slowImpact = 1f;
    }
}
