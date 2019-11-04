using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeMusic : MonoBehaviour
{
    public AudioClip[] levelMusic;

    private AudioSource source;

    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int level = scene.buildIndex;
        if (level <= 0)
            return;
        // First level is preload
        int actualLevel = level - 1; 
        // Modify the music to be the appropriate one for the level
        if (levelMusic.Length > actualLevel)
        {
            source.clip = levelMusic[actualLevel];
            source.Play();
        }
    }
}
