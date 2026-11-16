
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private Animator animator;

    private static readonly int IsBookRaining = Animator.StringToHash("IsBookRaining");
    private static readonly int Charge = Animator.StringToHash("Charge");
    private static readonly int Sweep = Animator.StringToHash("Sweep");
    private static readonly int Stun = Animator.StringToHash("Stun");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("BossAnimator: Animator가 없습니다.", this);
        }
    }

    public void PlayBookRain(bool isActive)
    {
        if (animator == null) return;
        animator.SetBool(IsBookRaining, isActive);
    }

    public void PlayCharge()
    {
        if (animator == null) return;
        animator.SetTrigger(Charge);
    }

    public void PlaySweep()
    {
        if (animator == null) return;
        animator.SetTrigger(Sweep);
    }

    public void PlayStun()
    {
        if (animator == null) return;
        animator.SetTrigger(Stun);
    }

    public void PlayDead()
    {
        if (animator == null) return;

        animator.SetBool(IsBookRaining, false);
        animator.ResetTrigger(Charge);
        animator.ResetTrigger(Sweep);
        animator.ResetTrigger(Stun);

        animator.SetBool(IsDead, true);
    }
}
