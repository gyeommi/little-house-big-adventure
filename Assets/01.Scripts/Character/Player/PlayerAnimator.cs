using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;

    private bool throwAnimationFinished;
    private bool isPushPulling;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Throw = Animator.StringToHash("Throw");
    private static readonly int ButtSlam = Animator.StringToHash("ButtSlam");
    private static readonly int IsPushPulling = Animator.StringToHash("IsPushPulling");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        player.OnButtSlamInput += PlayButtSlam;
    }

    private void OnDisable()
    {
        player.OnButtSlamInput -= PlayButtSlam;
    }

    private void Update()
    {
        UpdateMovementAnimation();
        UpdateGroundAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (isPushPulling)
        {
            animator.SetFloat(Speed, 0f);
            return;
        }

        float speed = 0f;

        if (player.HasMoveInput)
        {
            speed = player.IsSprint ? 1f : 0.5f;
        }

        animator.SetFloat(Speed, speed);
    }

    private void UpdateGroundAnimation()
    {
        animator.SetBool(IsGrounded, player.IsGrounded);
    }

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

    public void PlayButtSlam()
    {
        animator.SetTrigger(ButtSlam);
    }

    /// <summary>
    /// Push/Pull 애니메이션을 시작한다.
    /// </summary>
    public void PlayPushPull()
    {
        isPushPulling = true;
        animator.SetBool(IsPushPulling, true);
    }

    /// <summary>
    /// Push/Pull 애니메이션을 종료한다.
    /// </summary>
    public void StopPushPull()
    {
        isPushPulling = false;
        animator.SetBool(IsPushPulling, false);
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