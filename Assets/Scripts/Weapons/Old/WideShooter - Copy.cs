using UnityEngine;

public class WideShooter_Backup : Weapon //MonoBehaviour, IWeapon
{
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float attackRate = 1f;

    [SerializeField]
    private int damage = 0;

    [SerializeField]
    private float bulletSpeed = 1f;
    [SerializeField]
    private float weaponRange = 30f;
    public float bulletWidth = 1f;
    [SerializeField]
    private int pierce=-1;

    [SerializeField]
    private ParticleSystem gunshotAnimation;

    [SerializeField]
    private SimpleAudioEvent gunSound;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    public int[] shootableLayers;
    private int shootableLayerMask;


    private void Awake()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask = shootableLayerMask + (1 << layer);
        }
    }
    public override float getAttackRate()
    {
        return attackRate;
    }
    public override int getShootableLayerMask() {
        return shootableLayerMask;
    }

    public override void Attack(Vector3 direction)
    {
        //Debug.DrawRay(firePoint.position, firePoint.forward * 100, Color.red, 10f);

        // Animate flash
        if (gunshotAnimation != null)
            gunshotAnimation.Play();
        // Animate flash
        if (gunSound != null)
            gunSound.Play(audioSource);

        // Create bullet
        //Quaternion rotation = new Quaternion(0, FirePoint.transform.rotation.y, FirePoint.transform.rotation.z, FirePoint.transform.rotation.w);
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        GameObject newBullet = Instantiate(bulletPrefab, FirePoint.transform.position, rotation);
        newBullet.GetComponent<BulletWideMovement>().Initiailise(direction, bulletSpeed, damage, weaponRange, pierce, shootableLayerMask, bulletWidth);

    }

}
