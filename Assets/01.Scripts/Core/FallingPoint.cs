using DG.Tweening;
using UnityEngine;

public class FallingPoint : MonoBehaviour, IPoolable
{
    [SerializeField] private float maxScale = 200f;
    [SerializeField] private float pulseDuration = 0.25f;

    private Vector3 originalScale;
    private Tween pulseTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    /// <summary>
    /// 낙하 경고를 표시한다.
    /// X/Z는 최대 100까지 커지고 Y는 고정된다.
    /// </summary>
    public void Show()
    {
        pulseTween?.Kill();

        gameObject.SetActive(true);

        // 시작 크기
        transform.localScale = new Vector3(0f, originalScale.y, 0f);

        // X/Z만 100까지 확대
        Vector3 targetScale = new Vector3(maxScale, originalScale.y, maxScale);

        pulseTween = transform
            .DOScale(targetScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// 낙하 경고를 종료한다.
    /// </summary>
    public void Hide()
    {
        pulseTween?.Kill();
        pulseTween = null;

        transform.localScale = originalScale;

        gameObject.SetActive(false);
    }

    public void Init()
    {
        Show();
    }

    public void ReturnToPool()
    {
        Hide();
    }
}