using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 10f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        informationPanel.SetActive(false);
    }

    public void ShowInformation(
        Sprite icon,
        string title,
        string content)
    {
        // 기존 종료 코루틴이 있다면 중단
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // UI 정보 변경
        iconImage.sprite = icon;
        titleText.text = title;
        contentText.text = content;

        // UI 표시
        informationPanel.SetActive(true);

        // 10초 후 자동으로 숨기기
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        informationPanel.SetActive(false);
        hideCoroutine = null;
    }
}