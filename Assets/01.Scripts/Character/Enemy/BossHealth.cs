
using System;
using UnityEngine;

public class BossHealth : BaseCharacterHealth
{
    [SerializeField] private GameObject trigger;

    // 보스가 피해를 받았을 때 발생하는 이벤트
    public event Action OnDamaged;

    public override void TakeDamage(int damage)
    {
        // 유효하지 않은 피해는 무시
        if (damage <= 0 || CurrentHealth <= 0)
            return;

        // 체력 감소 및 사망 처리
        base.TakeDamage(damage);

        // 사망한 보스는 피해 이벤트를 발행하지 않음
        if (CurrentHealth <= 0)
            return;

        // 피해 이벤트 발행
        OnDamaged?.Invoke();
    }

    protected override void Die()
    {
        base.Die();

        trigger.SetActive(true);

        Destroy(gameObject);
    }
}
