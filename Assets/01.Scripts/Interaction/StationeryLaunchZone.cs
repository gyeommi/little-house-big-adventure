using UnityEngine;

public class StationeryLaunchZone : MonoBehaviour
{
    [Header("Stationery Objects")]
    [SerializeField] private StationeryProjectile[] stationeries;

    [Header("Launch Points")]
    [SerializeField] private Transform[] launchPoints;

    private PlayerController playerController;

    private bool playerInside;
    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerController = other.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        playerInside = true;

        // 엉덩방아 입력 이벤트 구독
        playerController.OnButtSlamInput += TryLaunchStationeries;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        UnsubscribePlayerInput();
        playerInside = false;
    }

    private void OnDisable()
    {
        UnsubscribePlayerInput();
    }

    private void UnsubscribePlayerInput()
    {
        if (playerController == null)
            return;

        playerController.OnButtSlamInput -= TryLaunchStationeries;
        playerController = null;
    }

    // 플레이어가 구역 안에서 Ctrl을 누르면 호출
    public void TryLaunchStationeries()
    {
        if (!playerInside || isActivated)
            return;

        if (stationeries == null || launchPoints == null ||
            stationeries.Length != launchPoints.Length)
        {
            Debug.LogError(
                "필기도구와 Launch Point의 개수가 일치하지 않습니다.",
                this
            );
            return;
        }

        isActivated = true;

        for (int i = 0; i < stationeries.Length; i++)
        {
            if (stationeries[i] == null || launchPoints[i] == null)
            {
                Debug.LogWarning(
                    $"필기도구 또는 Point {i}가 연결되지 않았습니다.",
                    this
                );
                continue;
            }

            Transform stationeryTransform = stationeries[i].transform;
            Transform pointTransform = launchPoints[i];

            // Launch Point의 위치와 회전 적용
            stationeryTransform.SetPositionAndRotation(
                pointTransform.position,
                pointTransform.rotation
            );

            // Launch Point의 월드 크기 적용
            stationeryTransform.localScale = pointTransform.lossyScale;

            // 발사 처리
            stationeries[i].Launch(pointTransform);
        }
    }
}