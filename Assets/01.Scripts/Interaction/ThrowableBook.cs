
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableBook : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private Collider[] bookColliders;

    public bool IsHeld { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bookColliders = GetComponentsInChildren<Collider>();
    }

    // PlayerInteraction에서 호출
    public void Interact()
    {
        if (IsHeld)
            return;

        PlayerBookThrow playerBookThrow = FindFirstObjectByType<PlayerBookThrow>();

        if (playerBookThrow == null)
            return;

        playerBookThrow.PickUpBook(this);
    }

    public void PickUp(Transform holdPoint)
    {
        IsHeld = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        SetCollidersEnabled(false);

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Throw(Vector3 direction, float force)
    {
        IsHeld = false;

        transform.SetParent(null);

        SetCollidersEnabled(true);

        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider col in bookColliders)
        {
            col.enabled = enabled;
        }
    }
}
