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
    protected float gravityVelocity;

    public CharacterController Controller => controller;

    public bool IsGrounded =>
        controller != null && controller.isGrounded;

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
    /// 지정한 방향으로 캐릭터를 이동시킨다.
    /// </summary>
    protected virtual void Move(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Rotate(direction);
        }

        Vector3 velocity = direction.normalized * speed;
        velocity.y = gravityVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// 이동 방향을 바라보도록 회전한다.
    /// </summary>
    protected virtual void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.01f)
            return;

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
    protected virtual void Jump()
    {
        if (!IsGrounded)
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
}