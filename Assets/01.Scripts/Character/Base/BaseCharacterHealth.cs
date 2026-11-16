
using System;
using UnityEngine;

public class BaseCharacterHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // 체력 변경 이벤트
    public event Action<float, float> OnHealthChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        // 초기 체력 전달
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log($"데미지 {damage}, 현재 체력: {currentHealth}");

        // 체력이 변경되었음을 알림
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        currentHealth = 0;

        Debug.Log($"{gameObject.name} 사망");
    }
}
