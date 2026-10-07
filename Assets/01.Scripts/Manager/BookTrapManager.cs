using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookTrapManager : MonoBehaviour
{
    [Header("Trap")]
    [SerializeField] private float trapDuration = 60f;

    [Header("Falling Area")]
    [SerializeField] private BookPool bookPool;
    [SerializeField] private Transform fallingArea;
    [SerializeField] private Transform fallingGround;

    [SerializeField] private float fallingAreaWidth = 120f;
    [SerializeField] private float fallingAreaDepth = 200f;

    [Header("Falling Warning")]
    [SerializeField] private FallingPointPool fallingPointPool;
    [SerializeField] private float warningDuration = 0.7f;

    [Header("Falling Interval")]
    [SerializeField] private float minFallingInterval = 1f;
    [SerializeField] private float maxFallingInterval = 1.5f;

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
        trapCoroutine = StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < trapDuration)
        {
            // 책 낙하
            yield return StartCoroutine(SpawnFallingBook());

            // 다음 책까지 대기
            float delay = Random.Range(minFallingInterval, maxFallingInterval);

            yield return new WaitForSeconds(delay);

            elapsedTime += warningDuration + delay;
        }

        CompleteTrap();
    }

    private IEnumerator SpawnFallingBook()
    {
        // 1. 천장에서 랜덤 위치 결정
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // 2. 바닥 위치
        Vector3 fallingPosition = new Vector3(spawnPosition.x, fallingGround.position.y, spawnPosition.z);

        // 3. 경고 표시
        FallingPoint fallingPoint = fallingPointPool.Get(fallingPosition, Quaternion.identity);

        // 4. 경고 시간
        yield return new WaitForSeconds(warningDuration);

        // 5. 경고 제거
        fallingPointPool.Return(fallingPoint);

        // 6. 책 생성
        FallingBook book = bookPool.Get(spawnPosition, Random.rotation);

        book.Initialize(this);

        activeBooks.Add(book);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-fallingAreaWidth * 0.5f, fallingAreaWidth * 0.5f);

        float randomZ = Random.Range(-fallingAreaDepth * 0.5f, fallingAreaDepth * 0.5f);

        return fallingArea.TransformPoint(new Vector3(randomX, 0f, randomZ));
    }

    #region Player Hit

    public void OnPlayerHit(FallingBook book)
    {
        ReleaseBook(book);

        PlayerRespawn respawn = FindFirstObjectByType<PlayerRespawn>();

        if (respawn != null)
        {
            respawn.Respawn();
        }

        ResetTrap();
    }

    #endregion

    #region Trap Complete / Reset

    private void CompleteTrap()
    {
        isTrapActive = false;
        isCompleted = true;

        trapCoroutine = null;

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

        ReleaseAllBooks();
    }

    #endregion

    #region Reward

    private void SpawnRewardBooks()
    {
        rewardBookPrefab.SetActive(true);
    }

    #endregion

    #region Book Pool

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

    #endregion
}