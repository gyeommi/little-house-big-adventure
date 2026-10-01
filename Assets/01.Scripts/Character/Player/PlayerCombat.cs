using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : BaseCharacterCombat
{
    [Header("Attack")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackRadius = 0.3f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Attack();
    }

    protected override void PerformAttack()
    {
        if (cameraTransform == null)
            return;

        Vector3 origin = transform.position;

        // 카메라의 좌우 회전만 적용
        Vector3 direction = cameraTransform.forward;
        direction.y = 0f;
        direction.Normalize();

        Debug.DrawRay(
            firePoint.position,
            direction * attackRange,
            Color.red,
            1f
        );
        if (!Physics.SphereCast(
                firePoint.position,
                attackRadius,
                direction,
                out RaycastHit hit,
                attackRange,
                enemyLayer))
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
        if (cameraTransform == null)
            return;

        Vector3 direction = cameraTransform.forward;
        direction.y = 0f;
        direction.Normalize();

        Gizmos.DrawWireSphere(
            firePoint.position + direction * attackRange,
            attackRadius
        );
    }
}