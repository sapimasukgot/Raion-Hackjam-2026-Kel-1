using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private float bgmVolume = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip transitionSFX;
    [SerializeField] private float sfxVolume = 0.8f;

    private static MainMenuAudio instance;

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Setup audio sources if not assigned
        if (bgmAudioSource == null)
            bgmAudioSource = GetComponent<AudioSource>();

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    // =====================================================
    // BACKGROUND MUSIC
    // =====================================================

    public void PlayBackgroundMusic()
    {
        if (bgmAudioSource == null || bgmClip == null)
            return;

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();

        Debug.Log("Background music started");
    }

    public void StopBackgroundMusic()
    {
        if (bgmAudioSource == null)
            return;

        bgmAudioSource.Stop();
        Debug.Log("Background music stopped");
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource == null)
            return;

        bgmAudioSource.volume = Mathf.Clamp01(volume);
    }

    // =====================================================
    // SOUND EFFECTS
    // =====================================================

    public void PlayButtonClickSFX()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayTransitionSFX()
    {
        PlaySFX(transitionSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource == null || clip == null)
            return;

        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.PlayOneShot(clip);

        Debug.Log("SFX played: " + clip.name);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxAudioSource == null)
            return;

        sfxAudioSource.volume = Mathf.Clamp01(volume);
    }

    // =====================================================
    // STATIC ACCESSORS
    // =====================================================

    public static void PlayButtonClick()
    {
        if (instance != null)
            instance.PlayButtonClickSFX();
    }

    public static void PlayTransition()
    {
        if (instance != null)
            instance.PlayTransitionSFX();
    }

    public static void PlayEffect(AudioClip clip)
    {
        if (instance != null)
            instance.PlaySFX(clip);
    }
}
