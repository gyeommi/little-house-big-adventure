
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableBook : MonoBehaviour, IInteractable
{
    [Header("Lifetime")]
    [SerializeField] private float expireTime = 10f;

    [Header("Collision")]
    [SerializeField] private int enemyDamage = 15;
    [SerializeField] private LayerMask enemyLayer;

    private bool isBossReward;
    private bool hasInteracted;

    private Rigidbody rb;
    private Collider[] bookColliders;

    private bool isThrown;
    private bool hasHitTarget;
    private int lifeCycle;

    public bool IsHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bookColliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        IsHeld = false;
        isThrown = false;
        hasHitTarget = false;
    }

    // PlayerInteraction에서 호출
    public void Interact()
    {
        if (IsHeld)
            return;

        PlayerBookThrow playerBookThrow =
            FindFirstObjectByType<PlayerBookThrow>();

        if (playerBookThrow == null)
            return;

        playerBookThrow.PickUpBook(this);
    }

    public void PickUp(Transform holdPoint)
    {
        IsHeld = true;
        isThrown = false;

        if (isBossReward)
        {
            hasInteracted = true;
            lifeCycle++;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        SetCollidersEnabled(false);

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Throw(Vector3 direction, float force)
    {
        IsHeld = false;
        isThrown = true;
        hasHitTarget = false;

        transform.SetParent(null);

        SetCollidersEnabled(true);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 직접 던진 책만 충돌 효과 발생
        if (!isThrown || hasHitTarget)
            return;

        // 책장 문과 충돌
        if (collision.gameObject.CompareTag("BookshelfDoor"))
        {
            hasHitTarget = true;
            OpenBookshelf(collision.gameObject);
            return;
        }

        // Enemy Layer와 충돌
        int enemyLayerIndex = collision.gameObject.layer;

        if ((enemyLayer.value & (1 << enemyLayerIndex)) != 0)
        {
            hasHitTarget = true;

            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(enemyDamage);
            }
        }
    }

    private void OpenBookshelf(GameObject bookshelfDoor)
    {
        bookshelfDoor.SetActive(false);

        Debug.Log("책장 문이 열렸습니다!");
    }

    private async UniTaskVoid ExpireAfterDelay(int currentLifeCycle)
    {
        await UniTask.Delay(
            (int)(expireTime * 1000),
            ignoreTimeScale: true
        );

        if (this == null || !gameObject.activeInHierarchy)
            return;

        if (currentLifeCycle != lifeCycle ||
            IsHeld ||
            hasInteracted ||
            !isBossReward)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider col in bookColliders)
        {
            col.enabled = enabled;
        }
    }

    // 보스 보상 책으로 설정
    public void SetBossReward()
    {
        isBossReward = true;
        hasInteracted = false;

        int currentLifeCycle = ++lifeCycle;
        ExpireAfterDelay(currentLifeCycle).Forget();
    }
}
