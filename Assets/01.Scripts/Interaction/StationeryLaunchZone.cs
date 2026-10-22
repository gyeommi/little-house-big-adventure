
using UnityEngine;

public class StationeryLaunchZone : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private StationeryPool stationeryPool;

    [Header("Launch Point")]
    [SerializeField] private Transform launchPoint;

    [Header("Aim Settings")]
    [SerializeField] private Camera aimCamera;

    [Header("Launch Height")]
    [SerializeField] private float launchHeightOffset = 0.5f;

    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController controller =
            other.GetComponentInParent<PlayerController>();

        if (controller == null || playerController == controller)
            return;

        UnsubscribePlayerInput();

        playerController = controller;
        playerController.OnButtSlamInput += TryLaunchStationery;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController controller =
            other.GetComponentInParent<PlayerController>();

        if (controller == null || controller != playerController)
            return;

        UnsubscribePlayerInput();
    }

    private void OnDisable()
    {
        UnsubscribePlayerInput();
    }

    private void UnsubscribePlayerInput()
    {
        if (playerController != null)
        {
            playerController.OnButtSlamInput -= TryLaunchStationery;
        }

        playerController = null;
    }

    private void TryLaunchStationery()
    {
        if (playerController == null)
            return;

        if (stationeryPool == null ||
            launchPoint == null ||
            aimCamera == null)
        {
            Debug.LogWarning(
                "StationeryPool, LaunchPoint 또는 AimCamera를 연결해주세요.",
                this
            );
            return;
        }

        // 화면 중앙을 기준으로 조준 방향 계산
        Ray aimRay = aimCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 direction = aimRay.direction.normalized;

        Vector3 origin = launchPoint.position + Vector3.up * launchHeightOffset;

        Quaternion rotation =
            Quaternion.LookRotation(direction, Vector3.up);

        // 풀에서 필기도구 가져오기
        StationeryProjectile projectile =
            stationeryPool.Get(origin, rotation);

        if (projectile == null)
        {
            Debug.LogWarning(
                "필기도구를 풀에서 가져오지 못했습니다.",
                this
            );
            return;
        }

        // 발사
        projectile.Launch(direction);
    }
}
