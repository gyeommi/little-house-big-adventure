using UnityEngine;

public class InformationTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject information;
    [SerializeField] private InformationUI informationUI;

    [Header("Information")]
    [SerializeField] private Sprite icon;
    [SerializeField] private string title;
    [TextArea(3, 6)]
    [SerializeField] private string content;

    [Header("Trigger Settings")]
    [SerializeField] private bool showOnlyOnce = true;

    private bool hasShown;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (showOnlyOnce && hasShown)
            return;

        if (informationUI == null)
        {
            Debug.LogWarning("InformationUI가 연결되지 않았습니다.", this);
            return;
        }

        information.SetActive(true);

        informationUI.ShowInformation(icon, title, content);

        hasShown = true;
    }
}