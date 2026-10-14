
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookTrapManager : MonoBehaviour
{
    [Header("Carpet Trap")]
    [SerializeField] private float trapDuration = 60f;
    [SerializeField] private BoxCollider fallingAreaCollider;
    [SerializeField] private GameObject carpetBoundary;

    [Header("Boss Book Rain")]
    [SerializeField] private BoxCollider bossFallingAreaCollider;
    [SerializeField] private float bossBookRainDuration = 12f;
    [SerializeField] private float bossFallingInterval = 1.5f;

    [Header("Falling Book")]
    [SerializeField] private BookPool bookPool;
    [SerializeField] private float spawnHeight = 10f;

    [Header("Falling Warning")]
    [SerializeField] private FallingPointPool fallingPointPool;
    [SerializeField] private float warningDuration = 0.7f;

    [Header("Carpet Falling Interval")]
    [SerializeField] private float normalFallingInterval = 7f;
    [SerializeField] private float finalFallingInterval = 4f;
    [SerializeField] private float finalPhaseTime = 20f;

    [Header("Reward Books")]
    [SerializeField] private GameObject rewardBookPrefab;

    [Header("Boss Reward Books")]
    [SerializeField] private GameObject bossRewardBookPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private int bossRewardBookCount = 3;
    [SerializeField] private float rewardSpawnRadius = 2f;
    [SerializeField] private float rewardSpawnHeight = 0.5f;

    private bool isTrapActive;
    private bool isCompleted;
    private bool isBossBookRainActive;

    private Coroutine trapCoroutine;
    private Coroutine bossBookRainCoroutine;

    private readonly List<FallingBook> activeBooks = new();

    // ========================================
    // Carpet Trap
    // ========================================

    public void ActivateTrap()
    {
        if (isCompleted || isTrapActive)
            return;

        isTrapActive = true;

        if (carpetBoundary != null)
            carpetBoundary.SetActive(true);

        trapCoroutine = StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        float startTime = Time.time;

        while (Time.time - startTime < trapDuration)
        {
            float remainingTime =
                trapDuration - (Time.time - startTime);

            float interval =
                remainingTime <= finalPhaseTime
                    ? finalFallingInterval
                    : normalFallingInterval;

            yield return StartCoroutine(
                SpawnFallingBook(fallingAreaCollider)
            );

            if (Time.time - startTime >= trapDuration)
                break;

            yield return new WaitForSeconds(interval);
        }

        CompleteTrap();
    }

    private void CompleteTrap()
    {
        isTrapActive = false;
        isCompleted = true;
        trapCoroutine = null;

        if (carpetBoundary != null)
            carpetBoundary.SetActive(false);

        ReleaseAllBooks();

        if (rewardBookPrefab != null)
            rewardBookPrefab.SetActive(true);

        Debug.Log("책장 함정 완료!");
    }

    private void ResetTrap()
    {
        if (trapCoroutine != null)
        {
            StopCoroutine(trapCoroutine);
            trapCoroutine = null;
        }

        isTrapActive = false;

        if (carpetBoundary != null)
            carpetBoundary.SetActive(false);

        ReleaseAllBooks();
    }

    // ========================================
    // Boss Book Rain
    // ========================================

    public void StartBossBookRain()
    {
        if (isBossBookRainActive)
            return;

        if (bossFallingAreaCollider == null)
        {
            Debug.LogWarning(
                "BookTrapManager: Boss Falling Area Collider가 설정되지 않았습니다."
            );
            return;
        }

        isBossBookRainActive = true;

        bossBookRainCoroutine =
            StartCoroutine(BossBookRainRoutine());
    }

    private IEnumerator BossBookRainRoutine()
    {
        float startTime = Time.time;

        while (Time.time - startTime < bossBookRainDuration)
        {
            yield return StartCoroutine(
                SpawnFallingBook(bossFallingAreaCollider)
            );

            if (Time.time - startTime >= bossBookRainDuration)
                break;

            yield return new WaitForSeconds(bossFallingInterval);
        }

        // 폭격 중 생성된 책을 전부 제거
        ReleaseAllBooks();

        // 보상 책 3개 활성화
        SpawnBossRewardBooks();

        isBossBookRainActive = false;
        bossBookRainCoroutine = null;

        Debug.Log("보스 책 폭격 종료! 보상 책 3개 생성");
    }

    private void SpawnBossRewardBooks()
    {
        if (player == null || bossRewardBookPrefab == null)
        {
            Debug.LogWarning("플레이어 또는 보상 책 프리팹이 설정되지 않았습니다.");
            return;
        }

        for (int i = 0; i < bossRewardBookCount; i++)
        {
            float angle = i * (360f / bossRewardBookCount);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * rewardSpawnRadius;

            Vector3 spawnPosition = player.position + offset;
            spawnPosition.y += rewardSpawnHeight;

            Instantiate(
                bossRewardBookPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }

    // ========================================
    // Shared Falling Book
    // ========================================

    private IEnumerator SpawnFallingBook(BoxCollider areaCollider)
    {
        if (areaCollider == null)
            yield break;

        Vector3 targetPosition = GetRandomFallingPosition(areaCollider);
        Vector3 spawnPosition = targetPosition + Vector3.up * spawnHeight;

        FallingPoint fallingPoint =
            fallingPointPool.Get(targetPosition, Quaternion.identity);

        yield return new WaitForSeconds(warningDuration);

        if (fallingPoint != null)
            fallingPointPool.Return(fallingPoint);

        FallingBook book =
            bookPool.Get(spawnPosition, Random.rotation);

        book.Initialize(this);

        activeBooks.Add(book);
    }

    private Vector3 GetRandomFallingPosition(BoxCollider areaCollider)
    {
        Vector3 center = areaCollider.center;
        Vector3 halfSize = areaCollider.size * 0.5f;

        float randomX = Random.Range(
            center.x - halfSize.x,
            center.x + halfSize.x
        );

        float randomZ = Random.Range(
            center.z - halfSize.z,
            center.z + halfSize.z
        );

        Vector3 localPosition = new Vector3(
            randomX,
            center.y + halfSize.y,
            randomZ
        );

        return areaCollider.transform.TransformPoint(localPosition);
    }

    // ========================================
    // Hit & Pool
    // ========================================

    public void OnPlayerHit(FallingBook book)
    {
        ReleaseBook(book);

        PlayerRespawn respawn =
            FindFirstObjectByType<PlayerRespawn>();

        if (respawn != null)
            respawn.Respawn();

        // 책장 함정 중 맞았을 때만 함정을 초기화한다.
        // 보스의 책 폭격은 여기서 중단하지 않는다.
        if (isTrapActive)
            ResetTrap();
    }

    public void ReleaseBook(FallingBook book)
    {
        if (!activeBooks.Remove(book))
            return;

        bookPool.Return(book);
    }

    private void ReleaseAllBooks()
    {
        for (int i = activeBooks.Count - 1; i >= 0; i--)
        {
            FallingBook book = activeBooks[i];
            activeBooks.RemoveAt(i);
            bookPool.Return(book);
        }
    }

    private void OnDisable()
    {
        if (trapCoroutine != null)
            StopCoroutine(trapCoroutine);

        if (bossBookRainCoroutine != null)
            StopCoroutine(bossBookRainCoroutine);

        trapCoroutine = null;
        bossBookRainCoroutine = null;

        isTrapActive = false;
        isBossBookRainActive = false;

        if (carpetBoundary != null)
            carpetBoundary.SetActive(false);

        ReleaseAllBooks();
    }
}
