using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCharge : MonoBehaviour, IColdable
{
    [SerializeField]
    private float attackRefreshRate = 3f;
    [SerializeField]
    private int attackDamage=1;
    [SerializeField]
    private float attackRange=20f;
    [SerializeField]
    private float tellTime;

    private PlayerAnimationController animator;

    private float attackTimer;
    private Health healthTarget;
    //private Transform target;
    private bool disabled;
    public bool Disabled { get => disabled; set => disabled = value; }
    private bool attacking;
    private float slowImpact=1f;
    private AggroDetection aggroDetection;
    private int obstacleMask;
    public int[] obstacleLayers;
    public float obstacleCheckHeight = 1f;

    private void Awake() {
        animator = transform.parent.GetComponentInChildren<PlayerAnimationController>();
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += AggroDetection_OnAggro;

        // Bit shift the index of the layers to get a bit mask
        foreach (int layer in obstacleLayers)
        {
            obstacleMask += (1 << layer);
        }
    }

    void Update()
    {
        if (disabled)
            return;

        if (healthTarget != null)
        {
            attackTimer += Time.deltaTime * slowImpact;
            if (CanAttack())
            {
                StartAttack();
            }
        }
    }

    private bool CanAttack()
    {
        return (healthTarget != null && attackTimer >= attackRefreshRate && PositionHelper.IsTargetObscured(transform, healthTarget.transform, obstacleCheckHeight, obstacleMask));
    }

    private void StartAttack()
    {
        if (slowImpact > 0)
        {
            attacking = true;
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
        attacking = false;
        // Check that the target is still in range
        if (healthTarget != null)
        {
            // Has the target moved out of range?
            if (System.Math.Abs(healthTarget.transform.position.x - transform.position.x) <= attackRange && 
                System.Math.Abs(healthTarget.transform.position.z - transform.position.z) <= attackRange)
                healthTarget.TakeDamage(attackDamage);
            //else
                //healthTarget = null;
        }
    }

    public void TakeColdDamage(float slowAmount, float duration) {
        slowImpact = (1 - slowAmount);
        Invoke("removeColdEffect", duration);
    }

    private void removeColdEffect() {
        slowImpact = 1f;
    }

    private void AggroDetection_OnAggro(Transform newTarget)
    {
        Health health = newTarget.GetComponent<Health>();
        if (health != null)
        {
            healthTarget = health;
        }
    }
}
