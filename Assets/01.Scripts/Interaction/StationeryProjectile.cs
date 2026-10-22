using UnityEngine;

public class StationeryProjectile : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float speed = 25f;
    [SerializeField] private float maxDistance = 50f;

    [Header("Damage")]
    [SerializeField] private int damage = 10;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask bossLayerMask;

    private StationeryPool stationeryPool;
    private Rigidbody rb;

    private Vector3 direction;
    private float traveledDistance;
    private bool isFlying;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                "StationeryProjectile에 Rigidbody가 없습니다.",
                this
            );

            enabled = false;
            return;
        }
    }

    public void SetPool(StationeryPool stationeryPool)
    {
        this.stationeryPool = stationeryPool;
    }

    public void Launch(Vector3 launchDirection)
    {
        if (launchDirection.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning("발사 방향이 올바르지 않습니다.", this);
            return;
        }

        direction = launchDirection.normalized;
        traveledDistance = 0f;
        isFlying = true;

        rb.position = transform.position;
        rb.rotation =
            Quaternion.LookRotation(direction, Vector3.up);

        // 키네마틱 Rigidbody이므로 linearVelocity를 설정하지 않음
        rb.detectCollisions = true;

        Debug.Log(
            $"필기도구 발사! 방향: {direction}, " +
            $"Wall Mask: {wallLayerMask.value}, " +
            $"Boss Mask: {bossLayerMask.value}",
            this
        );
    }

    private void FixedUpdate()
    {
        if (!isFlying)
            return;

        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"충돌 발생: {collision.gameObject.name}", this);

        // 벽에 충돌하면 즉시 정지
        if (((1 << collision.gameObject.layer) & wallLayerMask.value) != 0)
        {
            isFlying = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 벽 충돌 이후 추가 이동 방지
            rb.constraints = RigidbodyConstraints.FreezeAll;

            Debug.Log("벽 충돌! 필기도구 정지");
            return;
        }

        // 보스 충돌 처리
        if (((1 << collision.gameObject.layer) & bossLayerMask.value) != 0)
        {
            BaseCharacterHealth bossHealth = collision.collider.GetComponentInParent<BaseCharacterHealth>();

            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (!isFlying)
            return;

        Debug.Log("ReturnToPool() 호출!");

        isFlying = false;

        if (stationeryPool != null)
        {
            stationeryPool.Return(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        isFlying = false;
    }
}