using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/NovaAbility")]
[Serializable]
public class NovaAbility : Ability
{
    public int damage = 1;
    public float radius = 10f;
    public float coldEffect = 0.5f;
    public float coldDuration = 5f;
    public GameObject attackAnimationPrefab;
    public int[] shootableLayers;

    private NovaLauncher launcher;

    public override void Initialise(GameObject obj, List<AbilityPowerup> powerups)
    {
        // Find the launcher on the object
        launcher = obj.AddComponent<NovaLauncher>();
        // Set the appropriate values
        launcher.damage = damage;
        launcher.radius = radius;
        launcher.coldEffect = coldEffect;
        launcher.coldDuration = coldDuration;
        launcher.attackAnimationPrefab = attackAnimationPrefab;
        launcher.shootableLayers = shootableLayers;
        launcher.Initialise();
    }

    public override void TriggerAbility(Vector3 direction)
    {
        launcher.Attack(direction);
    }
}