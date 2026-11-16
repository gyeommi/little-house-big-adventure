using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerController playerController;
    private StateMachine stateMachine;
    private PlayerBookThrow bookThrow;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        bookThrow = GetComponent<PlayerBookThrow>();

        stateMachine = new StateMachine();
    }
    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnJumpInput += HandleJumpInput;
            playerController.OnThrowInput += HandleThrowInput;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnJumpInput -= HandleJumpInput;
            playerController.OnThrowInput -= HandleThrowInput;
        }
    }

    private void Start()
    {
        ChangeState(new PlayerIdleState(playerController, stateMachine));
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void ChangeState(IState state)
    {
        stateMachine.ChangeState(state);
    }

    private void HandleJumpInput()
    {
        if (!playerController.IsGrounded)
            return;

        ChangeState(new PlayerJumpState(playerController, stateMachine));
    }

    private void HandleThrowInput()
    {
        Debug.Log("Attack 이벤트 수신");

        // 책을 들고 있지 않으면 던질 수 없다.
        if (bookThrow == null || !bookThrow.HasBook)
            return;

        // 던지는 중에는 중복 입력을 막는다.
        if (stateMachine.CurrentState is PlayerThrowState)
            return;

        ChangeState(new PlayerThrowState(playerController, stateMachine));
    }

    public void ForceJump(float jumpPower)
    {
        if (playerController == null)
            return;

        ChangeState(new PlayerJumpState(playerController, stateMachine, jumpPower));
    }
}