using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settings;
    [SerializeField] private ExitPopup exitPopup;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void OnClickNext()
    {
        //일단 테스트용으로 스테이지2로 가도록하기 나중에 데이터 기반으로 수정하기
        LoadScene("Stage2");
    }

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

    public void OnClickHome()
    {
        LoadScene("TitleScene");
    }
}
