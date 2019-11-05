using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Add an entry to the Assets menu for creating an asset of this [type
[Serializable]
public class PlayerData
{
    public Ability[] abilities; // This is the list of abilities for the player
    public List<AbilityPowerup> powerups; // A list of powerups applied on the players abilities
    public List<Ability> powerupOnAbility; // Which abilities the previous powerups are applied on
    public List<AbilityPowerup> unusedPowerups;

    public const int FirstLevel = 4;
    public const int LastLevel = 6;
    public int currentLevel=FirstLevel;
    public float previousLevelTime=0f;

    static PlayerData _instance = null;
    public string username = "Unknown";

    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                // Lazy loading
                _instance = CreateOrLoad();
            }
            return _instance;
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string tmp = JsonUtility.ToJson(this);
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        bf.Serialize(file, tmp);
        file.Close();
    }

    /// <summary>
    /// Loads the player data from file. If the file doesn't exist, will create a new instance of PlayerData
    /// </summary>
    static private PlayerData CreateOrLoad()
    {
        PlayerData newInstance;
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
                String tmp = (string)bf.Deserialize(file);
                file.Close();
                newInstance = JsonUtility.FromJson<PlayerData>(tmp);
            } catch (Exception e)
            {
                newInstance = createNewPlayerData();
            }
        } else
        {
            // Create a new one
            newInstance = createNewPlayerData();
        }
        return newInstance;
    }

    static private PlayerData createNewPlayerData()
    {
        PlayerData newInstance = new PlayerData();
        newInstance.abilities = new Ability[4];
        // Initialise abilities here?
        Ability[] allAbilities = Resources.FindObjectsOfTypeAll<Ability>();
        for (int i = 0; i < allAbilities.Length; i++)
        {
            Ability ability = allAbilities[i];
            // Match on strings??
            switch (ability.name)
            {
                case "Light":
                    newInstance.abilities[0] = ability;
                    break;
                case "LightningBolt":
                    newInstance.abilities[1] = ability;
                    break;
                case "ColdNova":
                    newInstance.abilities[2] = ability;
                    break;
                case "Teleport":
                    newInstance.abilities[3] = ability;
                    break;
            }

        }
        newInstance.powerups = new List<AbilityPowerup>();
        newInstance.powerupOnAbility = new List<Ability>();
        newInstance.unusedPowerups = new List<AbilityPowerup>();
        return newInstance;
    }

    public List<AbilityPowerup> GetAbilityPowerups(Ability selectedAbility)
    {
        List<AbilityPowerup> powerupReturn = new List<AbilityPowerup>();
        // Now add all the initial powerups
        for (int i = 0; i < powerups.Count; i++)
        {
            Ability ability = powerupOnAbility[i];
            if (ability == selectedAbility)
                powerupReturn.Add(powerups[i]);
        }
        return powerupReturn;
    }

    internal void AddUnusedPowerup(AbilityPowerup rewardPowerup)
    {
        unusedPowerups.Add(rewardPowerup);
    }

    internal void AddPowerup(Ability ability, AbilityPowerup powerup)
    {
        powerups.Add(powerup);
        powerupOnAbility.Add(ability);
    }
    /// <summary>
    /// Resets all powerups applied to the player
    /// </summary>
    public void ResetPowerups()
    {
        powerups.Clear();
        powerupOnAbility.Clear();
    }
    /// <summary>
    /// Resets all powerups applied to the player, all unused power ups and starts player back at first level
    /// </summary>
    public void ResetPlayer()
    {
        powerups.Clear();
        powerupOnAbility.Clear();
        unusedPowerups.Clear();
        currentLevel = FirstLevel;
    }

    internal void SetUnusedPowerups(List<AbilityPowerup> powerups)
    {
        unusedPowerups = powerups;
    }

    /// <summary>
    /// Called when the player completes the current scene they're on
    /// </summary>
    /// <param name="timeTaken"></param>
    internal void CompleteScene(float timeTaken)
    {
        currentLevel++;
        // Go to extra difficulty if level's > total levels

        // Record the amount of time taken
        previousLevelTime = timeTaken;
    }

    /* This didn't work due to serialization problems, I believe with the List
public Dictionary<Ability, List<AbilityPowerup>> abilityPowerups = new Dictionary<Ability, List<AbilityPowerup>>();
public AbilityPowerup[] initialPowerups;
public Ability[] initialPowerupAbility;

// This is unreliable
public void Awake()
{
    // Load up the initialPowerups into abilityPowerups
    for (int i=0;i<initialPowerups.Length;i++)
    {
        Ability ability = initialPowerupAbility[i];
        List<AbilityPowerup> powerups;
        if (abilityPowerups.ContainsKey(ability))
            powerups = abilityPowerups[ability];
        else
        {
            powerups = new List<AbilityPowerup>();
            abilityPowerups[ability] = powerups;
        }
        powerups.Add(initialPowerups[i]);
    }
}

public List<AbilityPowerup> GetAbilityPowerups(Ability selectedAbility)
{
    List<AbilityPowerup> powerups;
    if (abilityPowerups.ContainsKey(selectedAbility))
        powerups = abilityPowerups[selectedAbility];
    else
        powerups = new List<AbilityPowerup>();
    // Now add all the initial powerups
    for (int i = 0; i < initialPowerups.Length; i++)
    {
        Ability ability = initialPowerupAbility[i];
        if (ability == selectedAbility)
            powerups.Add(initialPowerups[i]);
    }
    return powerups;
}

internal void SetAbilityPowerups(Ability ability, List<AbilityPowerup> powerups)
{
    abilityPowerups[ability] = powerups;
}


  */
}