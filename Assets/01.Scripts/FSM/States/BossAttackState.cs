using UnityEngine;

public class BossAttackState : BaseState
{
    private BossController boss;
    private FirstBossController firstBoss;
    private readonly bool forceSweep;

    public BossAttackState(BossController controller, StateMachine stateMachine, bool forceSweep = false) : base(controller, stateMachine)
    {
        boss = controller;
        firstBoss = controller as FirstBossController;
        this.forceSweep = forceSweep;
    }

    public override void Enter()
    {
        if (firstBoss == null)
        {
            Debug.LogError("FirstBossController를 찾지 못했습니다.");
            return;
        }

        if (!firstBoss.StartBossAttack(forceSweep))
        {
            stateMachine.ChangeState(
                new BossIdleState(boss, stateMachine)
            );
            return;
        }

        switch (firstBoss.CurrentAttackType)
        {
            case FirstBossController.AttackType.BookRain:
                firstBoss.StartBookRain();
                break;

            case FirstBossController.AttackType.Charge:
                firstBoss.StartChargeAttack();
                break;

            case FirstBossController.AttackType.Sweep:
                firstBoss.StartSweepAttack();
                break;
        }
    }

    public override void Update()
    {
        if (firstBoss == null)
            return;

        if (!firstBoss.IsAttacking)
        {
            stateMachine.ChangeState(
                new BossIdleState(boss, stateMachine)
            );
        }
    }

    public override void FixedUpdate() { }

    public override void Exit() { }
}