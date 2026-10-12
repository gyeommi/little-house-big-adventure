
using UnityEngine;

public class BossAttackState : BaseState
{
    private BossController boss;
    private FirstBossController firstBoss;

    private readonly bool forceSweep;

    private float attackDuration;
    private float attackEndTime;

    public BossAttackState(BossController controller, StateMachine stateMachine, bool forceSweep = false) : base(controller, stateMachine)
    {
        boss = controller;
        firstBoss = controller as FirstBossController;
        this.forceSweep = forceSweep;
    }

    public override void Enter()
    {
        if (firstBoss == null)
            return;

        // 공격 선택에 실패하면 공격을 실행하지 않는다.
        if (!firstBoss.StartBossAttack(forceSweep))
        {
            stateMachine.ChangeState(new BossIdleState(boss, stateMachine));
            return;
        }

        switch (firstBoss.CurrentAttackType)
        {
            case FirstBossController.AttackType.BookRain:
                firstBoss.StartBookRain();
                attackDuration = 10f;
                break;

            case FirstBossController.AttackType.Charge:
                firstBoss.StartChargeAttack();
                attackDuration = 2f;
                break;

            case FirstBossController.AttackType.Sweep:
                firstBoss.StartSweepAttack();
                attackDuration = 2f;
                break;
        }

        attackEndTime = Time.time + attackDuration;

        Debug.Log($"Boss Attack: {firstBoss.CurrentAttackType}");
    }

    public override void Update()
    {
        if (firstBoss == null)
            return;

        if (Time.time < attackEndTime)
            return;

        // 공격 종료 시 쿨다운 시작
        firstBoss.NotifyAttackFinished();

        stateMachine.ChangeState(new BossIdleState(boss, stateMachine));
    }

    public override void FixedUpdate() 
    {
    }

    public override void Exit() 
    {
    }
}
