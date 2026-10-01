using UnityEngine;

public class EnemyCombat : BaseCharacterCombat
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;

    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (enemyController.Target == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            enemyController.Target.position
        );

        if (distance > attackRange)
            return;

        Attack();
    }

    protected override void PerformAttack()
    {
        Transform target = enemyController.Target;

        if (target == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        if (distance > attackRange)
            return;

        BaseCharacterHealth health =
            target.GetComponent<BaseCharacterHealth>();

        if (health == null)
            return;

        health.TakeDamage(AttackDamage);

        Debug.Log($"{target.name}에게 {AttackDamage} 데미지!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}