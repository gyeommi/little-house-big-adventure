using UnityEngine;

public class BaseCharacterHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        currentHealth -= damage;

        Debug.Log($"데미지 {damage}, 현재 체력: {currentHealth}");

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