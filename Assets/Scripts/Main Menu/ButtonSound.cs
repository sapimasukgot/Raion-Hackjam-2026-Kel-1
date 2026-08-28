using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float volume = 0.8f;

    private Button button;
    private static AudioSource sharedAudioSource;

    private void Awake()
    {
        button = GetComponent<Button>();

        // Create shared audio source if doesn't exist
        if (sharedAudioSource == null)
        {
            GameObject audioObject = new GameObject("ButtonSoundPlayer");
            sharedAudioSource = audioObject.AddComponent<AudioSource>();
            sharedAudioSource.playOnAwake = false;
            DontDestroyOnLoad(audioObject);
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null && sharedAudioSource != null)
        {
            sharedAudioSource.PlayOneShot(clickSound, volume);
        }
    }
}
