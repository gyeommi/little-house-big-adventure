using UnityEngine;

public class BookshelfOpen : MonoBehaviour
{
    [SerializeField] private GameObject bookshelfBooks;

    private bool isOpened;

    private void OnCollisionEnter(Collision collision)
    {
        // 던진 책인지 확인
        if (!collision.gameObject.CompareTag("ThrowableBook"))
            return;

        // 이미 열린 책장이면 무시
        if (isOpened)
            return;

        OpenBookshelf();
    }

    private void OpenBookshelf()
    {
        isOpened = true;

        bookshelfBooks.SetActive(false);

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        Debug.Log("책장에 길이 열렸습니다!");
    }
}