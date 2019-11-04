using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAbilities : MonoBehaviour
{
    public Ability[] abilties;
    public AbilityPowerup[] abiltyPowerups;

    public void OnDestroy()
    {
        PlayerData playerData = PlayerData.Instance;
        playerData.Save();
    }
}
