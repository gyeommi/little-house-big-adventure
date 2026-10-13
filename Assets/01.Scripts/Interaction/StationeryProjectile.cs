
using DG.Tweening;
using UnityEngine;

public class StationeryProjectile : MonoBehaviour
{
    [Header("Platform")]
    [SerializeField] private Collider platformCollider;

    [Header("Flight")]
    [SerializeField] private float flyDuration = 0.5f;
    [SerializeField] private float jumpPower = 1f;

    private bool isLaunched;

    private void Awake()
    {
        // 도착하기 전까지 발판 충돌 비활성화
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }
    }

    public void Launch(Transform targetPoint)
    {
        if (isLaunched || targetPoint == null)
            return;

        isLaunched = true;

        // 비행 중 발판 충돌 방지
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        transform.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            transform.DOJump(
                targetPoint.position,
                jumpPower,
                1,
                flyDuration
            ).SetEase(Ease.OutQuad)
        );

        sequence.Join(
            transform.DORotateQuaternion(
                targetPoint.rotation,
                flyDuration
            ).SetEase(Ease.OutQuad)
        );

        sequence.OnComplete(() =>
        {
            // 지정 위치와 회전 적용
            transform.SetPositionAndRotation(
                targetPoint.position,
                targetPoint.rotation
            );

            // 도착한 필기도구를 발판으로 활성화
            if (platformCollider != null)
            {
                platformCollider.enabled = true;
            }
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
