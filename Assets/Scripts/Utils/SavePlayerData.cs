using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePlayerData : MonoBehaviour
{
    public void OnDestroy()
    {
        PlayerData playerData = PlayerData.Instance;
        playerData.Save();
    }
}
