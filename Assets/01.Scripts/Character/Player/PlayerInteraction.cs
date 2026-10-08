using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private LayerMask interactLayer;

    private IPushPullable grabbedObject;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryInteract();
        }
        else if (context.canceled)
        {
            ReleaseObject();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (!Physics.SphereCast(ray, interactRadius, out RaycastHit hit, interactDistance, interactLayer))
        {
            Debug.Log("상호작용 대상 없음");
            return;
        }

        // 밀기/당기기 가능한 오브젝트
        IPushPullable pushPullable = hit.collider.GetComponent<IPushPullable>();

        if (pushPullable != null)
        {
            grabbedObject = pushPullable;
            grabbedObject.Grab(transform);

            Debug.Log($"{hit.collider.name}을(를) 잡았습니다.");
            return;
        }

        // 일반 상호작용 오브젝트
        IInteractable interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact();

            Debug.Log($"{hit.collider.name}과 상호작용했습니다.");
            return;
        }

        Debug.Log("상호작용할 수 없는 오브젝트");
    }

    private void ReleaseObject()
    {
        if (grabbedObject == null)
            return;

        grabbedObject.Release();
        grabbedObject = null;

        Debug.Log("오브젝트를 놓았습니다.");
    }
}