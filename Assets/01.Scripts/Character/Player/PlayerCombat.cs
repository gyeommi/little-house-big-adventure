using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : BaseCharacterCombat
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackRadius = 0.3f;
    [SerializeField] private LayerMask enemyLayer;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Attack();
    }

    protected override void PerformAttack()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * attackRange, Color.red, 1f);

        if (!Physics.SphereCast(origin, attackRadius, direction, out RaycastHit hit, attackRange, enemyLayer))
        {
            Debug.Log("공격 대상 없음");
            return;
        }

        BaseCharacterHealth health =
            hit.collider.GetComponent<BaseCharacterHealth>();

        if (health == null)
        {
            Debug.Log("체력 컴포넌트 없음");
            return;
        }

        health.TakeDamage(AttackDamage);

        Debug.Log($"{hit.collider.name} 공격!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRadius);
    }
}