
using System.Collections.Generic;
using UnityEngine;

public class StationeryPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private StationeryProjectile prefab;
    [SerializeField] private int poolSize = 10;

    private Pool pool;

    // 현재 활성화된 투사체 관리
    private List<StationeryProjectile> activeProjectiles = new List<StationeryProjectile>();

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
        {
            projectile.SetPool(this);

            if (!activeProjectiles.Contains(projectile))
                activeProjectiles.Add(projectile);
        }

        return projectile;
    }

    // 개별 투사체 회수
    public void Return(StationeryProjectile projectile)
    {
        if (projectile == null || pool == null)
            return;

        if (!activeProjectiles.Contains(projectile))
            return;

        activeProjectiles.Remove(projectile);

        pool.ReturnObject(projectile.gameObject);
    }

    // 활성화된 모든 투사체 회수
    public void ReturnAll()
    {
        if (pool == null)
            return;

        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            StationeryProjectile projectile = activeProjectiles[i];

            if (projectile == null)
                continue;

            pool.ReturnObject(projectile.gameObject);
        }

        activeProjectiles.Clear();
    }
}
