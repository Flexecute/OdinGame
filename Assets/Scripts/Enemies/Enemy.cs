using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IColdable
{
    private PlayerAnimationController animator;
    private NavMeshAgent navMeshAgent;
    private new Collider collider;

    public Health enemyHealth { get; private set; }

    public event Action<Enemy> OnDeath = delegate { };

    private bool dead;
    private Renderer[] meshRenderers;

    private void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimationController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        collider = GetComponent<Collider>();
        enemyHealth = GetComponent<Health>();
        enemyHealth.OnHit += Health_OnHit;
        enemyHealth.OnDeath += Health_OnDeath;
        meshRenderers = GetComponentsInChildren<Renderer>();
        // Factor animation speed based on difficulty to account for enemy speed up
        int difficultyLevel = PlayerData.Instance.difficultyLevel;
        if (difficultyLevel > 0 && animator != null)
            animator.SetAnimationSpeed(difficultyLevel * PlayerData.difficultySpeedFactor);

    }

    private void Health_OnHit(float damage)
    {
        // Animate the hit if its animatable
        if (animator != null)
        {
            animator.AnimateHit();
        }
    }

    private void Health_OnDeath()
    {
        dead = true;
        // Animate the hit if its animatable
        if (animator != null)
        {
            animator.AnimateDeath();
        }
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
        // Disable navMesh if exists
        if (navMeshAgent != null)
            navMeshAgent.isStopped = true;

        // Disable collider
        collider.enabled = false;

        OnDeath(this);
        // Destroy the object after 5 seconds
        Destroy(gameObject, 5f);
    }

    public void TakeColdDamage(float slowAmount, float duration)
    {
        animator.ChangeAnimationSpeed(1 - slowAmount);
        foreach (Renderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = Color.blue;
        }
        Invoke("removeColdEffect", duration);
    }

    private void removeColdEffect()
    {
        animator.ResetAnimationSpeed();
        foreach (Renderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.color = Color.white;
        }
    }


}
