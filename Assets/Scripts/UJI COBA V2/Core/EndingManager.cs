using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [Header("Ending UI")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private CanvasGroup endingCanvasGroup;
    [SerializeField] private TMP_Text endingText;

    [Header("Skull/Death Image")]
    [SerializeField] private GameObject skullImagePanel; // Panel untuk gambar tengkorak
    [SerializeField] private CanvasGroup skullCanvasGroup;
    [SerializeField] private float skullDisplayDuration = 5f; // Durasi tampil gambar tengkorak

    [Header("Ending Config")]
    [SerializeField] private int finalDay = 7;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Ending Story")]
    [SerializeField] private string[] endingStoryLines = new string[]
    {
        "7 hari telah berlalu...",
        "Keluarga ini berhasil bertahan.",
        "Namun, berapa banyak yang tersisa?",
        "THE END"
    };

    [Header("Timing")]
    [SerializeField] private float textDisplayDuration = 4f; // Diperlambat dari 3f ke 4f
    [SerializeField] private float fadeInDuration = 2f; // Diperlambat dari 1f ke 2f
    [SerializeField] private float fadeOutDuration = 2f; // Diperlambat dari 1f ke 2f
    [SerializeField] private float pauseBetweenLines = 1.5f; // Jeda antar baris text

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize
        if (endingPanel != null)
            endingPanel.SetActive(false);

        if (skullImagePanel != null)
            skullImagePanel.SetActive(false);

        if (endingCanvasGroup == null && endingPanel != null)
            endingCanvasGroup = endingPanel.GetComponent<CanvasGroup>();

        if (skullCanvasGroup == null && skullImagePanel != null)
            skullCanvasGroup = skullImagePanel.GetComponent<CanvasGroup>();
    }

    // =====================================================
    // CHECK IF ALL CHARACTERS ARE DEAD
    // Dipakai untuk trigger Bad Ending lebih awal,
    // sebelum finalDay tercapai.
    // =====================================================

    public bool IsAllCharactersDead()
    {
        if (GameManager.Instance == null)
            return false;

        GameManager gm = GameManager.Instance;

        bool dadDead = gm.dad == null || !gm.dad.isAlive;
        bool momDead = gm.mom == null || !gm.mom.isAlive;
        bool sonDead = gm.son == null || !gm.son.isAlive;
        bool daughterDead = gm.daughter == null || !gm.daughter.isAlive;

        return dadDead && momDead && sonDead && daughterDead;
    }


    // =====================================================
    // CHECK IF GAME SHOULD END
    // =====================================================

    public bool ShouldEndGame(int currentDay)
    {
        return currentDay > finalDay;
    }

    // =====================================================
    // TRIGGER ENDING
    // =====================================================

    public void TriggerEnding()
    {
        Debug.Log("========================================");
        Debug.Log("GAME ENDING TRIGGERED");
        Debug.Log("========================================");

        StartCoroutine(EndingSequence());
    }

    // =====================================================
    // ENDING SEQUENCE
    // =====================================================

    private IEnumerator EndingSequence()
    {
        // Show ending panel
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }

        if (endingCanvasGroup != null)
        {
            endingCanvasGroup.alpha = 0f;
        }

        // Display each ending line
        foreach (string line in endingStoryLines)
        {
            // Fade in text
            yield return StartCoroutine(FadeInText(line));

            // Display text (diperlambat)
            yield return new WaitForSeconds(textDisplayDuration);

            // Fade out text
            yield return StartCoroutine(FadeOutText());

            // Jeda antar baris
            yield return new WaitForSeconds(pauseBetweenLines);
        }

        // Wait a bit setelah semua text selesai
        yield return new WaitForSeconds(2f);

        // Tampilkan gambar tengkorak sebelum kembali ke main menu
        if (skullImagePanel != null)
        {
            yield return StartCoroutine(ShowSkullImage());
        }

        // Return to main menu
        Debug.Log("Loading Main Menu: " + mainMenuSceneName);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // =====================================================
    // FADE IN TEXT
    // =====================================================

    private IEnumerator FadeInText(string text)
    {
        if (endingText != null)
        {
            endingText.text = text;
        }

        if (endingCanvasGroup == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeInDuration);

            if (endingCanvasGroup != null)
                endingCanvasGroup.alpha = progress;

            yield return null;
        }

        if (endingCanvasGroup != null)
            endingCanvasGroup.alpha = 1f;
    }

    // =====================================================
    // FADE OUT TEXT
    // =====================================================

    private IEnumerator FadeOutText()
    {
        if (endingCanvasGroup == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeOutDuration);

            if (endingCanvasGroup != null)
                endingCanvasGroup.alpha = 1f - progress;

            yield return null;
        }

        if (endingCanvasGroup != null)
            endingCanvasGroup.alpha = 0f;
    }

    // =====================================================
    // SHOW SKULL IMAGE
    // Tampilkan gambar tengkorak sebelum kembali ke menu
    // =====================================================

    private IEnumerator ShowSkullImage()
    {
        // Sembunyikan ending text panel
        if (endingPanel != null)
        {
            endingPanel.SetActive(false);
        }

        // Tampilkan skull panel
        if (skullImagePanel != null)
        {
            skullImagePanel.SetActive(true);
        }

        if (skullCanvasGroup != null)
        {
            skullCanvasGroup.alpha = 0f;
        }

        // Fade in gambar tengkorak
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeInDuration);

            if (skullCanvasGroup != null)
                skullCanvasGroup.alpha = progress;

            yield return null;
        }

        if (skullCanvasGroup != null)
            skullCanvasGroup.alpha = 1f;

        // Tampilkan gambar tengkorak selama beberapa detik
        yield return new WaitForSeconds(skullDisplayDuration);

        // Fade out gambar tengkorak
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeOutDuration);

            if (skullCanvasGroup != null)
                skullCanvasGroup.alpha = 1f - progress;

            yield return null;
        }

        if (skullCanvasGroup != null)
            skullCanvasGroup.alpha = 0f;
    }

    // =====================================================
    // CALCULATE ENDING TYPE (OPTIONAL)
    // Bisa dipake untuk multiple ending
    // =====================================================

    public EndingType CalculateEndingType()
    {
        if (GameManager.Instance == null)
            return EndingType.BadEnding;

        int aliveCount = 0;

        if (GameManager.Instance.dad != null && GameManager.Instance.dad.isAlive)
            aliveCount++;
        if (GameManager.Instance.mom != null && GameManager.Instance.mom.isAlive)
            aliveCount++;
        if (GameManager.Instance.son != null && GameManager.Instance.son.isAlive)
            aliveCount++;
        if (GameManager.Instance.daughter != null && GameManager.Instance.daughter.isAlive)
            aliveCount++;

        if (aliveCount == 4)
            return EndingType.Day66Ending;
        else if (aliveCount >= 2)
            return EndingType.Day66Ending;
        else if (aliveCount == 1)
            return EndingType.Day66Ending;
        else
            return EndingType.BadEnding;
    }

    // =====================================================
    // GET ENDING STORY BASED ON TYPE
    // =====================================================

    public string[] GetEndingStory(EndingType endingType)
    {
        switch (endingType)
        {
             
 

            case EndingType.NormalEnding:
                return new string[]
                {
                    "7 hari telah berlalu...",
                    "Hanya satu yang tersisa.",
                    "Kesepian menghantuinya.",
                    "NORMAL ENDING"
                };

            case EndingType.BadEnding:
                return new string[]
                {
                    "",
                    "Tidak ada yang tersisa.",
                    "Rumah ini sekarang kosong.",
                    "BAD ENDING"
                };

            case EndingType.Day66Ending:
                return new string[]
                {
                    "66 Hari telah berlalu"
                };

            case EndingType.Dikorbankan:
                return new string[]
                {
                    "Semua Orang desa Masuk ke Rumah",
                    "Membawa Seluruh Anggota Keluarga",
                    "...",
                    "Satu Keluarga Telah Menjadi Tumbal"
                };
   

            default:
                return endingStoryLines;
        }
    }

    // =====================================================
    // TRIGGER BAD ENDING (FORCED)
    // Dipakai untuk konsekuensi event yang tidak diselesaikan
    // (misal: Pintu rusak tidak diperbaiki → langsung Bad Ending),
    // TIDAK menghitung aliveCount seperti TriggerEndingWithType().
    // =====================================================

    public void TriggerBadEndingForced()
    {
        endingStoryLines = GetEndingStory(EndingType.Dikorbankan);

        Debug.Log("========================================");
        Debug.Log("GAME ENDING: BadEnding (FORCED by event consequence)");
        Debug.Log("========================================");

        StartCoroutine(EndingSequence());
    }


    // =====================================================
    // TRIGGER ENDING WITH TYPE
    // =====================================================

    public void TriggerEndingWithType()
    {
        EndingType type = CalculateEndingType();
        endingStoryLines = GetEndingStory(type);

        Debug.Log("========================================");
        Debug.Log("GAME ENDING: " + type);
        Debug.Log("========================================");

        StartCoroutine(EndingSequence());
    }
}

public enum EndingType
{
    PerfectEnding,
    GoodEnding,
    NormalEnding,
    BadEnding,
    Day66Ending,
    Dikorbankan
}