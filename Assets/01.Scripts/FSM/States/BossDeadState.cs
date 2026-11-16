
using UnityEngine;

public class BossDeadState : BaseState
{
    private BossController boss;
    private FirstBossController firstBoss;

    public BossDeadState(
        BossController controller,
        StateMachine stateMachine
    ) : base(controller, stateMachine)
    {
        boss = controller;
        firstBoss = controller as FirstBossController;
    }

    public override void Enter()
    {
        Debug.Log("보스 사망");

        if (firstBoss != null)
        {
            firstBoss.PlayDeadAnimation();
        }

        // 보스의 행동 업데이트 중지
        boss.enabled = false;
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
