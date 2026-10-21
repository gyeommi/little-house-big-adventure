
using UnityEngine;

public class StationeryLaunchZone : MonoBehaviour
{
    [Header("Stationery Objects")]
    [SerializeField] private StationeryProjectile[] stationeries;

    [Header("Launch Point")]
    [SerializeField] private Transform launchPoint;

    [Header("Aim Settings")]
    [SerializeField] private Transform aimCamera;
    [SerializeField] private float launchDistance = 50f;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float wallOffset = 0.05f;

    // 추가: 위쪽 발사 보정
    [SerializeField, Range(0f, 30f)]
    private float aimHeightAngle = 8f;

    private PlayerController playerController;
    private Transform playerTransform;

    private int nextStationeryIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController controller =
            other.GetComponent<PlayerController>();

        if (controller == null || playerController == controller)
            return;

        UnsubscribePlayerInput();

        playerController = controller;
        playerTransform = other.transform;

        playerController.OnButtSlamInput += TryLaunchStationeries;
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerTransform == null)
            return;

        if (other.transform != playerTransform)
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
            playerController.OnButtSlamInput -= TryLaunchStationeries;
        }

        playerController = null;
        playerTransform = null;
    }

    public void TryLaunchStationeries()
    {
        if (playerController == null || playerTransform == null)
            return;

        if (launchPoint == null || aimCamera == null)
        {
            Debug.LogWarning(
                "LaunchPoint 또는 Aim Camera가 연결되지 않았습니다.",
                this
            );
            return;
        }

        Camera cameraComponent = aimCamera.GetComponent<Camera>();

        if (cameraComponent == null)
        {
            Debug.LogWarning(
                "Aim Camera에 Camera 컴포넌트가 없습니다.",
                this
            );
            return;
        }

        if (stationeries == null || stationeries.Length == 0)
        {
            Debug.LogWarning("발사할 필기도구가 없습니다.", this);
            return;
        }

        // 다음 필기도구 찾기
        while (nextStationeryIndex < stationeries.Length &&
               stationeries[nextStationeryIndex] == null)
        {
            nextStationeryIndex++;
        }

        if (nextStationeryIndex >= stationeries.Length)
        {
            Debug.Log("발사할 필기도구를 모두 사용했습니다.", this);
            return;
        }

        StationeryProjectile stationery =
            stationeries[nextStationeryIndex];

        // 1. 발사 시작점
        Vector3 origin = launchPoint.position;

        // 2. 화면 중앙의 실제 카메라 조준 방향
        Ray aimRay = cameraComponent.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        // 카메라 조준 방향
        Vector3 direction = aimRay.direction.normalized;

        // 카메라가 바라보는 방향보다 조금 위쪽으로 보정
        direction = Quaternion.AngleAxis(
            -aimHeightAngle,
            aimCamera.right
        ) * direction;

        direction.Normalize();

        // 3. 발사 시작점에서 조준 방향으로 벽 검사
        Vector3 targetPosition;
        float rayDistance = launchDistance;

        if (Physics.Raycast(
            aimRay,
            out RaycastHit cameraHit,
            launchDistance,
            obstacleLayerMask,
            QueryTriggerInteraction.Ignore))
        {
            // 카메라가 벽을 보고 있다면
            // 발사 시작점에서 그 벽까지의 거리를 고려한다.
            rayDistance = Vector3.Distance(origin, cameraHit.point)
                          + Vector3.Distance(origin, aimRay.origin);
        }

        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit launchHit,
            rayDistance,
            obstacleLayerMask,
            QueryTriggerInteraction.Ignore))
        {
            targetPosition =
                launchHit.point + launchHit.normal * wallOffset;

            Debug.DrawLine(
                origin,
                launchHit.point,
                Color.red,
                5f
            );
        }
        else
        {
            targetPosition = origin + direction * launchDistance;

            Debug.DrawLine(
                origin,
                targetPosition,
                Color.green,
                5f
            );
        }

        // 4. 카메라 조준 방향에 맞춰 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        Vector3 euler = targetRotation.eulerAngles;

        // Y 회전값 제한
        euler.y = Mathf.Clamp(euler.y, 90f, 270f);

        targetRotation = Quaternion.Euler(euler);

        Debug.Log($"목표 회전값: {targetRotation.eulerAngles}");

        // 5. 발사 시작점에 배치
        stationery.transform.position = origin;
        stationery.transform.rotation = targetRotation;

        // 6. 디버깅
        Debug.Log(
            $"카메라 방향: {cameraComponent.transform.forward}\n" +
            $"조준 방향: {direction}\n" +
            $"발사 시작: {origin}\n" +
            $"목표 위치: {targetPosition}\n" +
            $"이동 방향: {(targetPosition - origin).normalized}\n" +
            $"이동 거리: {Vector3.Distance(origin, targetPosition):F2}"
        );

        Debug.DrawRay(
            origin,
            direction * launchDistance,
            Color.cyan,
            5f
        );

        // 7. 발사
        stationery.Launch(targetPosition, targetRotation);

        nextStationeryIndex++;
    }
}
