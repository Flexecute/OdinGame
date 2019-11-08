using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardLevelMenu : MonoBehaviour
{
    public Text previousTimeText;
    public Text infoTitle;
    public Text infoText;
    public GameObject[] powerupHolders;
    public AbilityPowerup[] powerups;

    public AbilityPowerup selectedPowerup;
    public GameObject selectedPowerupHolder;
    public GameObject[] abilityHolder;
    public GameObject victoryPanel;

    public PlayerData pd; // Dodgy pointer to playerData to prevent constant errors with the static singleton scriptable object
    private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        // Find the previous level completion time
        playerData = PlayerData.Instance;
        previousTimeText.text = Math.Round(playerData.previousLevelTime, 2) + " seconds";

        // Did the player just win? Show the victory panel
        if (playerData.currentLevel == PlayerData.FirstLevel)
        {
            victoryPanel.SetActive(true);
        }
        // Fill in the reward Holders with the rewards
        for (int i = 0; i < Math.Min(powerupHolders.Length, powerups.Length); i++)
        {
            GameObject newPowerup = new GameObject("Powerup");
            // Assign the powerup to the game object
            PowerupPointer pp = newPowerup.AddComponent<PowerupPointer>();
            pp.powerup = powerups[i];
            Image tmpImage = newPowerup.AddComponent<Image>();
            tmpImage.sprite = pp.powerup.sprite;
            newPowerup.transform.SetParent(powerupHolders[i].transform, false);
        }

        // Add in the abilities below to identify which abilities each power up works on
        for (int i = 0; i < playerData.abilities.Length; i++)
        {
            Ability ability = playerData.abilities[i];
            // Match the sprite with the ability
            Image firstSlotImage = abilityHolder[i].GetComponent<Image>();
            firstSlotImage.sprite = ability.aSprite;
        }

        // Select the first powerup
        SelectPowerup(powerupHolders[0]);

    }

    public void UpdateInfoText(GameObject powerupHolder)
    {
        AbilityPowerup powerup = powerups[Array.IndexOf(powerupHolders, powerupHolder)];
        infoText.text = powerup.infoText;
        infoTitle.text = powerup.title;
        // Highlight which abilities can be used by this powerup
        for (int i=0;i<playerData.abilities.Length;i++)
        {
            Ability ability = playerData.abilities[i];
            Image slotImage = abilityHolder[i].GetComponent<Image>();
            //SpriteRenderer m_SpriteRenderer = abilityHolder[i].GetComponent<SpriteRenderer>();
            //Set the GameObject's Color quickly to a set Color (blue)
            if (ability.CanUsePowerup(powerup))
            {
                // Highlight this abiltiy holder
                slotImage.color = Color.white;
            } else
            {
                slotImage.color = new Color(0.1f,0.1f,0.1f,0.5f);
            }
        }
    }

    /// <summary>
    /// Reverts the info text to the currently selected powerup
    /// </summary>
    public void RevertInfoText()
    {
        UpdateInfoText(selectedPowerupHolder);
    }

    public void SelectPowerup(GameObject powerupHolder)
    {
        // Unselect previous powerup
        if (selectedPowerupHolder != null)
            UnSelectPowerup(selectedPowerupHolder);
        AbilityPowerup powerup = powerups[Array.IndexOf(powerupHolders, powerupHolder)];
        selectedPowerup = powerup;
        selectedPowerupHolder = powerupHolder;
        UpdateInfoText(selectedPowerupHolder);
        // Enable the image on the holder
        Image image = powerupHolder.GetComponent<Image>();
        image.enabled = true;
    }

    private void UnSelectPowerup(GameObject powerupHolder)
    {
        // Disable the image on the holder
        Image image = powerupHolder.GetComponent<Image>();
        image.enabled = false;
        selectedPowerup = null;
    }

    public void SavePowerup()
    {
        PlayerData playerData = PlayerData.Instance;
        playerData.AddUnusedPowerup(selectedPowerup);
    }

}
