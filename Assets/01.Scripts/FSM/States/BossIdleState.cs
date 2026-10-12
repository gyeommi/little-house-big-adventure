
using UnityEngine;

public class BossIdleState : BaseState
{
    private BossController boss;
    private FirstBossController firstBoss;

    public BossIdleState(BossController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        boss = controller;
        firstBoss = controller as FirstBossController;
    }

    public override void Enter()
    {
        Debug.Log("Boss Idle");
    }

    public override void Update()
    {
        if (firstBoss == null)
            return;

        if (firstBoss.CanAttack())
        {
            stateMachine.ChangeState(new BossAttackState(boss, stateMachine));
        }
    }

    public override void FixedUpdate() { }

    public override void Exit() { }
}
