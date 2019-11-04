using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;
using System;

public class TeleportAttack : Weapon
{
    [SerializeField]
    private float radius = 0f;

    [SerializeField]
    private float attackRate = 2f;

    [SerializeField]
    private int damage = 0;

    [SerializeField]
    GameObject animationPrefab;
    ParticleSystem attackAnimation;

    [SerializeField]
    private AudioSource sound;

    [SerializeField]
    private Collider boundingBox;

    [SerializeField]
    public int[] shootableLayers;
    private int shootableLayerMask;

    private CharacterController characterController;

    private void Awake()
    {
        // Find the Character Controller (Doesn't work as we're on a prefab .. getting in FirePoint now but want to change it
        //characterController = GetComponentInParent<CharacterController>();

        // Bit shift the index of the layer (8) to get a bit mask
        if (damage > 0 && shootableLayers.Length <= 0)
            throw new ArgumentNullException("No shootable layers set for TeleportAttack");
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
        characterController = player.GetComponent<CharacterController>();
        FirePoint = newFirePoint;
        // Create a new animation object from the prefab
        if (animationPrefab != null)
        {
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
        }
    }

    public override void Attack(Vector3 direction)
    {
        // Teleport character Controller
        Vector3 newPosition = new Vector3(FirePoint.position.x + direction.x, 0f, FirePoint.position.z + direction.z);
        // Check if end point is outside the bounding box
        if (boundingBox != null && !boundingBox.bounds.Contains(newPosition))
            return;
        // Play sound
        if (sound != null)
            sound.Play();

        // Play animation
        if (attackAnimation != null)
            attackAnimation.Play();

        characterController.enabled = false;
        characterController.transform.position = newPosition;
        characterController.enabled = true;

        // Check if anything is hit within range
        if (damage > 0)
        {
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
            }
        }

    }

}
