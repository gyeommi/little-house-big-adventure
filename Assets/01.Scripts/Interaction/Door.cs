using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"{gameObject.name}과 상호작용했습니다.");
    }
}