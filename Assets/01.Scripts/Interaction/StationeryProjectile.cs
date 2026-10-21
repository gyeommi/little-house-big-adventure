
using DG.Tweening;
using UnityEngine;

public class StationeryProjectile : MonoBehaviour
{
    [Header("Platform")]
    [SerializeField] private Collider platformCollider;

    [Header("Flight")]
    [SerializeField] private float flyDuration = 0.5f;

    [Header("Scale")]
    [SerializeField]
    private Vector3 targetScale =
        new Vector3(12f, 10f, 12f);

    [SerializeField] private Ease scaleEase = Ease.OutQuad;

    private bool isLaunched;
    private Tween launchTween;

    private void Awake()
    {
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }
    }

    public void Launch(
        Vector3 targetPosition,
        Quaternion targetRotation)
    {
        if (isLaunched)
            return;

        isLaunched = true;

        launchTween?.Kill();

        Sequence sequence = DOTween.Sequence();

        // 조준 방향으로 직선 이동
        sequence.Join(
            transform.DOMove(
                targetPosition,
                flyDuration
            ).SetEase(Ease.Linear)
        );

        // 비행 중 부드럽게 커짐
        sequence.Join(
            transform.DOScale(
                targetScale,
                flyDuration
            ).SetEase(scaleEase)
        );

        // 목표 방향으로 회전
        sequence.Join(
            transform.DORotateQuaternion(
                targetRotation,
                flyDuration
            )
        );

        // 도착 후 발판으로 활성화
        sequence.OnComplete(() =>
        {
            if (platformCollider != null)
            {
                platformCollider.enabled = true;
            }
        });

        launchTween = sequence;
    }

    private void OnDestroy()
    {
        launchTween?.Kill();
    }
}
