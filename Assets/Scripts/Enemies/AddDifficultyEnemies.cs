using UnityEngine;
using System.Collections;

public class AddDifficultyEnemies : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        // Is the player up to the harder difficulty?
        PlayerData playerData = PlayerData.Instance;
        if (playerData.difficultyLevel <= 0)
        {
            // Find all enemies tagged with difficulty and level and de-activate them
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Difficulty1");
            
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(false);
            }
        }
    }
    
}
