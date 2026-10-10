using UnityEngine;

public class PlayerThrowState : BaseState
{
    private readonly PlayerController player;
    private PlayerAnimator playerAnimator;
    private PlayerBookThrow bookThrow;

    public PlayerThrowState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        player = controller;
    }

    public override void Enter()
    {
        Debug.Log("Throw");

        playerAnimator = player.GetComponent<PlayerAnimator>();
        bookThrow = player.GetComponent<PlayerBookThrow>();

        if (playerAnimator == null || bookThrow == null)
        {
            Debug.LogError("PlayerAnimator 또는 PlayerBookThrow가 없습니다.");
            return;
        }

        if (!bookThrow.HasBook)
        {
            Debug.LogWarning("던질 책이 없습니다.");
            return;
        }

        playerAnimator.PlayThrow();
    }

    public override void Update()
    {
        if (playerAnimator == null)
            return;

        if (playerAnimator.IsThrowAnimationFinished())
        {
            if (player.HasMoveInput)
            {
                stateMachine.ChangeState(
                    new PlayerMoveState(player, stateMachine));
            }
            else
            {
                stateMachine.ChangeState(
                    new PlayerIdleState(player, stateMachine));
            }
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        if (playerAnimator != null)
        {
            playerAnimator.ResetThrow();
        }
    }
}