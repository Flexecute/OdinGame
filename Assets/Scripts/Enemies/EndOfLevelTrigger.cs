using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevelTrigger : MonoBehaviour
{
    public float finalEnemyRadius = 20f;
    public List<Enemy> enemies;
    public GameObject particleEffect;
    private Collider playerPickup;
    public int enemyLayer=10;
    public AbilityPowerup rewardPowerup;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private float startTime;
    private float timeTaken;

    // Start is called before the first frame update
    void Awake()
    {
        playerPickup = GetComponent<Collider>();
        playerPickup.enabled = false;
        // Determine list of enemies that have to be killed
        int layerMask;
        layerMask = (1 << enemyLayer);
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, finalEnemyRadius, layerMask);
        for (int i=0;i<enemyColliders.Length;i++)
        {
            Enemy enemy = enemyColliders[i].GetComponentInParent<Enemy>();
            if (enemy != null)
                AddEnemyToKill(enemy);
        }
        // Add the sprite to be shown
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = rewardPowerup.sprite;
        spriteRenderer.enabled = false;

        audioSource = GetComponent<AudioSource>();
        startTime = Time.time;
    }

    private void AddEnemyToKill(Enemy enemy)
    {
        // Attach an observer to fire off whenever an enemy dies
        enemy.OnDeath += enemyDied;
        // Store as list for easy removal
        enemies.Add(enemy);
    }

    public void enemyDied(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count <= 0)
        {
            EndOfLevel();
        }
    }

    private void EndOfLevel()
    {
        GameObject tmp = Instantiate(particleEffect, transform.position, transform.rotation);
        ParticleSystem ps = tmp.GetComponent<ParticleSystem>();
        ps.Play();
        spriteRenderer.enabled = true;
        StartCoroutine("RotateReward");
        Invoke("EnablePickup", 1f);
        if (audioSource != null)
            audioSource.Play();
        // Record how long it took
        timeTaken = Time.time - startTime;
    }

    private void EnablePickup()
    {
        playerPickup.enabled = true;
    }

    System.Collections.IEnumerator RotateReward()
    {
        while (true)
        {
            // Rotate
            gameObject.transform.RotateAround(transform.position, transform.up, 2);
            yield return new WaitForSeconds(.05f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.enabled = false;
            // Prevent re-enabling
            playerPickup.enabled = false;
            // Animate victory if possible
            PlayerAnimationController animator = other.transform.GetComponentInChildren<PlayerAnimationController>();
            if (animator != null)
                animator.AnimateVictory();
            // Add the reward to the player
            PlayerData playerData = PlayerData.Instance;
            //playerData.AddUnusedPowerup(rewardPowerup);
            playerData.CompleteScene(timeTaken);
            Invoke("LoadNextScene", 2f);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("RewardScene");
    }
}
