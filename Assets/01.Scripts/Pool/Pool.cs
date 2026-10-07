using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private readonly Queue<GameObject> pool = new();

    private readonly GameObject prefab;
    private readonly Transform parent;

    public int AvailableCount => pool.Count;

    public Pool(GameObject prefab, Transform parent, int size)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < size; i++)
        {
            Create();
        }
    }

    private GameObject Create()
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.SetActive(false);

        pool.Enqueue(go);

        return go;
    }

    public T GetObject<T>(
        Vector3 position,
        Quaternion rotation
    ) where T : Component
    {
        if (pool.Count == 0)
        {
            Create();
        }

        GameObject go = pool.Dequeue();

        go.transform.SetPositionAndRotation(
            position,
            rotation
        );

        go.SetActive(true);

        if (go.TryGetComponent(out IPoolable poolable))
        {
            poolable.Init();
        }

        return go.GetComponent<T>();
    }

    public void ReturnObject(GameObject go)
    {
        if (!go.activeSelf)
            return;

        if (go.TryGetComponent(out IPoolable poolable))
        {
            poolable.ReturnToPool();
        }

        go.SetActive(false);
        pool.Enqueue(go);
    }

    public void Resize(int size)
    {
        while (pool.Count < size)
        {
            Create();
        }

        while (pool.Count > size)
        {
            Object.Destroy(pool.Dequeue());
        }
    }

    public void Dispose()
    {
        Object.Destroy(parent.gameObject);
        pool.Clear();
    }
}