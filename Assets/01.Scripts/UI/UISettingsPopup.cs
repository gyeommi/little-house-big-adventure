
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class UISettingsPopup : MonoBehaviour
{
    [Header("Pause Option")]
    [SerializeField] private bool pauseGameOnOpen = true;

    [Header("Sound Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Convenience Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Menu Return Button")]
    [SerializeField] private GameObject returnToMenuButton;

    private List<Resolution> resolutions = new List<Resolution>();

    private void Start()
    {
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        InitConvenienceSettings();
    }

    private void OnEnable()
    {
        if (SoundManager.instance != null)
        {
            if (bgmSlider != null)
                bgmSlider.value = SoundManager.instance.GetBGMVolume();

            if (sfxSlider != null)
                sfxSlider.value = SoundManager.instance.GetSFXVolume();
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (returnToMenuButton != null)
            returnToMenuButton.SetActive(currentSceneName != "TitleScene");
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Close();
        else
            Open();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (returnToMenuButton != null)
            returnToMenuButton.SetActive(currentSceneName != "TitleScene");

        if (pauseGameOnOpen)
            Time.timeScale = 0f;
    }

    public void Close()
    {
        gameObject.SetActive(false);

        if (pauseGameOnOpen)
            Time.timeScale = 1f;
    }

    public void OnHomeButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (SoundManager.instance != null)
            SoundManager.instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.instance != null)
            SoundManager.instance.SetSFXVolume(value);
    }

    private void InitConvenienceSettings()
    {
        // 해상도 설정
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.ClearOptions();

            resolutions.Clear();

            List<string> options = new List<string>();

            int maxMonitorWidth = Screen.currentResolution.width;
            int maxMonitorHeight = Screen.currentResolution.height;

            var presetResolutions = new (int width, int height, string label)[]
            {
                (1280, 720, "1280 × 720"),
                (1366, 768, "1366 × 768"),
                (1920, 1080, "1920 × 1080"),
                (2560, 1440, "2560 × 1440")
            };

            foreach (var preset in presetResolutions)
            {
                if ((preset.width <= maxMonitorWidth &&
                     preset.height <= maxMonitorHeight) ||
                     resolutions.Count == 0)
                {
                    resolutions.Add(new Resolution
                    {
                        width = preset.width,
                        height = preset.height
                    });

                    options.Add(preset.label);
                }
            }

            int currentResIndex = 0;

            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == Screen.width &&
                    resolutions[i].height == Screen.height)
                {
                    currentResIndex = i;
                    break;
                }
            }

            resolutionDropdown.AddOptions(options);

            int savedResIndex = PlayerPrefs.GetInt(
                "ResolutionIndex", currentResIndex);

            savedResIndex = Mathf.Clamp(
                savedResIndex, 0, resolutions.Count - 1);

            SetResolution(savedResIndex);

            resolutionDropdown.value = savedResIndex;
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        // 전체 화면 설정
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveAllListeners();

            bool isFull = PlayerPrefs.GetInt(
                "IsFullscreen", Screen.fullScreen ? 1 : 0) == 1;

            Screen.fullScreen = isFull;
            fullscreenToggle.isOn = isFull;

            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
    }

    public void OnResolutionChanged(int index)
    {
        SetResolution(index);

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    private void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Count)
        {
            Resolution res = resolutions[index];

            Screen.SetResolution(
                res.width, res.height, Screen.fullScreen);
        }
    }

    public void OnFullscreenChanged(bool isFull)
    {
        Screen.fullScreen = isFull;

        PlayerPrefs.SetInt("IsFullscreen", isFull ? 1 : 0);
        PlayerPrefs.Save();
    }
}
