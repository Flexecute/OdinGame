using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    public GameObject abilityPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Find the abilities required for this player
        PlayerData playerData = PlayerData.Instance;
        AbilityCooldown[] coolDownButtons = abilityPanel.GetComponentsInChildren<AbilityCooldown>();

        for (int i=0;i<playerData.abilities.Length;i++)
        {
            Ability ability = playerData.abilities[i];
            // Add the Monobehaviour associated with this ability
            coolDownButtons[i].Initialise(ability, gameObject);
        }

        /*
        for (int i = 0; i < playerData.initialPowerupAbility.Length; i++)
        {
            Ability ability = playerData.initialPowerupAbility[i];
            // Power up the ability
            ability.Powerup(playerData.initialPowerups[i]);
        }
        */
    }


}
