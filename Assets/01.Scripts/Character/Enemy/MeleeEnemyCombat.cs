using UnityEngine;

public class MeleeEnemyCombat : EnemyCombat
{
    protected override void PerformAttack(Transform target)
    {
        BaseCharacterHealth health =
            target.GetComponent<BaseCharacterHealth>();

        if (health == null)
            return;

        health.TakeDamage(AttackDamage);

        Debug.Log($"{target.name}에게 근접 공격!");
    }
}