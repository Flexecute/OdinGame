using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/ShootAbility")]
[Serializable]
public class ShootAbility : Ability
{
    public int damage = 1;
    public float bulletSpeed = 1f;
    public GameObject bulletPrefab;
    public int[] shootableLayers;
    public int extraShots;
    private BulletLauncher launcher;
    public int multishotSpreadAngle = 10;
    public float weaponRange = 10f;
    public int pierce;


    public override void Initialise(GameObject obj, List<AbilityPowerup> powerups)
    {
        // Find the launcher on the object
        //launcher = obj.GetComponent<BulletLauncher>();
        launcher = obj.AddComponent<BulletLauncher>();
        // Set the appropriate values
        launcher.damage = damage + (int)PowerupThisProperty("damage", powerups);
        launcher.bulletSpeed = bulletSpeed * PowerupThisPropertyMultiply("bulletSpeed", powerups);
        launcher.bulletPrefab = bulletPrefab;
        launcher.shootableLayers = shootableLayers;
        launcher.extraShots = extraShots + (int)PowerupThisProperty("extraShots", powerups);
        launcher.multishotSpreadAngle = multishotSpreadAngle;
        launcher.weaponRange = weaponRange * PowerupThisPropertyMultiply("weaponRange", powerups);
        launcher.pierce = pierce + (int) PowerupThisProperty("pierce", powerups);
        launcher.Initialise();
    }

    public override void TriggerAbility(Vector3 direction)
    {
        launcher.Attack(direction);
    }
}