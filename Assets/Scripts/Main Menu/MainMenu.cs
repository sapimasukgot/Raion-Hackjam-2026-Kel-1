using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public UIAutoAnimation uiAnimation;
    private CanvasGroup canvasGroup;

    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject pausePanel;

    private MainMenuAudio audioManager;

    [Header("Story")]
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private CanvasGroup storyCanvasGroup;
    [SerializeField] private float storyDisplayDuration = 3f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("UI Animation")]
    [SerializeField] private float uiOpenDuration = 0.4f;
    [SerializeField] private float uiCloseDuration = 0.3f;
    [SerializeField] private float uiOpenScale = 0.8f;
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string[] storyLines = new string[]
    {
        "Keluarga kami terjebak di bunker...",
        "Persediaan terbatas, dan setiap keputusan penting.",
        "Dapatkah kita bertahan?"
    };

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        uiAnimation = GetComponent<UIAutoAnimation>();
        audioManager = GetComponent<MainMenuAudio>();

        // Initialize panels
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        if (storyPanel != null)
            storyPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Get story canvas group from text element if not assigned
        if (storyText != null && storyCanvasGroup == null)
            storyCanvasGroup = storyText.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        uiAnimation.EntranceAnimation();
    }

    // =====================================================
    // PLAY BUTTON
    // =====================================================

    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked - starting story sequence");
        MainMenuAudio.PlayButtonClick();
        StartCoroutine(PlayStorySequence());
    }

    private IEnumerator PlayStorySequence()
    {
        // Show story panel
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
        }

        if (storyCanvasGroup == null && storyPanel != null)
        {
            storyCanvasGroup = storyPanel.GetComponent<CanvasGroup>();
        }

        // Display each story line with fade animation
        foreach (string line in storyLines)
        {
            // Fade in text
            yield return StartCoroutine(FadeInText(line));

            // Display text
            yield return new WaitForSeconds(storyDisplayDuration);

            // Fade out text
            yield return StartCoroutine(FadeOutText());
        }

        // Fade out story panel and load scene
        yield return StartCoroutine(TransitionToGameScene());
    }

    private IEnumerator FadeInText(string newText)
    {
        if (storyText == null || storyCanvasGroup == null)
            yield break;

        storyText.text = newText;
        Debug.Log("Story: " + newText);

        float elapsed = 0f;

        // Fade in
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeInDuration);

            if (storyCanvasGroup != null)
                storyCanvasGroup.alpha = progress;

            yield return null;
        }

        if (storyCanvasGroup != null)
            storyCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutText()
    {
        if (storyCanvasGroup == null)
            yield break;

        float elapsed = 0f;

        // Fade out
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeOutDuration);

            if (storyCanvasGroup != null)
                storyCanvasGroup.alpha = 1f - progress;

            yield return null;
        }

        if (storyCanvasGroup != null)
            storyCanvasGroup.alpha = 0f;
    }

    private IEnumerator TransitionToGameScene()
    {
        // Play transition sound
        MainMenuAudio.PlayTransition();

        // Tunggu sebentar sebelum transition
        yield return new WaitForSeconds(0.5f);

        // Load game scene langsung tanpa fade panel
        Debug.Log("Loading game scene: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    // =====================================================
    // PAUSE
    // =====================================================

    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked");
        MainMenuAudio.PlayButtonClick();

        if (pausePanel == null)
            return;

        pausePanel.SetActive(true);

        RectTransform pauseRect = pausePanel.GetComponent<RectTransform>();
        CanvasGroup pauseCanvasGroup = pausePanel.GetComponent<CanvasGroup>();

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 0f;
        }

        if (pauseRect != null)
        {
            pauseRect.localScale = Vector3.one * uiOpenScale;
        }

        StartCoroutine(AnimateUIOpen(pausePanel, uiOpenDuration));
    }

    public void OnPauseCloseButtonClicked()
    {
        Debug.Log("Pause close button clicked");
        MainMenuAudio.PlayButtonClick();

        if (pausePanel == null)
            return;

        StartCoroutine(AnimateUIClose(pausePanel, uiCloseDuration));
    }

    // =====================================================
    // SETTINGS
    // =====================================================

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked");
        MainMenuAudio.PlayButtonClick();

        if (settingsPanel == null)
            return;

        settingsPanel.SetActive(true);

        RectTransform settingsRect = settingsPanel.GetComponent<RectTransform>();
        CanvasGroup settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();

        if (settingsCanvasGroup != null)
        {
            settingsCanvasGroup.alpha = 0f;
        }

        if (settingsRect != null)
        {
            settingsRect.localScale = Vector3.one * uiOpenScale;
        }

        StartCoroutine(AnimateUIOpen(settingsPanel, uiOpenDuration));
    }

    public void OnSettingsCloseButtonClicked()
    {
        Debug.Log("Settings close button clicked");
        MainMenuAudio.PlayButtonClick();

        if (settingsPanel == null)
            return;

        StartCoroutine(AnimateUIClose(settingsPanel, uiCloseDuration));
    }

    // =====================================================
    // CREDITS
    // =====================================================
    public void SkipCutScene(){
          SceneManager.LoadScene(gameSceneName);
    }
    public void OnCreditsButtonClicked()
    {
        Debug.Log("Credits button clicked");
        MainMenuAudio.PlayButtonClick();

        if (creditsPanel == null)
            return;

        creditsPanel.SetActive(true);

        RectTransform creditsRect = creditsPanel.GetComponent<RectTransform>();
        CanvasGroup creditsCanvasGroup = creditsPanel.GetComponent<CanvasGroup>();

        if (creditsCanvasGroup != null)
        {
            creditsCanvasGroup.alpha = 0f;
        }

        if (creditsRect != null)
        {
            creditsRect.localScale = Vector3.one * uiOpenScale;
        }

        StartCoroutine(AnimateUIOpen(creditsPanel, uiOpenDuration));
    }

    public void OnCreditsCloseButtonClicked()
    {
        Debug.Log("Credits close button clicked");
        MainMenuAudio.PlayButtonClick();

        if (creditsPanel == null)
            return;

        StartCoroutine(AnimateUIClose(creditsPanel, uiCloseDuration));
    }

    // =====================================================
    // FADE ANIMATION HELPER
    // =====================================================

    private IEnumerator AnimateUIOpen(GameObject panel, float duration)
    {
        if (panel == null)
            yield break;

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        CanvasGroup panelCanvasGroup = panel.GetComponent<CanvasGroup>();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            // Fade in
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = progress;

            // Scale up
            if (panelRect != null)
                panelRect.localScale = Vector3.one * Mathf.Lerp(uiOpenScale, 1f, progress);

            yield return null;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 1f;

        if (panelRect != null)
            panelRect.localScale = Vector3.one;
    }

    private IEnumerator AnimateUIClose(GameObject panel, float duration)
    {
        if (panel == null)
            yield break;

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        CanvasGroup panelCanvasGroup = panel.GetComponent<CanvasGroup>();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            // Fade out
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = 1f - progress;

            // Scale down
            if (panelRect != null)
                panelRect.localScale = Vector3.one * Mathf.Lerp(1f, uiOpenScale, progress);

            yield return null;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        if (panelRect != null)
            panelRect.localScale = Vector3.one * uiOpenScale;

        panel.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator FadeCanvasGroupAndClose(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, GameObject panel)
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, startAlpha, endAlpha, duration));

        panel.SetActive(false);
    }

    // =====================================================
    // QUIT
    // =====================================================

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked");
        MainMenuAudio.PlayButtonClick();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
