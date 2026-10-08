using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushPullObject : MonoBehaviour, IPushPullable
{
    private Rigidbody rb;

    private Transform player;
    private float distance;

    private bool isGrabbed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform player)
    {
        this.player = player;
        distance = Vector3.Distance(player.position, transform.position);

        isGrabbed = true;

        rb.isKinematic = true;
    }

    public void Release()
    {
        isGrabbed = false;
        player = null;

        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (!isGrabbed || player == null)
            return;

        Vector3 direction =
            (transform.position - player.position).normalized;

        Vector3 targetPosition =
            player.position + direction * distance;

        targetPosition.y = transform.position.y;

        rb.MovePosition(targetPosition);
    }
}