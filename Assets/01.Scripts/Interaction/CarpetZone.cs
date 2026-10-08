using UnityEngine;

public class CarpetZone : MonoBehaviour
{
    [SerializeField] private BookTrapManager bookTrapManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        bookTrapManager.ActivateTrap();
    }
}