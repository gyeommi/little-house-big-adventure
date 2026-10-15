
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

    [Header("Boss Controller")]
    [SerializeField] private FirstBossController firstBossController;

    [Header("Damage Settings")]
    [SerializeField] private int carpetBookDamage = 1;
    [SerializeField] private int bossBookDamage = 20;


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
                SpawnFallingBook(fallingAreaCollider, carpetBookDamage)
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

    public bool StartBossBookRain()
    {
        if (isBossBookRainActive)
        {
            Debug.LogWarning("이미 책 폭격이 진행 중입니다.");
            return false;
        }

        if (bossFallingAreaCollider == null)
        {
            Debug.LogError(
                "BookTrapManager: Boss Falling Area Collider가 설정되지 않았습니다."
            );
            return false;
        }

        if (firstBossController == null)
        {
            Debug.LogError(
                "BookTrapManager: FirstBossController가 연결되지 않았습니다."
            );
            return false;
        }

        isBossBookRainActive = true;

        bossBookRainCoroutine =
            StartCoroutine(BossBookRainRoutine());

        return true;
    }

    private IEnumerator BossBookRainRoutine()
    {
        float startTime = Time.time;

        while (Time.time - startTime < bossBookRainDuration)
        {
            yield return StartCoroutine(
                SpawnFallingBook(bossFallingAreaCollider, bossBookDamage)
            );

            if (Time.time - startTime >= bossBookRainDuration)
                break;

            yield return new WaitForSeconds(bossFallingInterval);
        }

        // 폭격 중 생성된 책 제거
        ReleaseAllBooks();

        // 보상 책 생성
        SpawnBossRewardBooks();

        isBossBookRainActive = false;
        bossBookRainCoroutine = null;

        // 애니메이션 종료
        firstBossController.PlayBookRainAnimation(false);

        Debug.Log("보스 책 폭격 종료! 보상 책 3개 생성");

        // 공격 종료 및 쿨다운 시작
        firstBossController.NotifyAttackFinished();
    }

    private void SpawnBossRewardBooks()
    {
        if (player == null || bossRewardBookPrefab == null)
        {
            Debug.LogWarning("플레이어 또는 보상 책 프리팹이 설정되지 않았습니다.");
            return;
        }

        float[] xOffsets = { -20f, 0f, 20f };

        for (int i = 0; i < bossRewardBookCount; i++)
        {
            Vector3 spawnPosition = new Vector3(
                player.position.x + xOffsets[i],
                player.position.y + rewardSpawnHeight,
                player.position.z + 15f
            );

            // 보상 책 생성
            GameObject rewardBook = Instantiate(
                bossRewardBookPrefab,
                spawnPosition,
                Quaternion.identity
            );

            // 보스 보상 책으로 설정
            ThrowableBook throwableBook =
                rewardBook.GetComponent<ThrowableBook>();

            if (throwableBook != null)
            {
                throwableBook.SetBossReward();
            }
            else
            {
                Debug.LogWarning(
                    "보상 책 프리팹에 ThrowableBook 컴포넌트가 없습니다."
                );
            }
        }
    }

    // ========================================
    // Shared Falling Book
    // ========================================

    private IEnumerator SpawnFallingBook(BoxCollider areaCollider, int damage)
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

        book.Initialize(this, damage);

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

    public void OnPlayerHit(FallingBook book, int damage)
    {
        ReleaseBook(book);

        // 책장 함정에 맞은 경우
        if (isTrapActive)
        {
            ResetTrap();

            PlayerRespawn respawn =
                FindFirstObjectByType<PlayerRespawn>();

            if (respawn != null)
                respawn.Respawn();

            return;
        }

        // 보스 책 폭격에 맞은 경우
        PlayerHealth playerHealth =
            FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
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
