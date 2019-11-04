using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour, IColdable
{
    [SerializeField]
    internal float attackRate;
    [SerializeField]
    internal GameObject weaponPrefab;
    [SerializeField]
    internal Transform firePoint;
    [SerializeField]
    internal float tellTime;

    internal AggroDetection aggroDetection;

    internal float attackTimer;
    internal PlayerAnimationController animator;
    internal Health healthTarget;
    internal Transform target;
    internal bool disabled;
    internal Weapon weapon;

    // Cold slowing
    internal float slowImpact = 1f; // 0 = Stopped, 1 = no impact
    internal bool attacking;

    internal bool Disabled { get => disabled; set => disabled = value; }

    internal void Awake()
    {
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += AggroDetection_OnAggro;
        // Set up the weapon
        weapon = Instantiate(weaponPrefab.GetComponent<Weapon>(), firePoint.position, firePoint.rotation);
        attackRate = Mathf.Max(attackRate, weapon.getAttackRate());
        weapon.FirePoint = firePoint;
        animator = GetComponentInChildren<PlayerAnimationController>();
        // Randomly initialise the attack timer
        attackTimer = Random.Range(0, attackRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
            return;

        if (healthTarget != null)
        {
            attackTimer += Time.deltaTime* slowImpact;
            if (CanAttack())
            {
                StartAttack();
            }
        }
    }

    internal bool CanAttack()
    {
        return (attackTimer >= attackRate && !attacking);
    }

    /**
     * Starts the animation as if the enemy is going to attack (giving a tell to the player)
     */
    internal void StartAttack()
    {
        attacking = true;
        if (animator != null)
            animator.AnimateAttack(true);
        attackTimer = 0;
        // Determine direction of attack (ignore y axis)
        Vector3 direction = new Vector3(target.position.x - firePoint.transform.position.x, 0, target.position.z - firePoint.transform.position.z);
        // Rotate towards direction of attack
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        if (tellTime > 0 && slowImpact > 0)
            Invoke("ReallyAttack", tellTime/slowImpact);
        else
            ReallyAttack();
    }

    internal void ReallyAttack() {
        attacking = false;
        // Determine direction of attack (ignore y axis)
        Vector3 direction = new Vector3(target.position.x - firePoint.transform.position.x, 0, target.position.z - firePoint.transform.position.z);
        weapon.Attack(direction);
        // Rotate towards direction of attack
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

    }


    internal void AggroDetection_OnAggro(Transform newTarget)
    {
        target = newTarget;
        Health health = newTarget.GetComponent<Health>();
        if (health != null)
        {
            healthTarget = health;
        }
    }

    public void TakeColdDamage(float slowAmount, float duration)
    {
        slowImpact = (1 - slowAmount);
        Invoke("removeColdEffect", duration);
    }

    internal void removeColdEffect()
    {
        slowImpact = 1f;
    }


}
