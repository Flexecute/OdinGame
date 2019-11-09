using UnityEngine;
using UnityEditor;
using System;

public class BulletLauncher : MonoBehaviour
{
    // These properties should be set by the Ability on Initialise
    [HideInInspector] public int damage = 1;
    [HideInInspector] public float bulletSpeed = 1f;
    [HideInInspector] public GameObject bulletPrefab;
    [HideInInspector] public int extraShots;
    [HideInInspector] public float weaponRange;
    [HideInInspector] public int pierce;

    public int multishotSpreadAngle = 10;
    public ParticleSystem gunshotAnimation;
    public int[] shootableLayers;
    private int shootableLayerMask;

    private bool disabled;

    public void Attack(Vector3 direction)
    {
        if (disabled)
            return;
        //Debug.DrawRay(firePoint.position, firePoint.forward * 100, Color.red, 10f);

        // Animate flash
        if (gunshotAnimation != null)
            gunshotAnimation.Play();

        // Create bullet for each extra shots
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Offset direction to ensure it 'averages' to required direction
        if (extraShots > 0)
            direction = Quaternion.Euler(0, -multishotSpreadAngle*extraShots/2, 0) * direction;
        for (int i=0;i<extraShots+1;i++)
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, rotation);
            newBullet.GetComponent<BulletMovement>().Initiailise(direction, bulletSpeed, damage, weaponRange, pierce, shootableLayerMask);
            // Rotate the angle slightly
            direction = Quaternion.Euler(0, multishotSpreadAngle, 0) * direction;
        }

    }

    public void Initialise()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask = shootableLayerMask + (1 << layer);
        }
    }

    void OnDisable()
    {
        disabled = true;
    }
}