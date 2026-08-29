using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayTransitionUI : MonoBehaviour
{
    public static DayTransitionUI Instance;

    // =====================================================
    // TRANSITION UI
    // =====================================================

    [Header("Transition UI")]
    [SerializeField] private Image blackPanel;
    [SerializeField] private TMP_Text dayText;


    // =====================================================
    // NEXT DAY BUTTON
    // =====================================================

    [Header("Next Day Button")]
    [SerializeField] private Button nextDayButton;
    [SerializeField] private TMP_Text nextDayButtonText;
    [SerializeField] private string normalButtonText = "NEXT DAY";
    [SerializeField] private string blockedButtonText = "Selesaikan Event!";


    // =====================================================
    // UI YANG DITUTUP SAAT NEXT DAY
    // =====================================================

    [Header("UI To Close")]
    [SerializeField] private GameObject fridgeUI;
    [SerializeField] private GameObject bigPaperUI;
    [SerializeField] private GameObject reportUI;


    // =====================================================
    // TIMING
    // =====================================================

    [Header("Timing")]
    [SerializeField] private float fadeToBlackDuration = 0.5f;
    [SerializeField] private float dayTextDuration = 1.2f;
    [SerializeField] private float fadeFromBlackDuration = 0.5f;


    // =====================================================
    // STATE
    // =====================================================

    private bool isTransitioning = false;


    // =====================================================
    // AWAKE
    // =====================================================

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

        // Pastikan layar tidak hitam saat game mulai
        SetBlackAlpha(0f);

        // Pastikan tulisan Day tidak muncul
        if (dayText != null)
        {
            dayText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Update status button setiap frame
        UpdateNextDayButtonState();
    }


    // =====================================================
    // UPDATE NEXT DAY BUTTON STATE
    // =====================================================

    private void UpdateNextDayButtonState()
    {
        if (nextDayButton == null)
            return;

        nextDayButton.interactable = true;

        if (nextDayButtonText != null)
        {
            nextDayButtonText.text = normalButtonText;
        }
    }


    // =====================================================
    // START NEXT DAY
    // =====================================================

    public void StartNextDay()
    {
        if (isTransitioning)
        {
            Debug.Log(
                "Day Transition sedang berjalan."
            );

            return;
        }

        StartCoroutine(
            NextDaySequence()
        );
    }


    // =====================================================
    // NEXT DAY SEQUENCE
    // =====================================================

    private IEnumerator NextDaySequence()
    {
        isTransitioning = true;

        Debug.Log(
            "========== DAY TRANSITION START =========="
        );


        // =================================================
        // 1. TUTUP UI KEPUTUSAN
        // =================================================

        CloseDecisionUI();


        // =================================================
        // 2. FADE KE HITAM
        // =================================================

        yield return StartCoroutine(
            FadeToBlack()
        );


        // =================================================
        // 3. PROSES GAME MANAGER
        // =================================================

        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextDay();
        }
        else
        {
            Debug.LogError(
                "GameManager.Instance tidak ditemukan!"
            );
        }


        // =================================================
        // 3.5. CEK APAKAH GAME ENDING
        // Kalau game ending, EndingManager sudah take over
        // Jangan lanjut ke show day text
        // =================================================

        if (EndingManager.Instance != null && 
            EndingManager.Instance.ShouldEndGame(GameManager.Instance.currentDay))
        {
            Debug.Log("Game ending detected. Stopping day transition.");
            
            isTransitioning = false;
            
            // EndingManager will handle the rest
            yield break;
        }


        // =================================================
        // 3.6. FORCE REFRESH CHARACTER VISUALS
        // =================================================

        RefreshAllCharacterVisuals();


        // =================================================
        // 4. TAMPILKAN DAY BARU
        // =================================================

        if (
            dayText != null &&
            GameManager.Instance != null
        )
        {
            dayText.text =
                "DAY " +
                GameManager.Instance.currentDay;

            dayText.gameObject.SetActive(true);
        }


        // =================================================
        // 5. TUNGGU DAY TEXT
        // =================================================

        yield return new WaitForSeconds(
            dayTextDuration
        );


        // =================================================
        // 6. HILANGKAN DAY TEXT
        // =================================================

        if (dayText != null)
        {
            dayText.gameObject.SetActive(false);
        }


        // =================================================
        // 7. FADE KEMBALI KE ROOM
        // =================================================

        yield return StartCoroutine(
            FadeFromBlack()
        );


        // =================================================
        // 8. SELESAI
        // =================================================

        isTransitioning = false;

        Debug.Log(
            "========== DAY TRANSITION END =========="
        );
    }


    // =====================================================
    // CLOSE DECISION UI
    // =====================================================

    private void CloseDecisionUI()
    {
        // Fridge
        if (fridgeUI != null)
        {
            fridgeUI.SetActive(false);

            Debug.Log(
                "Fridge UI ditutup."
            );
        }

        // Big Paper / Report
        if (bigPaperUI != null)
        {
            bigPaperUI.SetActive(false);

            Debug.Log(
                "Big Paper UI ditutup."
            );
        }

        // Report Panel (alternative name)
        if (reportUI != null)
        {
            reportUI.SetActive(false);

            Debug.Log(
                "Report UI ditutup."
            );
        }
    }


    // =====================================================
    // FADE TO BLACK
    // =====================================================

    private IEnumerator FadeToBlack()
    {
        if (blackPanel == null)
        {
            Debug.LogError(
                "BlackPanel belum di-assign!"
            );

            yield break;
        }

        float time = 0f;

        while (
            time <
            fadeToBlackDuration
        )
        {
            time += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    time /
                    fadeToBlackDuration
                );

            SetBlackAlpha(progress);

            yield return null;
        }

        SetBlackAlpha(1f);
    }


    // =====================================================
    // FADE FROM BLACK
    // =====================================================

    private IEnumerator FadeFromBlack()
    {
        if (blackPanel == null)
        {
            yield break;
        }

        float time = 0f;

        while (
            time <
            fadeFromBlackDuration
        )
        {
            time += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    time /
                    fadeFromBlackDuration
                );

            float alpha =
                1f - progress;

            SetBlackAlpha(alpha);

            yield return null;
        }

        SetBlackAlpha(0f);
    }


    // =====================================================
    // SET BLACK ALPHA
    // =====================================================

    private void SetBlackAlpha(
        float alpha
    )
    {
        if (blackPanel == null)
            return;

        Color color =
            blackPanel.color;

        color.a = alpha;

        blackPanel.color =
            color;
    }


    // =====================================================
    // FORCE CLOSE
    // =====================================================

    public void CloseTransition()
    {
        StopAllCoroutines();

        isTransitioning = false;

        if (dayText != null)
        {
            dayText.gameObject.SetActive(false);
        }

        SetBlackAlpha(0f);
    }


    // =====================================================
    // REFRESH ALL CHARACTER VISUALS
    // =====================================================

    private void RefreshAllCharacterVisuals()
    {
        CharacterVisual[] allVisuals = FindObjectsByType<CharacterVisual>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (CharacterVisual visual in allVisuals)
        {
            if (visual != null)
            {
                visual.ForceRefresh();
            }
        }

        Debug.Log("DayTransition: All character visuals refreshed.");
    }
}