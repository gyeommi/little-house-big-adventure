
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableBook : MonoBehaviour, IInteractable
{
    [Header("Lifetime")]
    [SerializeField] private float expireTime = 10f;

    [Header("Collision")]
    [SerializeField] private int enemyDamage = 15;

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
        if (!isThrown || hasHitTarget)
            return;

        // 책장 문과 충돌
        if (collision.gameObject.CompareTag("BookshelfDoor"))
        {
            hasHitTarget = true;
            OpenBookshelf(collision.gameObject);
            return;
        }

        // Enemy 태그가 붙은 오브젝트 또는 부모를 확인
        if (!HasEnemyTagInParents(collision.collider.transform))
            return;

        // 충돌한 콜라이더부터 부모까지 체력 컴포넌트 탐색
        BaseCharacterHealth enemyHealth =
            collision.collider.GetComponentInParent<BaseCharacterHealth>();

        if (enemyHealth == null)
        {
            Debug.LogWarning(
                $"Enemy 태그는 있지만 BaseCharacterHealth를 찾지 못했습니다: " +
                $"{collision.collider.name}",
                collision.collider
            );
            return;
        }

        hasHitTarget = true;

        Debug.Log(
            $"책 충돌! 대상: {enemyHealth.gameObject.name}, " +
            $"데미지: {enemyDamage}",
            this
        );

        enemyHealth.TakeDamage(enemyDamage);

        gameObject.SetActive(false);
    }

    private bool HasEnemyTagInParents(Transform target)
    {
        while (target != null)
        {
            if (target.CompareTag("Enemy"))
                return true;

            target = target.parent;
        }

        return false;
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

    public void SetBossReward()
    {
        isBossReward = true;
        hasInteracted = false;

        int currentLifeCycle = ++lifeCycle;
        ExpireAfterDelay(currentLifeCycle).Forget();
    }
}
