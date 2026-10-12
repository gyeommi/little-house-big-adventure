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
}