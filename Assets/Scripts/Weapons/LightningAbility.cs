using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/LightningAbility")]
[Serializable]
public class LightningAbility : Ability
{
    public int damage = 1;
    public int forks = 2;
    public GameObject lightningPrefab;
    public int[] shootableLayers;
    public int extraShots;
    public int multishotSpreadAngle = 20;
    public float weaponRange = 10f;
    //public int pierce;

    private LightningLauncher launcher;

    public override void Initialise(GameObject obj, List<AbilityPowerup> powerups)
    {
        // Find the launcher on the object
        launcher = obj.AddComponent<LightningLauncher>();
        // Set the appropriate values
        launcher.damage = damage;
        launcher.forks = forks;
        launcher.shootableLayers = shootableLayers;
        launcher.lightningPrefab = lightningPrefab;
        launcher.extraShots = extraShots + (int)PowerupThisProperty("extraShots", powerups);
        launcher.multishotSpreadAngle = multishotSpreadAngle;
        launcher.weaponRange = weaponRange * PowerupThisPropertyMultiply("weaponRange", powerups);
        //launcher.pierce = pierce + (int)PowerupThisProperty("pierce", powerups);

        launcher.Initialise();
    }

    public override void TriggerAbility(Vector3 direction)
    {
        launcher.Attack(direction);
    }
}