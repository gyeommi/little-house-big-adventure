using UnityEngine;

public class BossHealth : BaseCharacterHealth
{
    protected override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }
}
