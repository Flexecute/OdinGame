using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Image damageImage;
    public float flashSpeed=5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    public Slider healthSlider;
    public PlayerData playerData;

    public Health health { get; private set; }
    private PlayerAnimationController animator;
    private Boolean damaged;

    // Start is called before the first frame update
    void Start()
    {
        // Set the volume
        //AudioListener.volume =1f;

        animator = GetComponentInChildren<PlayerAnimationController>();

        health = GetComponent<Health>();
        health.OnHit += Health_OnHit;
        health.OnDeath += Health_OnDeath;
    }

    void Update()
    {
        if (damaged)
        {
            damageImage.color = flashColour;
        } else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }

    private void Health_OnHit(float damage)
    {
        // Animate the hit if its animatable
        animator.AnimateHit();
        // Flash screen if image has been set
        damaged = true;
        // Update hud
        healthSlider.value -= damage;
    }

    private void Health_OnDeath()
    {
        // Animate the hit if its animatable
        animator.AnimateDeath();

        // Disable all other scripts
        MonoBehaviour[] scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
        // Destroy the object after 2 seconds
        //Destroy(gameObject, 2f);
        Invoke("RestartGame", 3f);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Disables this script along with all other scripts on the game object
    /// </summary>
    public void Disable()
    {
        // Disable all other scripts
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
        // Disable all other scripts
        scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
    }

    /// <summary>
    /// Disables this script along with all other scripts on the game object
    /// </summary>
    public void Enable()
    {
        // Disable all other scripts
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
        // Disable all other scripts
        scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
    }
}
