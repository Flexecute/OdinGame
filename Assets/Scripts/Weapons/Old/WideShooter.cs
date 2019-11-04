using UnityEngine;

[CreateAssetMenu(menuName = "EnemyWeapon/WideShooter")]
public class WideShooter : ScriptableObject
{
    public GameObject bulletPrefab;
    public float attackRate = 1f;
    public int damage = 0;
    public float bulletSpeed = 1f;
    public float weaponRange = 30f;
    public float bulletWidth = 1f;
    public int pierce=-1;
    public GameObject attackAnimationPrefab;
    public int[] shootableLayers;

    private ParticleSystem attackAnimation;
    public AudioEvent attackSound;

    private AudioSource audioSource;

    private int shootableLayerMask;
    private Transform firePoint;

    public void Initialise(Transform FirePoint, AudioSource aSource)
    {
        audioSource = aSource;
        firePoint = FirePoint;
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
    }

    public void Attack(Vector3 direction)
    {
        //Debug.DrawRay(firePoint.position, firePoint.forward * 100, Color.red, 10f);

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

    }

}
