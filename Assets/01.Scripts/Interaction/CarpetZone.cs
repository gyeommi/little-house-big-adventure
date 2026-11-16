using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class CarpetZone : MonoBehaviour
{
    [SerializeField] private BookTrapManager bookTrapManager;
    [SerializeField] private GameObject time;

    private CinemachineImpulseSource impulseSource;
    private bool activated;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (!other.CompareTag("Player"))
            return;

        activated = true;

        impulseSource.GenerateImpulse();

        time.SetActive(true);

        ActivateTrapAfterDelay().Forget();
    }

    private async UniTaskVoid ActivateTrapAfterDelay()
    {
        await UniTask.Delay(2000);

        bookTrapManager.ActivateTrap();
    }
}