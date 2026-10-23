
using UnityEngine;

public abstract class BossController : BaseCharacterController
{
    [Header("Boss Components")]
    [SerializeField] protected BossHealth bossHealth;
    [SerializeField] protected BossStateMachine bossStateMachine;

    public BossHealth BossHealth => bossHealth;
    public BossStateMachine BossStateMachine => bossStateMachine;

    protected override void Awake()
    {
        base.Awake();

        if (bossHealth == null)
            bossHealth = GetComponent<BossHealth>();

        if (bossStateMachine == null)
            bossStateMachine = GetComponent<BossStateMachine>();
    }

    protected virtual void OnEnable()
    {
        if (bossHealth != null)
            bossHealth.OnDamaged += HandleDamaged;
    }

    protected virtual void OnDisable()
    {
        if (bossHealth != null)
            bossHealth.OnDamaged -= HandleDamaged;
    }

    /// <summary>
    /// 보스가 피해를 받으면 스턴 상태로 전환한다.
    /// </summary>
    private void HandleDamaged()
    {
        if (bossHealth == null || bossStateMachine == null)
            return;

        // 사망한 보스는 스턴시키지 않는다.
        if (bossHealth.CurrentHealth <= 0)
            return;

        bossStateMachine.ChangeToStun();
    }
}
