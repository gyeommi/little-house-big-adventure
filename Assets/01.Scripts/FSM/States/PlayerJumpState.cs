using UnityEngine;

public class PlayerJumpState : BaseState
{
    private PlayerController player;
    private PlayerAnimator playerAnimator;

    private bool hasLeftGround;
    private float? jumpPower;

    // 일반 점프
    public PlayerJumpState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        player = controller;
        playerAnimator = controller.GetComponent<PlayerAnimator>();
        jumpPower = null;
    }

    // 특정 점프 힘을 사용하는 점프
    public PlayerJumpState(PlayerController controller, StateMachine stateMachine, float jumpPower) : base(controller, stateMachine)
    {
        player = controller;
        playerAnimator = controller.GetComponent<PlayerAnimator>();
        this.jumpPower = jumpPower;
    }

    public override void Enter()
    {
        Debug.Log("Jump");

        hasLeftGround = false;

        // JumpPad의 점프 힘이 있으면 해당 값을 사용
        if (jumpPower.HasValue)
        {
            player.JumpPlayer(jumpPower.Value);
        }
        else
        {
            // 일반 점프는 Player의 기본 점프 힘 사용
            player.JumpPlayer();
        }

        playerAnimator.PlayJump();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (!player.IsGrounded)
        {
            hasLeftGround = true;
        }

        if (hasLeftGround && player.IsGrounded)
        {
            if (!player.HasMoveInput)
            {
                stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
            }
            else if (player.IsSprint)
            {
                stateMachine.ChangeState(new PlayerSprintState(player, stateMachine));
            }
            else
            {
                stateMachine.ChangeState(new PlayerMoveState(player, stateMachine));
            }
        }
    }

    public override void FixedUpdate()
    {
        if (player.IsSprint)
        {
            player.MovePlayer(player.GetSprintSpeed());
        }
        else
        {
            player.MovePlayer(player.MoveSpeed);
        }
    }
}