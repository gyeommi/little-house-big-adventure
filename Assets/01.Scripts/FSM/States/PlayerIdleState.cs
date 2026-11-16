using UnityEngine;

public class PlayerIdleState : BaseState
{
    private PlayerController player;

    public PlayerIdleState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        player = controller;
    }

    public override void Enter()
    {
        Debug.Log("Idle");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // 공중이라면 Jump
        if (!player.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerJumpState(player, stateMachine));

            return;
        }

        // 이동 입력이 들어오면
        if (player.HasMoveInput)
        {
            if (player.IsSprint)
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
    }
}
