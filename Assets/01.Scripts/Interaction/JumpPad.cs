using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPower = 13f;

    public void Activate(PlayerController player)
    {
        player.Jump(jumpPower);
    }
}