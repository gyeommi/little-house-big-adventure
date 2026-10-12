
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    private BossController bossController;
    private StateMachine stateMachine;

    private void Awake()
    {
        bossController = GetComponent<BossController>();
        stateMachine = new StateMachine();
    }

    private void Start()
    {
        stateMachine.ChangeState(
            new BossIdleState(bossController, stateMachine)
        );
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    /// <summary>
    /// 상태를 변경한다.
    /// </summary>
    public void ChangeState(IState state)
    {
        stateMachine.ChangeState(state);
    }

    /// <summary>
    /// 보스를 스턴 상태로 전환한다.
    /// </summary>
    public void ChangeToStun()
    {
        stateMachine.ChangeState(
            new BossStunState(bossController, stateMachine)
        );
    }

    /// <summary>
    /// 보스를 사망 상태로 전환한다.
    /// </summary>
    public void ChangeToDead()
    {
        stateMachine.ChangeState(
            new BossDeadState(bossController, stateMachine)
        );
    }
}
