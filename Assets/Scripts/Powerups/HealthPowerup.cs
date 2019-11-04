
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerup/Health")]
public class HealthPowerup : PowerupEffect
{
    public int amount;

    public override void Apply(GameObject collector)
    {
        collector.GetComponent<Health>().AddHealth(amount);
    }
}
