using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{

    public AudioMixer masterMixer;

    public void SetSfxLvl(float level)
    {
        masterMixer.SetFloat("SoundEffectsVol", level);
        PlayerPrefs.SetFloat("sfxVol", level);
    }
    public void SetMusicLvl(float level)
    {
        masterMixer.SetFloat("MusicVol", level);
        PlayerPrefs.SetFloat("musicVol", level);
    }
    public void SetMasterLvl(float level)
    {
        masterMixer.SetFloat("MasterVol", level);
        PlayerPrefs.SetFloat("masterVol", level);
    }
}
