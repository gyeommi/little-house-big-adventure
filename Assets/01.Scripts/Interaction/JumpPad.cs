using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPower = 20f;

    public void Activate(PlayerController player)
    {
        player.Jump(jumpPower);
    }
}