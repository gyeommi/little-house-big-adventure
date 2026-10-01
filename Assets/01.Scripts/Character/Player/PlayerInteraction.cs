using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        TryInteract();
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position;

        Ray ray = new Ray(origin, transform.forward);

        Debug.DrawRay(origin, transform.forward * interactDistance, Color.red, 1f);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            Debug.Log("상호작용 대상 없음");
            return;
        }

        IInteractable interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable == null)
        {
            Debug.Log("IInteractable이 없음");
            return;
        }

        Debug.Log($"{hit.collider.name}과 상호작용");

        interactable.Interact();
    }
}