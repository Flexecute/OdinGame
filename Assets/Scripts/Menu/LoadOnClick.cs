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
        SceneManager.LoadScene(playerData.currentLevel);
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
