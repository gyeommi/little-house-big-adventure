using UnityEngine;

public class ToyGun : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject gunPrefab;
    [SerializeField] private Transform weaponPoint;

    public void Interact()
    {
        Debug.Log($"{gameObject.name}과 상호작용했습니다.");

        Instantiate(gunPrefab, weaponPoint.position, weaponPoint.rotation, weaponPoint);
        gameObject.SetActive(false);
    }
}