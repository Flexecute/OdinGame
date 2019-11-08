using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackMelee : MonoBehaviour, IColdable
{
    [SerializeField]
    private float attackRate = 3f;
    [SerializeField]
    private int attackDamage=1;
    [SerializeField]
    private float attackRange=1f;
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

    private void Awake() {
        animator = transform.parent.GetComponentInChildren<PlayerAnimationController>();

        // Factor attack rate according to difficulty
        int difficultyLevel = PlayerData.Instance.difficultyLevel;
        if (difficultyLevel > 0)
        {
            attackRate = attackRate / (difficultyLevel * PlayerData.difficultySpeedFactor);
            tellTime = tellTime / (difficultyLevel * PlayerData.difficultySpeedFactor);
        }

    }

    void Update()
    {
        if (disabled)
            return;

        attackTimer += Time.deltaTime * slowImpact;
        if (healthTarget != null)
        {
            if (CanAttack())
            {
                StartAttack();
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //target = other.transform;
            healthTarget = other.GetComponent<Health>();
        }
    }

    /*  This doesn't work due to the other collider on the enemy apparently 
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //target = null;
            healthTarget = null;
        }
    }
    */

    private bool CanAttack()
    {
        return (healthTarget != null && attackTimer >= attackRate);
    }

    private void StartAttack()
    {
        if (slowImpact > 0)
        {
            attacking = true;
            attackTimer = 0;
            animator.AnimateAttack(true);
            if (tellTime > 0)
                Invoke("ReallyAttack", tellTime / slowImpact);
            else
                ReallyAttack();
        }

    }

    private void ReallyAttack() 
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
}
