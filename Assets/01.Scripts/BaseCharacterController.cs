using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BaseCharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 10f;

    [Header("Jump")]
    [SerializeField] protected float gravity = -9.8f;
    [SerializeField] protected float jumpPower = 5f;

    protected CharacterController controller;

    protected Vector2 moveInput;
    protected float gravityVelocity;

    public CharacterController Controller => controller;

    public Vector2 MoveInput => moveInput;

    public bool IsGrounded => controller != null && controller.isGrounded;

    public float MoveSpeed => moveSpeed;
    public float RotateSpeed => rotateSpeed;

    public float Gravity => gravity;
    public float JumpPower => jumpPower;

    public float GravityVelocity => gravityVelocity;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        UpdateCharacter();
    }

    protected virtual void UpdateCharacter()
    {
        UpdateGravity();
    }

    /// <summary>
    /// 이동 입력을 설정한다.
    /// </summary>
    public virtual void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    /// <summary>
    /// 실제 이동을 처리한다.
    /// </summary>
    protected virtual void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Rotate(direction);
        }

        direction *= moveSpeed;
        direction.y = gravityVelocity;

        controller.Move(direction * Time.deltaTime);
    }

    /// <summary>
    /// 이동 방향을 바라보도록 회전한다.
    /// </summary>
    protected virtual void Rotate(Vector3 direction)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// 중력을 처리한다.
    /// </summary>
    protected virtual void UpdateGravity()
    {
        if (controller.isGrounded && gravityVelocity < 0f)
        {
            gravityVelocity = -2f;
        }

        gravityVelocity += gravity * Time.deltaTime;
    }

    /// <summary>
    /// 점프한다.
    /// </summary>
    public virtual void Jump()
    {
        if (!controller.isGrounded)
            return;

        gravityVelocity = jumpPower;
    }

    /// <summary>
    /// 중력 속도를 초기화한다.
    /// </summary>
    protected void ResetGravity()
    {
        gravityVelocity = -2f;
    }

    /// <summary>
    /// 이동을 멈춘다.
    /// </summary>
    protected virtual void StopMovement()
    {
        moveInput = Vector2.zero;
    }

    protected virtual void OnControllerColliderHit(
        ControllerColliderHit hit)
    {
        Rigidbody rb = hit.rigidbody;

        if (rb == null)
            return;

        Vector3 direction = new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        );

        // 필요하면 자식 클래스에서 힘을 적용할 수 있다.
        // rb.AddForce(direction * 5f, ForceMode.Impulse);
    }
}