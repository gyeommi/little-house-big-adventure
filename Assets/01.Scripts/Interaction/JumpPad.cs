using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPower = 20f;

    public void Activate(PlayerController player)
    {
        PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();

        if (stateMachine == null)
            return;

        stateMachine.ForceJump(jumpPower);
    }
}