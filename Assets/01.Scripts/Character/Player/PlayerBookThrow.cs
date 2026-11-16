
using UnityEngine;
using DG.Tweening;

public class PlayerBookThrow : MonoBehaviour
{
    [Header("Book")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Vector3 holdScale = new Vector3(3f, 3f, 3f);
    [SerializeField] private Vector3 throwScale = new Vector3(20f, 20f, 20f);

    [Header("Throw")]
    [SerializeField] private float throwForce = 40f;
    [SerializeField] private float throwUpward = 0.8f;
    [SerializeField] private float throwScaleDuration = 0.3f;

    private ThrowableBook heldBook;

    public bool HasBook => heldBook != null;

    public void PickUpBook(ThrowableBook book)
    {
        if (heldBook != null)
            return;

        heldBook = book;
        heldBook.PickUp(holdPoint);
        heldBook.transform.localScale = holdScale;

        Debug.Log("책을 주웠습니다.");
    }

    // Animation Event에서 호출
    public void ReleaseBook()
    {
        Debug.Log("ReleaseBook 호출");

        if (heldBook == null)
            return;

        Vector3 direction = (transform.forward + Vector3.up * throwUpward).normalized;

        ThrowableBook book = heldBook;
        heldBook = null;

        book.Throw(direction, throwForce);

        book.transform.DOScale(throwScale, throwScaleDuration).SetEase(Ease.OutQuad);

        Debug.Log("책을 던졌습니다.");
    }
}
