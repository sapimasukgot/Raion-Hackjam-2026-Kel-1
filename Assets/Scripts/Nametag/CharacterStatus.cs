using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CharacterStatusPopup : MonoBehaviour
{
    public static CharacterStatusPopup Instance;

    [Header("Popup")]
    [SerializeField] private GameObject popupPanel;

    [Header("Popup Animation")]
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private RectTransform popupTransform;

    [SerializeField] private float openDuration = 0.2f;
    [SerializeField] private float closeDuration = 0.15f;
    [SerializeField] private float startScale = 0.85f;

    [Header("UI Text")]
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text aliveText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text injuryText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text expeditionText;

    [Header("Close Button")]
    [SerializeField] private Button closeButton;

    private Coroutine animationCoroutine;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePopup);
            closeButton.onClick.AddListener(ClosePopup);
        }

        // Initial state
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0f;

        if (popupTransform != null)
            popupTransform.localScale = Vector3.one;
    }


    // =====================================================
    // SHOW CHARACTER
    // =====================================================

    public void ShowCharacter(CharacterData character)
    {
        if (character == null)
        {
            Debug.LogWarning(
                "CharacterStatusPopup → Character NULL."
            );

            return;
        }

        // =================================================
        // NAMA
        // =================================================

        if (characterNameText != null)
        {
            characterNameText.text =
                character.characterName;
        }


        // =================================================
        // ALIVE
        // =================================================

        if (aliveText != null)
        {
            if (!character.isAlive)
            {
                aliveText.text = "Status: DEAD";
            }
            else if (character.isMissing)
            {
                aliveText.text = "Status: MISSING";
            }
            else
            {
                aliveText.text = "Status: ALIVE";
            }
        }


        // =================================================
        // HUNGER
        // =================================================

        if (hungerText != null)
        {
            if (!character.isAlive ||
                character.isMissing)
            {
                hungerText.text = "Hunger: N/A";
            }
            else
            {
                hungerText.text =
                    "Hunger: " +
                    FormatHunger(character.hungerState);
            }
        }


        // =================================================
        // INJURY
        // =================================================

        if (injuryText != null)
        {
            if (!character.isAlive)
            {
                injuryText.text = "Condition: N/A";
            }
            else if (character.isInjured)
            {
                injuryText.text = "Condition: INJURED";
            }
            else
            {
                injuryText.text = "Condition: HEALTHY";
            }
        }


        // =================================================
        // BODY
        // =================================================

        if (bodyText != null)
        {
            string body = "";

            if (character.missingFinger)
                body += "Missing Finger\n";

            if (character.missingArm)
                body += "Missing Arm\n";

            if (character.missingLeg)
                body += "Missing Leg\n";

            if (body == "")
                body = "No permanent injury";

            bodyText.text =
                "Body:\n" +
                body;
        }


        // =================================================
        // EXPEDITION
        // =================================================

        if (expeditionText != null)
        {
            if (!character.isAlive ||
                character.isMissing ||
                !character.canExpedition)
            {
                expeditionText.text =
                    "Expedition: UNAVAILABLE";
            }
            else
            {
                expeditionText.text =
                    "Expedition: AVAILABLE";
            }
        }


        // =================================================
        // OPEN ANIMATION
        // =================================================

        OpenPopup();
    }


    // =====================================================
    // OPEN POPUP
    // =====================================================

    private void OpenPopup()
    {
        if (popupPanel == null)
            return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        popupPanel.SetActive(true);

        animationCoroutine =
            StartCoroutine(
                AnimateOpen()
            );
    }


    // =====================================================
    // OPEN ANIMATION
    // =====================================================

    private IEnumerator AnimateOpen()
    {
        float timer = 0f;

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0f;

        if (popupTransform != null)
            popupTransform.localScale =
                Vector3.one * startScale;

        while (timer < openDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / openDuration
                );

            // Smooth animation
            float smooth =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = smooth;

            if (popupTransform != null)
            {
                popupTransform.localScale =
                    Vector3.Lerp(
                        Vector3.one * startScale,
                        Vector3.one,
                        smooth
                    );
            }

            yield return null;
        }

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 1f;

        if (popupTransform != null)
            popupTransform.localScale =
                Vector3.one;

        animationCoroutine = null;
    }


    // =====================================================
    // CLOSE POPUP
    // =====================================================

    public void ClosePopup()
    {
        if (popupPanel == null)
            return;

        if (!popupPanel.activeSelf)
            return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine =
            StartCoroutine(
                AnimateClose()
            );
    }


    // =====================================================
    // CLOSE ANIMATION
    // =====================================================

    private IEnumerator AnimateClose()
    {
        float timer = 0f;

        float startAlpha =
            popupCanvasGroup != null
                ? popupCanvasGroup.alpha
                : 1f;

        Vector3 currentScale =
            popupTransform != null
                ? popupTransform.localScale
                : Vector3.one;

        Vector3 targetScale =
            Vector3.one * this.startScale;

        while (timer < closeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / closeDuration
                );

            float smooth =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        0f,
                        smooth
                    );
            }

            if (popupTransform != null)
            {
                popupTransform.localScale =
                    Vector3.Lerp(
                        currentScale,
                        targetScale,
                        smooth
                    );
            }

            yield return null;
        }

        // =================================================
        // FINAL STATE
        // =================================================

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0f;
        }

        if (popupTransform != null)
        {
            popupTransform.localScale =
                Vector3.one * this.startScale;
        }

        popupPanel.SetActive(false);

        animationCoroutine = null;
    }

    // =====================================================
    // HUNGER FORMAT
    // =====================================================

    private string FormatHunger(HungerState hunger)
    {
        switch (hunger)
        {
            case HungerState.Normal:
                return "NORMAL";

            case HungerState.Hungry:
                return "HUNGRY";

            case HungerState.Starving:
                return "STARVING";

            case HungerState.Dead:
                return "DEAD";

            default:
                return hunger.ToString().ToUpper();
        }
    }
}