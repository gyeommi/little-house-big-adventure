using UnityEngine;

public class PlayerSprintState : BaseState
{
    private PlayerController player;

    public PlayerSprintState(PlayerController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        player = controller;
    }

    public override void Enter()
    {
        Debug.Log("Sprint");
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

        // Shift를 떼면 Move
        if (!player.IsSprint)
        {
            stateMachine.ChangeState(new PlayerMoveState(player, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        player.MovePlayer(player.GetSprintSpeed());
    }
}
