
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushPullObject : MonoBehaviour, IPushPullable
{
    private Rigidbody rb;

    private Transform player;
    private PlayerController playerController;
    private PlayerAnimator playerAnimator;

    private float distance;
    private bool isGrabbed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 플레이어가 오브젝트를 잡는다.
    /// </summary>
    public void Grab(Transform player)
    {
        if (isGrabbed)
            return;

        this.player = player;

        playerController = player.GetComponent<PlayerController>();
        playerAnimator = player.GetComponent<PlayerAnimator>();

        distance = Vector3.Distance(player.position, transform.position);

        isGrabbed = true;
        rb.isKinematic = true;

        // 플레이어 회전 고정
        if (playerController != null)
            playerController.SetRotationLocked(true);

        // Push/Pull 애니메이션 재생
        if (playerAnimator != null)
            playerAnimator.PlayPushPull();
    }

    /// <summary>
    /// 플레이어가 오브젝트를 놓는다.
    /// </summary>
    public void Release()
    {
        if (!isGrabbed)
            return;

        isGrabbed = false;

        // 플레이어 회전 잠금 해제
        if (playerController != null)
            playerController.SetRotationLocked(false);

        // Push/Pull 애니메이션 종료
        if (playerAnimator != null)
            playerAnimator.StopPushPull();

        player = null;
        playerController = null;
        playerAnimator = null;

        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (!isGrabbed || player == null)
            return;

        Vector3 direction = (transform.position - player.position).normalized;

        Vector3 targetPosition = player.position + direction * distance;
        targetPosition.y = transform.position.y;

        rb.MovePosition(targetPosition);
    }

    private void OnDisable()
    {
        Release();
    }
}
