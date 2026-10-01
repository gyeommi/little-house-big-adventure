using UnityEngine;

public class RangedEnemyCombat : EnemyCombat
{
    [Header("Ranged Attack")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask playerLayer;

    protected override void PerformAttack(Transform target)
    {
        if (firePoint == null)
            return;

        Vector3 direction =
            (target.position - firePoint.position).normalized;

        if (!Physics.Raycast(
                firePoint.position,
                direction,
                out RaycastHit hit,
                attackRange,
                playerLayer))
        {
            Debug.Log("공격 대상 없음");
            return;
        }

        BaseCharacterHealth health =
            hit.collider.GetComponent<BaseCharacterHealth>();

        if (health == null)
            return;

        health.TakeDamage(AttackDamage);

        Debug.Log($"{hit.collider.name}에게 원거리 공격!");
    }
}