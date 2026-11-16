using UnityEngine;

public class EnemyHealth : BaseCharacterHealth
{
    protected override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }
}