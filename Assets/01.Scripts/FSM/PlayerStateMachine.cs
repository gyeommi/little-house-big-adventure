using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerController playerController;
    private StateMachine stateMachine;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        stateMachine = new StateMachine();
    }
    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnJumpInput += HandleJumpInput;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnJumpInput -= HandleJumpInput;
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

    public void ForceJump(float jumpPower)
    {
        if (playerController == null)
            return;

        ChangeState(new PlayerJumpState(playerController, stateMachine, jumpPower));
    }
}