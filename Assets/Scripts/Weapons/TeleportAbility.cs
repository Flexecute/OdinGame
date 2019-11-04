using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/TeleportAbility")]
[Serializable]
public class TeleportAbility : Ability
{
    private TeleportLauncher launcher;
    //public CharacterController characterController;

    public override void Initialise(GameObject obj, List<AbilityPowerup> powerups)
    {
        // Find the launcher on the object
        launcher = obj.AddComponent<TeleportLauncher>();
        launcher.characterController = obj.transform.parent.GetComponent<CharacterController>();
    }

    public override void TriggerAbility(Vector3 direction)
    {
        launcher.Attack(direction);
    }
}