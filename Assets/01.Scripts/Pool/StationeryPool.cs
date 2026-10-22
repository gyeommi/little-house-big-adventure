
using UnityEngine;

public class StationeryPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private StationeryProjectile prefab;
    [SerializeField] private int poolSize = 10;

    private Pool pool;

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError(
                "StationeryPool: Prefab이 연결되지 않았습니다.",
                this
            );
            return;
        }

        pool = new Pool(
            prefab.gameObject,
            transform,
            poolSize
        );
    }

    public StationeryProjectile Get(
        Vector3 position,
        Quaternion rotation)
    {
        if (pool == null)
            return null;

        StationeryProjectile projectile =
            pool.GetObject<StationeryProjectile>(
                position,
                rotation
            );

        if (projectile != null)
            projectile.SetPool(this);

        return projectile;
    }

    public void Return(StationeryProjectile projectile)
    {
        if (projectile == null || pool == null)
            return;

        pool.ReturnObject(projectile.gameObject);
    }
}
