using UnityEngine;

public class EnemyController : BaseCharacterController
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;

    private Transform target;
    private EnemyCombat combat;

    public Transform Target => target;

    protected override void Awake()
    {
        base.Awake();

        combat = GetComponent<EnemyCombat>();
    }

    protected override void UpdateCharacter()
    {
        base.UpdateCharacter();

        FindPlayer();

        if (target == null)
            return;

        MoveToTarget();
    }

    private void FindPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            playerLayer
        );

        if (colliders.Length == 0)
        {
            target = null;
            return;
        }

        target = GetClosestPlayer(colliders);
    }

    private Transform GetClosestPlayer(Collider[] colliders)
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.SqrMagnitude(
                transform.position - collider.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = collider.transform;
            }
        }

        return closest;
    }

    private void MoveToTarget()
    {
        // 공격 범위 안에 들어오면 이동하지 않음
        if (combat != null && combat.IsInAttackRange)
            return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        Move(direction, MoveSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}