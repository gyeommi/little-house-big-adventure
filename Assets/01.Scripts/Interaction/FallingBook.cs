using Cysharp.Threading.Tasks;
using UnityEngine;

public class FallingBook : MonoBehaviour, IPoolable
{
    [SerializeField] private LayerMask carpetLayer;

    [Header("Fall Settings")]
    [SerializeField] private float gravityScale = 4f;
    [SerializeField] private float maxFallSpeed = 35f;

    private BookTrapManager bookTrapManager;
    private int damage;

    private Rigidbody rb;
    private bool hasHitPlayer;
    private bool isReturning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic)
            return;

        rb.AddForce(
            Physics.gravity * (gravityScale - 1f),
            ForceMode.Acceleration
        );

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                -maxFallSpeed,
                rb.linearVelocity.z
            );
        }
    }

    public void Initialize(BookTrapManager bookTrapManager, int damage)
    {
        this.bookTrapManager = bookTrapManager;
        this.damage = damage;
    }

    public void Init()
    {
        hasHitPlayer = false;
        isReturning = false;

        ResetPhysics();
    }

    private void ResetPhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = true;
        rb.mass = 5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitPlayer)
            return;

        PlayerController player =
            collision.collider.GetComponent<PlayerController>();

        if (player != null)
        {
            hasHitPlayer = true;
            bookTrapManager.OnPlayerHit(this, damage);
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

        bookTrapManager.ReleaseBook(this);
    }

    public void ReturnToPool()
    {
        ResetPhysics();
        hasHitPlayer = false;
        isReturning = false;
    }
}