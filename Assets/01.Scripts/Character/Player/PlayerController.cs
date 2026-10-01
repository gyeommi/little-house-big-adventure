using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController
{
    [Header("Player Movement")]
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private Vector2 moveInput;
    private bool isSprint;

    public bool IsSprint => isSprint;

    protected override void Awake()
    {
        base.Awake();

        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        // 마우스를 화면 중앙에 고정시키고 커서를 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
    }

    /// <summary>
    /// 플레이어 입력을 기반으로 이동한다.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 direction = GetMoveDirection();

        float currentSpeed =
            isSprint ? sprintSpeed : moveSpeed;

        Move(direction, currentSpeed);
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

    /// <summary>
    /// 이동 입력을 받는다.
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// 점프 입력을 받는다.
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Jump();
    }

    /// <summary>
    /// 달리기 입력을 받는다.
    /// </summary>
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
}