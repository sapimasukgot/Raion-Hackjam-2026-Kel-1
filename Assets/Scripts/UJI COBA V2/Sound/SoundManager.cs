using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [Tooltip("Musik latar yang diputar terus-menerus selama permainan (loop).")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] [Range(0f, 5f)] private float bgmVolume = 1.5f;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip clickClip;      // MouseClick.mp3
    [SerializeField] private AudioClip pageFlipClip;    // dragon-studio-flipping-book-page...
    [SerializeField] private AudioClip eatingClip;      // Eating.mp3

    [Header("SFX Volume")]
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.8f;

    [Header("Persistensi")]
    [Tooltip("Kalau dicentang, SoundManager (dan BGM yang lagi main) tidak akan hilang saat pindah scene.")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-buat AudioSource kalau belum di-assign manual
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayBGM();
    }

    // =====================================================
    // BGM
    // =====================================================

    public void PlayBGM()
    {
        if (bgmClip == null || bgmSource == null)
            return;

        bgmSource.clip = bgmClip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);

        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }

    // =====================================================
    // SFX - GENERIC
    // =====================================================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    // =====================================================
    // SFX - SHORTCUT KHUSUS
    // =====================================================

    public void PlayClick()
    {
        PlaySFX(clickClip);
    }

    public void PlayPageFlip()
    {
        PlaySFX(pageFlipClip);
    }

    public void PlayEating()
    {
        PlaySFX(eatingClip);
    }
}
