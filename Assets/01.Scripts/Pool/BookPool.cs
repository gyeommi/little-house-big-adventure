using UnityEngine;

public class BookPool : MonoBehaviour
{
    [SerializeField] private FallingBook prefab;
    [SerializeField] private int poolSize = 10;

    private Pool pool;

    private void Awake()
    {
        pool = new Pool(prefab.gameObject, transform, poolSize);
    }

    public FallingBook Get(Vector3 position, Quaternion rotation)
    {
        FallingBook book = pool.GetObject<FallingBook>(position, rotation);

        book.SetPool(this);

        return book;
    }

    public void Return(FallingBook book)
    {
        pool.ReturnObject(book.gameObject);
    }
}