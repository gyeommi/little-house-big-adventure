using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookTrapManager : MonoBehaviour
{
    [Header("Trap")]
    [SerializeField] private float trapDuration = 60f;

    [Header("Carpet")]
    [SerializeField] private BoxCollider fallingAreaCollider;
    [SerializeField] private GameObject carpetBoundary;

    [Header("Falling Book")]
    [SerializeField] private BookPool bookPool;

    [Header("Falling Warning")]
    [SerializeField] private FallingPointPool fallingPointPool;
    [SerializeField] private float warningDuration = 0.7f;

    [Header("Falling Interval")]
    [SerializeField] private float normalFallingInterval = 7f;
    [SerializeField] private float finalFallingInterval = 4f;
    [SerializeField] private float finalPhaseTime = 20f;

    [Header("Reward Books")]
    [SerializeField] private GameObject rewardBookPrefab;

    private bool isTrapActive;
    private bool isCompleted;

    private Coroutine trapCoroutine;

    private readonly List<FallingBook> activeBooks = new();

    public void ActivateTrap()
    {
        if (isCompleted)
            return;

        if (isTrapActive)
            return;

        isTrapActive = true;

        // Carpet 밖으로 못 나가게 함
        carpetBoundary.SetActive(true);

        trapCoroutine = StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        float startTime = Time.time;

        while (true)
        {
            float elapsedTime = Time.time - startTime;
            float remainingTime = trapDuration - elapsedTime;

            if (remainingTime <= 0f)
                break;

            float fallingInterval =
                remainingTime <= finalPhaseTime ? finalFallingInterval : normalFallingInterval;

            // 책 낙하
            yield return StartCoroutine(SpawnFallingBook());

            elapsedTime = Time.time - startTime;

            if (elapsedTime >= trapDuration)
                break;

            yield return new WaitForSeconds(fallingInterval);
        }

        CompleteTrap();
    }

    private IEnumerator SpawnFallingBook()
    {
        // Carpet 위에서 랜덤 위치 결정
        Vector3 targetPosition = GetRandomCarpetPosition();

        // 책은 Carpet 위 높은 위치에서 생성
        Vector3 spawnPosition = targetPosition + Vector3.up * 10f;

        // 경고 표시
        FallingPoint fallingPoint = fallingPointPool.Get(targetPosition, Quaternion.identity);

        // 경고 시간
        yield return new WaitForSeconds(warningDuration);

        // 경고 제거
        fallingPointPool.Return(fallingPoint);

        // 책 생성
        FallingBook book = bookPool.Get(spawnPosition, Random.rotation);

        book.Initialize(this);

        activeBooks.Add(book);
    }

    private Vector3 GetRandomCarpetPosition()
    {
        Bounds bounds = fallingAreaCollider.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, bounds.max.y, randomZ);
    }

    public void OnPlayerHit(FallingBook book)
    {
        ReleaseBook(book);

        PlayerRespawn respawn =
            FindFirstObjectByType<PlayerRespawn>();

        if (respawn != null)
        {
            respawn.Respawn();
        }

        ResetTrap();
    }

    private void CompleteTrap()
    {
        isTrapActive = false;
        isCompleted = true;

        trapCoroutine = null;

        carpetBoundary.SetActive(false);

        ReleaseAllBooks();

        SpawnRewardBooks();

        Debug.Log("책 피하기 성공!");
    }

    private void ResetTrap()
    {
        if (trapCoroutine != null)
        {
            StopCoroutine(trapCoroutine);
            trapCoroutine = null;
        }

        isTrapActive = false;

        carpetBoundary.SetActive(false);

        ReleaseAllBooks();
    }

    private void SpawnRewardBooks()
    {
        rewardBookPrefab.SetActive(true);
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
}