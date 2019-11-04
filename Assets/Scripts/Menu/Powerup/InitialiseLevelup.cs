using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialiseLevelup : MonoBehaviour
{
    public GameObject[] abilitySlots;
    public GameObject[] upgradePanels;
    public GameObject[] unusedUpgradeSlots;
    
    PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {

        playerData = PlayerData.Instance;
        // Go through each ability of the player and initialise the first slot of each panel with the icon of the ability
        for (int i=0;i<playerData.abilities.Length;i++)
        {
            Ability ability = playerData.abilities[i];
            // Match the sprite with the ability
            Image firstSlotImage = abilitySlots[i].GetComponent<Image>();
            firstSlotImage.sprite = ability.aSprite;
            // Match the upgradePanel slots with the appropriate ability
            Slot[] upgradeSlots = upgradePanels[i].GetComponentsInChildren<Slot>();
            // Add in the upgrades for each ability upgrade
            List<AbilityPowerup> powerups = playerData.GetAbilityPowerups(ability);
            for (int j=0;j< upgradeSlots.Length;j++)
            {
                if (j < powerups.Count)
                {
                    // Instantiate the power up
                    //GameObject newPowerup = Instantiate(powerups[j], upgradeSlots[j].transform);
                    GameObject newPowerup = new GameObject("Powerup");
                    newPowerup.AddComponent<CanvasGroup>();
                    newPowerup.AddComponent<DragHandler>();
                    // Assign the powerup to the game object
                    PowerupPointer pp = newPowerup.AddComponent<PowerupPointer>();
                    pp.powerup = powerups[j];
                    Image tmpImage = newPowerup.AddComponent<Image>();
                    tmpImage.sprite = powerups[j].sprite;
                    newPowerup.transform.SetParent(upgradeSlots[j].transform, false);
                }
                // Lable the slot with the appropriate ability
                upgradeSlots[j].ability = ability;
            }
            //Slot firstSlot = abilitySlots[0];
        }
        // Load in any unused power ups
        for (int i = 0; i < System.Math.Min(unusedUpgradeSlots.Length, playerData.unusedPowerups.Count); i++)
        {
            GameObject newPowerup = new GameObject("Powerup");
            newPowerup.AddComponent<CanvasGroup>();
            newPowerup.AddComponent<DragHandler>();
            // Assign the powerup to the game object
            PowerupPointer pp = newPowerup.AddComponent<PowerupPointer>();
            pp.powerup = playerData.unusedPowerups[i];
            Image tmpImage = newPowerup.AddComponent<Image>();
            tmpImage.sprite = playerData.unusedPowerups[i].sprite;
            newPowerup.transform.SetParent(unusedUpgradeSlots[i].transform, false);
        }
    }

    public void ApplyUpgrades()
    {
        // Reset all upgrades to begin
        playerData.ResetPowerups();
        // Go through each ability of the player and initialise the first slot of each panel with the icon of the ability
        for (int i = 0; i < playerData.abilities.Length; i++)
        {
            Ability ability = playerData.abilities[i];
            Slot[] upgradeSlots = upgradePanels[i].GetComponentsInChildren<Slot>();
            // Add in the upgrades for each ability upgrade
            //List<AbilityPowerup> powerups = new List<AbilityPowerup>();
            for (int j = 0; j < upgradeSlots.Length; j++)
            {
                // Is there anything in this slot?
                if (upgradeSlots[j].item != null)
                {
                    // Find the powerup associated with it
                    PowerupPointer pp = upgradeSlots[j].item.GetComponent<PowerupPointer>();
                    AbilityPowerup powerup = pp.powerup;
                    playerData.AddPowerup(ability, powerup);
                    //powerups.Add(powerup);
                }
            }
            // Set the playerData ability powerups for this ability
            //playerData.SetAbilityPowerups(ability, powerups);
        }

        List<AbilityPowerup> unusedPowerups = new List<AbilityPowerup>();
        // Load in any unused power ups
        for (int i = 0; i < unusedUpgradeSlots.Length; i++)
        {
            Slot unusedSlot = unusedUpgradeSlots[i].GetComponent<Slot>();
            // Is there anything in this slot?
            if (unusedSlot.item != null)
            {
                // Find the powerup associated with it
                PowerupPointer pp = unusedSlot.item.GetComponent<PowerupPointer>();
                AbilityPowerup powerup = pp.powerup;
                unusedPowerups.Add(powerup);
            }
        }
        playerData.SetUnusedPowerups(unusedPowerups);
    }

}
