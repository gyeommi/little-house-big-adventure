using UnityEngine;

public abstract class BaseCharacterCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float attackCooldown = 1f;

    protected float lastAttackTime;

    public int AttackDamage => attackDamage;
    public float AttackCooldown => attackCooldown;

    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

    /// <summary>
    /// 공격을 실행한다.
    /// </summary>
    public virtual void Attack()
    {
        if (!CanAttack)
            return;

        lastAttackTime = Time.time;

        PerformAttack();
    }

    /// <summary>
    /// 실제 공격 방식을 구현한다.
    /// </summary>
    protected abstract void PerformAttack();
}