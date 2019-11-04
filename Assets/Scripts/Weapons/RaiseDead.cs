using UnityEngine;

public class RaiseDead : MonoBehaviour, IColdable
{
    [SerializeField]
    private float attackRate = 1f;

    [SerializeField]
    private ParticleSystem actionAnimation;

    [SerializeField]
    private SimpleAudioEvent actionSound;
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private GameObject enemyToRaise;
    [SerializeField]
    private int numberToRaise;
    [SerializeField]
    private float randomPosition;
    [SerializeField]
    private float tellTime;

    private AggroDetection aggroDetection;
    private float attackTimer;
    private PlayerAnimationController animator;
    private Transform target;
    private float slowImpact=1f;

    private void Awake()
    {
        aggroDetection = GetComponentInChildren<AggroDetection>();
        aggroDetection.OnAggro += AggroDetection_OnAggro;
        animator = GetComponentInChildren<PlayerAnimationController>();
        // Randomly initialise the attack timer
        attackTimer = Random.Range(0, attackRate);
    }

    public void Update()
    {
        if (target == null)
            return;
        attackTimer += Time.deltaTime * slowImpact;

        if (attackTimer <= attackRate)
            return;

        // WE have a target and have waited enough time to attack
        // Reset attack Timer
        attackTimer = 0;
        // Animate character
        if (animator != null)
            animator.AnimateAttack(true);
        // Raise dead after tell time
        if (tellTime > 0)
            Invoke("RaiseEnemies", tellTime);
        else
            RaiseEnemies();
    }

    private void RaiseEnemies()
    {
        // Animate flash
        if (actionAnimation != null)
            actionAnimation.Play();
        // Play sound
        if (actionSound != null)
            actionSound.Play(audioSource);

        Vector3 initPosition = transform.position + (target.position - transform.position) / 2;
        // Create enemies
        Quaternion newRotation = transform.rotation;
        for (int i=0;i<numberToRaise;i++)
        {
            Vector3 newPosition = new Vector3(initPosition.x + Random.Range(-randomPosition, randomPosition), 0, initPosition.z + Random.Range(-randomPosition, randomPosition));
            GameObject newEnemy = Instantiate(enemyToRaise, newPosition, newRotation);
        }
    }

    private void AggroDetection_OnAggro(Transform newTarget)
    {
        target = newTarget;
    }
    public void TakeColdDamage(float slowAmount, float duration)
    {
        slowImpact = (1 - slowAmount);
        Invoke("removeColdEffect", duration);
    }

    private void removeColdEffect()
    {
        slowImpact = 1f;
    }

}
