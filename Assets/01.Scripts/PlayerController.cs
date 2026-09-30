using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController
{
    [Header("Player Movement")]
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Interaction")]
    [SerializeField] private LayerMask layerMask;

    private bool isSprint;

    public bool IsSprint => isSprint;

    protected override void Awake()
    {
        base.Awake();

        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
                cameraTransform = mainCamera.transform;
        }
    }

    protected override void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        Vector3 direction = GetMoveDirection();

        if (direction.sqrMagnitude > 0.01f)
        {
            Rotate(direction);
        }

        float currentSpeed =
            isSprint ? sprintSpeed : moveSpeed;

        direction *= currentSpeed;
        direction.y = gravityVelocity;

        controller.Move(direction * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && gravityVelocity < 0f)
        {
            ResetGravity();
        }

        gravityVelocity += gravity * Time.deltaTime;
    }

    /// <summary>
    /// 카메라 방향을 기준으로 이동 방향을 계산한다.
    /// </summary>
    private Vector3 GetMoveDirection()
    {
        if (cameraTransform == null)
            return Vector3.zero;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 direction =
            forward * moveInput.y +
            right * moveInput.x;

        return Vector3.ClampMagnitude(direction, 1f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        SetMoveInput(
            context.ReadValue<Vector2>()
        );
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Jump();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprint = true;
        }
        else if (context.canceled)
        {
            isSprint = false;
        }
    }

    protected override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        base.OnControllerColliderHit(hit);

        Rigidbody rb = hit.rigidbody;

        if (rb == null)
            return;

        Vector3 direction = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // 필요하면 밀기 기능 활성화
        // rb.AddForce(direction * 5f, ForceMode.Impulse);
    }
}