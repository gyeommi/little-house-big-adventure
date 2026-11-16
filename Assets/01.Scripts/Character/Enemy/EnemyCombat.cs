using UnityEngine;

public abstract class EnemyCombat : BaseCharacterCombat
{
    [Header("Attack")]
    [SerializeField] protected float attackRange = 2f;

    protected EnemyController enemyController;

    public bool IsInAttackRange
    {
        get
        {
            if (enemyController == null ||
                enemyController.Target == null)
            {
                return false;
            }

            return Vector3.Distance(
                transform.position,
                enemyController.Target.position
            ) <= attackRange;
        }
    }

    public float AttackRange => attackRange;

    protected virtual void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    protected virtual void Update()
    {
        if (!IsInAttackRange)
            return;

        Attack();
    }

    protected override void PerformAttack()
    {
        Transform target = enemyController.Target;

        if (target == null)
            return;

        PerformAttack(target);
    }

    protected abstract void PerformAttack(Transform target);

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}