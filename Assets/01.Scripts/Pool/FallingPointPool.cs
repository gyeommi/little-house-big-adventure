using UnityEngine;

public class FallingPointPool : MonoBehaviour
{
    [SerializeField] private FallingPoint prefab;
    [SerializeField] private int poolSize = 5;

    private Pool pool;

    private void Awake()
    {
        pool = new Pool(prefab.gameObject, transform, poolSize);
    }

    public FallingPoint Get(Vector3 position, Quaternion rotation)
    {
        return pool.GetObject<FallingPoint>(position, rotation);
    }

    public void Return(FallingPoint point)
    {
        if (point == null)
            return;

        pool.ReturnObject(point.gameObject);
    }
}