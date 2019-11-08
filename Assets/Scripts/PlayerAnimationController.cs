using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private float turnSpeed = 5f;

    private Animator animator;
    private ParticleSystem hitEffect;

    private int IDLE_HASH = Animator.StringToHash("Idle");
    private int DEATH_HASH = Animator.StringToHash("Death");
    private int ATTACK_HASH = Animator.StringToHash("Attack");
    private int HIT_HASH = Animator.StringToHash("Hit");
    private int SPEED_HASH = Animator.StringToHash("Speed");
    private int VICTORY_HASH = Animator.StringToHash("Victory");
    private bool attacking;

    private Quaternion desiredRotation;

    public event Action OnAnimationAttack = delegate { };
    public event Action OnAnimationAttackFinish = delegate { };

    private float defaultAnimationSpeed;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        defaultAnimationSpeed = animator.speed;
        // Find the hit effect particle system
        //ParticleSystem[] ps = GetComponentsInParent<ParticleSystem>();
        ParticleSystem[] ps = transform.parent.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].gameObject.CompareTag("HitEffect"))
            {
                hitEffect = ps[i];
                break;
            }
        }

    }

    /// <summary>
    /// Permanently changes the speed
    /// </summary>
    /// <param name="factor"></param>
    internal void SetAnimationSpeed(float factor)
    {
        animator.speed = animator.speed * factor; 
        defaultAnimationSpeed = animator.speed;
    }

    public void AnimateIdle()
    {
        animator.SetBool(IDLE_HASH, true);
    }
    public void AnimateDeath()
    {
        // Disable all other animations
        DisableAllAnimations(animator);
        ResetAnimationSpeed();
        animator.SetBool(DEATH_HASH, true);
    }

    public void AnimateRotation(Quaternion newDirection, bool instant) {
        // Don't rotate if you're attacking
        if (!attacking)
            if (instant)
                transform.rotation = newDirection;
            else
                // 'Slerp' rotation between current value and desired value
                transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * turnSpeed);
    }

    public void AnimateAttack(bool newVal)
    {
        attacking = true;
        // Reset any previous Attacks
        animator.ResetTrigger(ATTACK_HASH);
        animator.SetTrigger(ATTACK_HASH);
    }
    public void AnimateHit()
    {
        // Reset any previous hits
        animator.ResetTrigger(HIT_HASH);
        animator.SetTrigger(HIT_HASH);
        if (hitEffect != null)
            hitEffect.Play();
    }
    public void AnimateVictory()
    {
        // Reset any previous hits
        animator.SetTrigger(VICTORY_HASH);
    }
    public void AnimateSpeed(float newSpeed) {
        animator.SetFloat(SPEED_HASH, newSpeed);
    }


    /// <summary>
    /// Called by the animator when the attack animation strikes
    /// </summary>
    public void AnimationAttack() {
        OnAnimationAttack();
    }
    /// <summary>
    /// Called by the animator when the attack animation is finished
    /// </summary>
    public void AnimationAttackFinish() {
        attacking = false;
        OnAnimationAttackFinish();
    }


    /// <summary>
    /// Multiplies the current animation speed by a factor
    /// </summary>
    public void ChangeAnimationSpeed(float factor)
    {
        animator.speed = animator.speed * factor;
    }
    /// <summary>
    /// Returns animation speed to its default
    /// </summary>
    public void ResetAnimationSpeed()
    {
        animator.speed = defaultAnimationSpeed;
    }

    private void DisableAllAnimations(Animator animator)
    {
        foreach (AnimatorControllerParameter p in animator.parameters) {
            if (p.type == AnimatorControllerParameterType.Bool) {
                animator.SetBool(p.name, false);
            } else if (p.type == AnimatorControllerParameterType.Trigger) {
                animator.ResetTrigger(p.name);
            }
        }
    }
}
