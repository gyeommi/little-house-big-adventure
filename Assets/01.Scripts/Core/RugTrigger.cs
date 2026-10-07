using UnityEngine;

public class RugTrigger : MonoBehaviour
{
    [SerializeField] private BookTrapManager trapManager;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        trapManager.ActivateTrap();
    }
}