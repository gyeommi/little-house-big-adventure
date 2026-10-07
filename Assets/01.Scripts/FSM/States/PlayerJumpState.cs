using UnityEngine;

public class PlayerJumpState : BaseState
{
    private PlayerController player;
    private PlayerAnimator playerAnimator;

    private bool hasLeftGround;

    public PlayerJumpState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        player = controller;
        playerAnimator = controller.GetComponent<PlayerAnimator>();
    }

    public override void Enter()
    {
        Debug.Log("Jump");

        hasLeftGround = false;

        player.JumpPlayer();

        playerAnimator.PlayJump();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // 먼저 실제로 공중에 떠났는지 확인
        if (!player.IsGrounded)
        {
            hasLeftGround = true;
        }

        // 공중에 올라갔다가 다시 착지했을 때만 상태 변경
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