using DG.Tweening;
using UnityEngine;

public class FallingPoint : MonoBehaviour, IPoolable
{
    private float maxScale = 3f;
    private float pulseDuration = 0.25f;

    private Tween pulseTween;

    public void Init()
    {
        pulseTween?.Kill();

        transform.localScale = Vector3.zero;

        pulseTween = transform
            .DOScale(Vector3.one * maxScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ReturnToPool()
    {
        pulseTween?.Kill();
        pulseTween = null;

        transform.localScale = Vector3.zero;
    }
}