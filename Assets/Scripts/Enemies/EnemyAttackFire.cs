using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackFire : MonoBehaviour, IColdable
{
    public Transform firePoint;
    public float tellTime;
    public AudioEvent tellSound;

    public float attackRate;
    public float attackDuration;
    public float attackWidth;
    public float attackLength;
    public float fireRotationSpeed;
    public int damage;

    internal AggroDetection aggroDetection;
    internal float attackTimer;
    internal float currentAttackDuration;
    internal PlayerAnimationController animator;
    internal Health healthTarget;
    internal Transform target;
    internal bool disabled;
    private AudioSource audioSource;
    public GameObject attackAnimationPrefab;
    private ParticleSystem attackAnimation;

    public int[] shootableLayers;
    private int shootableLayerMask;


    // Cold slowing
    internal float slowImpact = 1f; // 0 = Stopped, 1 = no impact

    internal bool attacking;
    private bool reallyAttacking;
    private HashSet<Health> ignoreTargets = new HashSet<Health>();
    private Quaternion fireRotation;
    
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
        // Create a copy of the attack Prefab and attach it to the firepoint
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
            if (attacking)
            {
                // Are we actually attacking, or just winding up?
                if (reallyAttacking)
                {
                    if (currentAttackDuration >= attackDuration)
                    {
                        attacking = false;
                        reallyAttacking = false;
                        attackTimer = 0f;
                        if (attackAnimation != null)
                            attackAnimation.Stop();
                    } else
                    {
                        CheckForCollision();
                    }
                }
            } else
            {
                attackTimer += Time.deltaTime * slowImpact;
                if (CanAttack())
                {
                    StartAttack();
                }
            }
        }
    }

    internal bool CanAttack()
    {
        return (attackTimer >= attackRate);
    }

    /**
     * Starts the animation as if the enemy is going to attack (giving a tell to the player)
     */
    internal void StartAttack()
    {
        attacking = true;
        currentAttackDuration = 0f;
        if (animator != null)
            animator.AnimateAttack(true);
        // Play sound
        if (tellSound != null)
            tellSound.Play(audioSource);

        // Determine direction of attack (ignore y axis)
        //Vector3 direction = new Vector3(target.position.x - firePoint.transform.position.x, 0, target.position.z - firePoint.transform.position.z);
        // Rotate towards direction of attack
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        //transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        fireRotation = transform.rotation;
        if (tellTime > 0 && slowImpact > 0)
            Invoke("ReallyAttack", tellTime/slowImpact);
        else
            ReallyAttack();
    }

    internal void ReallyAttack() {
        // Clear ignoreTargets so we can hit everyone again
        ignoreTargets.Clear();        
        // Start particle effect
        if (attackAnimation != null)
            attackAnimation.Play();
        reallyAttacking = true;
    }

    private void CheckForCollision()
    {
        currentAttackDuration += Time.deltaTime;

        // Did we hit anything?
        Vector3 boxHalfExtents = new Vector3(attackWidth/2, 1f, attackLength/2);
        // Lerp the fire rotation towards where the transform is looking 
        fireRotation = Quaternion.Lerp(fireRotation, transform.rotation, Time.deltaTime * fireRotationSpeed);

        Vector3 attackMiddle = firePoint.position + fireRotation * Vector3.forward * attackLength / 2;
        Collider[] hitColliders = Physics.OverlapBox(attackMiddle, boxHalfExtents, fireRotation, shootableLayerMask);
        ExtDebug.DrawBox(attackMiddle, boxHalfExtents, fireRotation, Color.red);
        //Debug.Log(Time.deltaTime + " " + prevPos.ToString());
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            // Reduce the health of the 'shootable' if it has health
            var health = hitColliders[i].GetComponent<Health>();
            // Can only hit each target once
            if (health != null && !ignoreTargets.Contains(health))
            {
                health.TakeDamage(damage);
                ignoreTargets.Add(health);
                // Knockback
                //hits[i].collider.transform.parent.position += direction;
            }
            i++;
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
