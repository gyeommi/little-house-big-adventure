using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        transform.DORotate(new Vector3(transform.eulerAngles.x, -209.196f, transform.eulerAngles.z), 0.8f).SetEase(Ease.InOutSine);
        Debug.Log($"{gameObject.name}과 상호작용했습니다.");
    }
}