
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossZone : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject playerHP;

    [Header("Boss Settings")]
    [SerializeField] private float bossActivationDelay = 10f;

    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // 중복 실행 방지
        if (isTriggered)
            return;

        isTriggered = true;

        ActivateBossAfterDelay().Forget();
    }

    private async UniTaskVoid ActivateBossAfterDelay()
    {
        await UniTask.Delay(
            (int)(bossActivationDelay * 1000),
            ignoreTimeScale: true,
            cancellationToken: this.GetCancellationTokenOnDestroy()
        );

        playerHP.SetActive(true);
        boss.SetActive(true);
    }
}
