using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitVolSliders : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public MixLevels mixLevels;

    public GameObject startNewButton;
    public GameObject continueButton;
    public GameObject usernamePanel;
    // Start is called before the first frame update
    void Start()
    {
        // Has the player never played before?
        PlayerData playerData = PlayerData.Instance;
        if (playerData.username == "Unknown")
        {
            // Disable the start new button until the user enters a username
            continueButton.SetActive(false);
            // Pop up the new username panel
            usernamePanel.SetActive(true);
        }

        if (playerData.currentLevel <= PlayerData.FirstLevel)
        {
            // Disable the start new button
            startNewButton.SetActive(false);
            // Rename the continue button to start
            Text txt = continueButton.GetComponentInChildren<Text>();
            txt.text = "Start";
        }

        if (PlayerPrefs.HasKey("masterVol"))
        {
            masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("masterVol"));
            mixLevels.SetMasterLvl(PlayerPrefs.GetFloat("masterVol"));
        }
        if (PlayerPrefs.HasKey("musicVol"))
        {
            musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("musicVol"));
            mixLevels.SetMusicLvl(PlayerPrefs.GetFloat("musicVol"));
        }
        if (PlayerPrefs.HasKey("sfxVol"))
        {
            sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("sfxVol"));
            mixLevels.SetSfxLvl(PlayerPrefs.GetFloat("sfxVol"));
        }
    }

}
