using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float respawnDuration = 0.2f;
    [SerializeField] private float respawnFOV = 20f;

    private PlayerController player;
    private float originalFOV;

    private void Awake()
    {
        player = GetComponent<PlayerController>();

        if (playerCamera != null)
        {
            originalFOV = playerCamera.fieldOfView;
        }
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        CharacterController controller =
            player.GetComponent<CharacterController>();

        controller.enabled = false;

        // 카메라 줌인
        yield return playerCamera
            .DOFieldOfView(respawnFOV, respawnDuration)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();

        // 플레이어 위치 이동
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;

        controller.enabled = true;

        // 카메라 원래대로
        playerCamera
            .DOFieldOfView(originalFOV, respawnDuration)
            .SetEase(Ease.OutQuad);
    }
}