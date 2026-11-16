using UnityEngine;

public class PlayerMoveState : BaseState
{
    private PlayerController player;

    public PlayerMoveState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        player = controller;
    }

    public override void Enter()
    {
        Debug.Log("Move");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // 공중
        if (!player.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerJumpState(player, stateMachine));

            return;
        }

        // 이동하지 않으면 Idle
        if (!player.HasMoveInput)
        {
            stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));

            return;
        }

        // Shift를 누르면 Sprint
        if (player.IsSprint)
        {
            stateMachine.ChangeState(new PlayerSprintState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        player.MovePlayer(player.MoveSpeed);
    }
}
