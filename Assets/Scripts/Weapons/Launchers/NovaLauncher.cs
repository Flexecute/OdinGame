using UnityEngine;
using UnityEditor;
using System;

public class NovaLauncher : MonoBehaviour
{
    // These properties should be set by the Ability on Initialise
    [HideInInspector] public int damage = 1;
    [HideInInspector] public float radius = 10f;
    [HideInInspector] public float coldEffect = 0.5f;
    [HideInInspector] public float coldDuration = 5f;
    public GameObject attackAnimationPrefab;

    public int[] shootableLayers;
    private int shootableLayerMask;
    private ParticleSystem attackAnimation;

    private bool disabled;

    public void Initialise()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask = shootableLayerMask + (1 << layer);
        }
        // Instantiate a version of the animation prefab and find its particle system
        GameObject animationObject = Instantiate(attackAnimationPrefab, transform.position, transform.rotation);
        animationObject.transform.parent = transform;
        ParticleSystem[] pSystems = animationObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pSystems)
        {
            if (ps.gameObject.CompareTag("AttackEffect"))
            {
                attackAnimation = ps;
                break;
            }
        }
        if (attackAnimation == null)
            throw new ArgumentNullException("No AttackEffect particle system found in animationPrefab for NovaAttack");
    }

    public void Attack(Vector3 direction)
    {
        if (disabled)
            return;

        // Play animation
        if (attackAnimation != null)
            attackAnimation.Play();

        // Check if anything is hit within range
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, shootableLayerMask);
        foreach (Collider collider in colliders)
        {
            // Reduce the health of the 'shootable' if it has health
            var health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            IColdable[] coldables = collider.GetComponentsInChildren<IColdable>();
            foreach (IColdable coldable in coldables)
            {
                coldable.TakeColdDamage(coldEffect, coldDuration);
            }
        }

    }

    void OnDisable()
    {
        disabled = true;
    }

}