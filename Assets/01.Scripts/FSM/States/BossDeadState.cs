
using UnityEngine;

public class BossDeadState : BaseState
{
    private BossController boss;

    public BossDeadState(BossController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        boss = controller;
    }

    public override void Enter()
    {
        Debug.Log("보스 사망");

        // BaseCharacterController의 이동 및 중력 업데이트 중지
        boss.enabled = false;

        // TODO: 사망 애니메이션, 보상 지급 등
    }

    public override void Update()
    {
    }

    public override void FixedUpdate() 
    {
    }

    public override void Exit() 
    {
    }
}
