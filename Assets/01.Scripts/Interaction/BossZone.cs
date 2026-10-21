using UnityEngine;

public class BossZone : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject playerHP;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boss.SetActive(true);
            playerHP.SetActive(true);
        }
    }
}
