
using UnityEngine;

public class BossStunState : BaseState
{
    private BossController boss;
    private FirstBossController firstBoss;

    private float stunDuration = 3f;
    private float stunEndTime;

    public BossStunState(
        BossController controller,
        StateMachine stateMachine
    ) : base(controller, stateMachine)
    {
        boss = controller;
        firstBoss = controller as FirstBossController;
    }

    public override void Enter()
    {
        // 진행 중인 공격을 중단
        if (firstBoss != null)
        {
            firstBoss.InterruptAttackForStun();
            firstBoss.PlayStunAnimation();
        }

        stunEndTime = Time.time + stunDuration;

        Debug.Log("Boss Stun");
    }

    public override void Update()
    {
        if (Time.time < stunEndTime)
            return;

        Debug.Log("Sweep Attack");

        stateMachine.ChangeState(
            new BossAttackState(
                boss,
                stateMachine,
                forceSweep: true
            )
        );
    }

    public override void FixedUpdate() { }

    public override void Exit() { }
}
