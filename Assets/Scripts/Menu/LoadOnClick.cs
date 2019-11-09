using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour
{
    public GameObject loadImage;

    public void LoadScene(int level)
    {
        loadImage.SetActive(true);
        SceneManager.LoadScene(level);
    }

    public void LoadNextScene()
    {
        loadImage.SetActive(true);
        // Load the next scene of the player
        PlayerData playerData = PlayerData.Instance;
        if (playerData.currentLevel > PlayerData.LastLevel)
            SceneManager.LoadScene(6);
        else
            SceneManager.LoadScene(playerData.currentLevel);
    }

    /// <summary>
    /// If the player has completed any levels, give them the change to 'respec', else just start the game
    /// </summary>
    public void LoadPowerupScreen()
    {
        PlayerData playerData = PlayerData.Instance;
        if (playerData.currentLevel > PlayerData.FirstLevel || playerData.difficultyLevel > 0)
        {
            SceneManager.LoadScene(3);
        } else
        {
            loadImage.SetActive(true);
            // Load the next scene of the player
            if (playerData.currentLevel > PlayerData.LastLevel)
                SceneManager.LoadScene(PlayerData.LastLevel);
            else
                SceneManager.LoadScene(playerData.currentLevel);
        }
    }

    /// <summary>
    /// Effectively starts again, resetting the player's powerups and current level
    /// </summary>
    public void ResetPlayerData()
    {
        PlayerData playerData = PlayerData.Instance;
        playerData.ResetPlayer();
    }

}
