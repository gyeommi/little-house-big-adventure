
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject stageSelection;
    [SerializeField] private GameObject settings;
    [SerializeField] private ExitPopup exitPopup;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void OnClickStart()
    {
        stageSelection.SetActive(true);
    }

    // 스테이지 선택 버튼
    public void OnClickStage1()
    {
        LoadScene("Stage1");
    }

    public void OnClickStage2()
    {
        LoadScene("Stage2");
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickSettings()
    {
        settings.SetActive(true);
    }

    public void OnClickExit()
    {
        if (exitPopup == null)
        {
            exitPopup = FindAnyObjectByType<ExitPopup>(FindObjectsInactive.Include);
        }

        if (exitPopup != null)
        {
            exitPopup.OpenPopup();
        }
    }
}
