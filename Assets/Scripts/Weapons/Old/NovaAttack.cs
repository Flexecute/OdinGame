using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;
using System;

public class NovaAttack : Weapon
{
    [SerializeField]
    private float radius = 10f;

    [SerializeField]
    private float attackRate = 2f;

    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float coldEffect = 0.5f;
    [SerializeField]
    private float coldDuration = 5f;

    [SerializeField]
    private float forkRadius = 10;
    private int forks;
    private List<ParticleSystem> forkObjects;

    [SerializeField]
    GameObject animationPrefab;
    ParticleSystem attackAnimation;

    [SerializeField]
    private AudioSource sound;

    [SerializeField]
    public int[] shootableLayers;
    private int shootableLayerMask;

    private void Awake()
    {

        // TODO: Make sure you have the root particle system?
        forkObjects = new List<ParticleSystem>();
        // Bit shift the index of the layer (8) to get a bit mask
        if (shootableLayers.Length <= 0)
            throw new ArgumentNullException("No shootable layers set for NovaAttack");
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask += (1 << layer);
        }
    }
    public override float getAttackRate()
    {
        return attackRate;
    }
    public override int getShootableLayerMask() {
        return shootableLayerMask;
    }

    public override void setFirePoint(Transform newFirePoint, GameObject player)
    {
        FirePoint = newFirePoint;
        // Create a new animation object from the prefab
        GameObject newAnimation = Instantiate(animationPrefab, FirePoint.transform.position, FirePoint.transform.rotation);
        newAnimation.transform.SetParent(FirePoint);
        // Find the particle system from the prefab
        ParticleSystem[] pSystems = newAnimation.GetComponentsInChildren<ParticleSystem>();
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

    public void setNumForks(int numForks)
    {
        for (int i=forks; i<numForks;i++)
        {
            // Create a new prefab for this lightning fork
            GameObject tmp = Instantiate(animationPrefab, this.transform.position, this.transform.rotation);
            // Find the attack animation for this prefab
            ParticleSystem[] pSystems = tmp.GetComponents<ParticleSystem>();
            ParticleSystem forkAttack = null;
            foreach (ParticleSystem ps in pSystems)
            {
                if (ps.gameObject.CompareTag("AttackEffect"))
                {
                    forkAttack = ps;
                    break;
                }
            }
            if (forkAttack == null)
                throw new ArgumentNullException("No AttackEffect particle system found in animationPrefab for NovaAttack");
            forkObjects.Add(forkAttack);
        }
        forks = numForks;
    }

    public override void Attack(Vector3 direction)
    {
        // Play sound
        if (sound != null)
            sound.Play();

        // Play animation
        attackAnimation.Play();

        // Check if anything is hit within range
        Collider[] colliders = Physics.OverlapSphere(FirePoint.position, radius, shootableLayerMask);
        int numForks = 0;
        foreach (Collider collider in colliders)
        {
            // Reduce the health of the 'shootable' if it has health
            var health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            IColdable[] coldables = collider.GetComponents<IColdable>();
            foreach (IColdable coldable in coldables)
            {
                coldable.TakeColdDamage(coldEffect, coldDuration);
            }
            // Fork if possible
            if (numForks < forks)
            {
                numForks++;
            }
        }
        
    }

}
