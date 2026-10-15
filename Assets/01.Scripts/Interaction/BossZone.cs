using UnityEngine;

public class BossZone : MonoBehaviour
{
    [SerializeField] GameObject Boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Boss.SetActive(true);
        }
    }
}
