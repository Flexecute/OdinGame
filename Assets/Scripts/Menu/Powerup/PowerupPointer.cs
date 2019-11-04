using UnityEngine;
using System.Collections;

/// <summary>
/// This class only exists to be able to add a reference of a SriptableObject (AbilityPowerup) to a GameObject
/// (The slots in the level up editor). This upsets me greatly, I think I need to re-write the powerups back to MonoBehaviours
/// </summary>
public class PowerupPointer : MonoBehaviour
{
    public AbilityPowerup powerup;

}
