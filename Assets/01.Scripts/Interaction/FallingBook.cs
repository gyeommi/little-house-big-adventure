using Cysharp.Threading.Tasks;
using UnityEngine;

public class FallingBook : MonoBehaviour, IPoolable
{
    [SerializeField] private GameObject[] bookMeshes;
    [SerializeField] private LayerMask carpetLayer;

    private BookTrapManager manager;

    private Rigidbody rb;
    private bool hasHitPlayer;
    private bool isReturning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(BookTrapManager manager)
    {
        this.manager = manager;
    }

    public void Init()
    {
        hasHitPlayer = false;
        isReturning = false;

        ResetPhysics();
        SetRandomMesh();
    }

    private void SetRandomMesh()
    {
        foreach (GameObject mesh in bookMeshes)
        {
            mesh.SetActive(false);
        }

        int index = Random.Range(0, bookMeshes.Length);

        bookMeshes[index].SetActive(true);
    }

    private void ResetPhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어 충돌
        if (!hasHitPlayer)
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();

            if (player != null)
            {
                hasHitPlayer = true;

                manager.OnPlayerHit(this);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReturning)
            return;

        if ((carpetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            ReturnAfterDelay().Forget();
        }
    }

    private async UniTaskVoid ReturnAfterDelay()
    {
        isReturning = true;

        await UniTask.Delay(3000);

        if (this == null || !gameObject.activeSelf)
            return;

        manager.ReleaseBook(this);
    }

    public void ReturnToPool()
    {
        ResetPhysics();
        hasHitPlayer = false;
        isReturning = false;
    }
}