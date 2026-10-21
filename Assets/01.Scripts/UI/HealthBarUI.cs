using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider hpFill;
    [SerializeField] private BaseCharacterHealth health;

    private void OnEnable()
    {
        if (health == null) return;

        health.OnHealthChanged += UpdateHPBar;

        // 활성화될 때 현재 체력도 즉시 반영
        UpdateHPBar(health.CurrentHealth, health.MaxHealth);
    }

    private void OnDisable()
    {
        if (health == null) return;

        health.OnHealthChanged -= UpdateHPBar;
    }

    private void UpdateHPBar(float current, float max)
    {
        if (hpFill != null)
        {
            hpFill.value = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        }
    }
}
