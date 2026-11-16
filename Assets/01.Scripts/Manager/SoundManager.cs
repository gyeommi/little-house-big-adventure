
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip titleBgm;
    [SerializeField] private AudioClip gameBgm;

    [Header("UI SFX")]
    [SerializeField] private AudioClip uiSelectSfx;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    protected override void Awake()
    {
        base.Awake();

        if (instance != this) return;

        isDontDestroy = true;
        DontDestroyOnLoad(gameObject);

        // AudioSource 초기화
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.playOnAwake = false;
            bgmAudioSource.loop = true;
            bgmAudioSource.spatialBlend = 0f;
        }

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.spatialBlend = 0f;
        }

        // 저장된 볼륨 불러오기
        float savedBgm = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float savedSfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        SetBGMVolume(savedBgm);
        SetSFXVolume(savedSfx);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScene":
                PlayBGM(titleBgm);
                break;

            case "Stage1":
            case "Stage2":
                PlayBGM(gameBgm);
                break;
        }
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource != null)
            bgmAudioSource.volume = volume;

        PlayerPrefs.SetFloat(BGM_KEY, volume);
        PlayerPrefs.Save();
    }

    // SFX 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        if (sfxAudioSource != null)
            sfxAudioSource.volume = volume;

        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }

    public float GetBGMVolume()
    {
        return bgmAudioSource != null
            ? bgmAudioSource.volume : 1f;
    }

    public float GetSFXVolume()
    {
        return sfxAudioSource != null
            ? sfxAudioSource.volume : 1f;
    }

    // BGM 재생
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmAudioSource == null)
            return;

        if (bgmAudioSource.isPlaying &&
            bgmAudioSource.clip == clip)
            return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    // UI 선택 효과음 재생
    public void PlayUISelectSFX()
    {
        if (uiSelectSfx == null || sfxAudioSource == null)
            return;

        sfxAudioSource.PlayOneShot(uiSelectSfx);
    }
}
