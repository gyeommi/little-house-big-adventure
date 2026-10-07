using UnityEngine;

public class FallingBook : MonoBehaviour, IPoolable
{
    [Header("Book Mesh")]
    [SerializeField] private GameObject[] bookMeshes;

    private string groundTag = "Carpet";

    private BookPool pool;
    private BookTrapManager manager;

    private Rigidbody rb;
    private bool hasHitPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetPool(BookPool pool)
    {
        this.pool = pool;
    }

    public void Initialize(BookTrapManager manager)
    {
        this.manager = manager;
    }

    public void Init()
    {
        hasHitPlayer = false;

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

        // 카펫 충돌
        if (collision.gameObject.CompareTag(groundTag))
        {
            manager.ReleaseBook(this);
        }
    }

    public void ReturnToPool()
    {
        ResetPhysics();
    }
}