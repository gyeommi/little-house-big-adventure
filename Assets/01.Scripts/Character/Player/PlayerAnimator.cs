using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;

    private bool throwAnimationFinished;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Throw = Animator.StringToHash("Throw");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateMovementAnimation();
        UpdateGroundAnimation();
    }

    /// <summary>
    /// 이동 애니메이션을 업데이트한다.
    /// Idle = 0
    /// Walk = 0.5
    /// Run = 1
    /// </summary>
    private void UpdateMovementAnimation()
    {
        float speed = 0f;

        if (player.HasMoveInput)
        {
            speed = player.IsSprint ? 1f : 0.5f;
        }

        animator.SetFloat(Speed, speed);
    }

    /// <summary>
    /// 지면 상태를 업데이트한다.
    /// </summary>
    private void UpdateGroundAnimation()
    {
        animator.SetBool(IsGrounded, player.IsGrounded);
    }

    /// <summary>
    /// 점프 애니메이션을 재생한다.
    /// </summary>
    public void PlayJump()
    {
        animator.SetTrigger(Jump);
    }

    public void PlayThrow()
    {
        throwAnimationFinished = false;

        animator.ResetTrigger(Throw);
        animator.SetTrigger(Throw);
    }

    public void FinishThrowAnimation()
    {
        throwAnimationFinished = true;
        Debug.Log("던지기 애니메이션 종료 이벤트");
    }

    public bool IsThrowAnimationFinished()
    {
        return throwAnimationFinished;
    }

    public void ResetThrow()
    {
        animator.ResetTrigger(Throw);
        throwAnimationFinished = false;
    }
}