
using UnityEngine;

public class BossStunState : BaseState
{
    private BossController boss;

    private float stunDuration = 3f;
    private float stunEndTime;

    public BossStunState(BossController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
        boss = controller;
    }

    public override void Enter()
    {
        stunEndTime = Time.time + stunDuration;

        Debug.Log("Boss Stun");
    }

    public override void Update()
    {
        if (Time.time < stunEndTime)
            return;

        Debug.Log("Sweep Attack");

        stateMachine.ChangeState(new BossAttackState(boss, stateMachine, forceSweep: true));
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit() 
    {
    }
}
