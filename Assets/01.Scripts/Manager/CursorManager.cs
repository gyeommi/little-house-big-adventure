
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    [Header("Custom Cursor")]
    [SerializeField] private Texture2D cursorTexture;

    [Header("Cursor Settings")]
    [SerializeField] private Vector2 hotspot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private static CursorManager instance;

    private void Awake()
    {
        // 중복 생성 방지
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        ApplyCustomCursor();
    }

    private void ApplyCustomCursor()
    {
        if (cursorTexture == null)
        {
            Debug.LogWarning("커스텀 커서 이미지가 지정되지 않았습니다.", this);
            return;
        }

        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDestroy()
    {
        if (instance != this)
            return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        instance = null;
    }
}
