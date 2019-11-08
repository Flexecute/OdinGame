using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackStrike : MonoBehaviour, IColdable
{
    public Transform firePoint;
    public float tellTime;
    public AudioEvent tellSound;

    internal AggroDetection aggroDetection;
    internal float attackTimer;
    internal PlayerAnimationController animator;
    internal Health healthTarget;
    internal Transform target;
    internal bool disabled;
    private AudioSource audioSource;


    public GameObject bulletPrefab;
    public float attackRate = 1f;
    public int damage = 0;
    public float bulletSpeed = 1f;
    public float weaponRange = 30f;
    public float bulletWidth = 1f;
    public int pierce = -1;
    public GameObject attackAnimationPrefab;
    public int[] shootableLayers;

    private ParticleSystem attackAnimation;
    public AudioEvent attackSound;

    private int shootableLayerMask;

    // Cold slowing
    internal float slowImpact = 1f; // 0 = Stopped, 1 = no impact
    internal bool attacking;

    internal bool Disabled { get => disabled; set => disabled = value; }

    internal void Awake()
    {
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += AggroDetection_OnAggro;
        // Set up the weapon
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<PlayerAnimationController>();
        // Randomly initialise the attack timer
        attackTimer = Random.Range(0, attackRate);

        if (attackAnimationPrefab != null)
        {
            GameObject newAnimation = Instantiate(attackAnimationPrefab, firePoint);
            // Grab the particle system from the animation
            attackAnimation = newAnimation.GetComponent<ParticleSystem>();
        }

        // Bit shift the index of the layer (8) to get a bit mask
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask = shootableLayerMask + (1 << layer);
        }

        // Factor attack rate according to difficulty
        int difficultyLevel = PlayerData.Instance.difficultyLevel;
        if (difficultyLevel > 0)
        {
            attackRate = attackRate / (difficultyLevel * PlayerData.difficultySpeedFactor);
            tellTime = tellTime / (difficultyLevel * PlayerData.difficultySpeedFactor);
        }


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
        // Play sound
        if (tellSound != null)
            tellSound.Play(audioSource);

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
        if (slowImpact > 0 )
        {
            attacking = false;
            // Determine direction of attack (ignore y axis)
            Vector3 direction = new Vector3(target.position.x - firePoint.transform.position.x, 0, target.position.z - firePoint.transform.position.z);

            // Animate flash
            if (attackAnimation != null)
                attackAnimation.Play();
            // Play sound
            if (attackSound != null)
                attackSound.Play(audioSource);

            // Create bullet
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            GameObject newBullet = Instantiate(bulletPrefab, firePoint.transform.position, rotation);
            newBullet.GetComponent<BulletWideMovement>().Initiailise(direction, bulletSpeed, damage, weaponRange, pierce, shootableLayerMask, bulletWidth);

            // Rotate towards direction of attack
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
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
