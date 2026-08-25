using System.Collections;
using UnityEngine;
using TMPro;

public class DayTransitionManager : MonoBehaviour
{
    [Header("Data Hari")]
    public int currentDay = 1;
    public TMP_Text mainDayText;

    [Header("UI Transisi Overlay")]
    public GameObject transitionPanel;       // Drag TransitionPanel ke sini
    public CanvasGroup transitionCanvasGroup; // Drag CanvasGroup TransitionPanel ke sini
    public TMP_Text overlayDayText;
    public GameObject nextDayButton;

    [Header("Pengaturan Durasi & Efek")]
    [Tooltip("Lama waktu dari terang ke hitam (detik)")]
    public float fadeInDuration = 0.8f;

    [Tooltip("Lama layar menahan warna hitam saat ganti hari (detik)")]
    public float stayBlackDuration = 1.2f;

    [Tooltip("Lama waktu dari hitam ke terang kembali (detik)")]
    public float fadeOutDuration = 0.8f;

    private bool isTransitioning = false;

    private void Start()
    {
        UpdateDayTexts();

        // Siapkan panel di awal game: Aktif, tapi bening total (alpha = 0)
        if (transitionPanel != null) transitionPanel.SetActive(true);
        if (transitionCanvasGroup != null)
        {
            transitionCanvasGroup.alpha = 0f;
            transitionCanvasGroup.blocksRaycasts = false;
        }
    }

    // FUNGSI UTAMA (Dipanggil oleh Tombol Next Day)
    public void OnNextDayButtonClicked()
    {
        if (!isTransitioning)
        {
            StartCoroutine(AnimateNextDaySequence());
        }
    }

    private IEnumerator AnimateNextDaySequence()
    {
        isTransitioning = true;

        // 1. Sembunyikan tombol & kunci layar agar tidak bisa diklik sembarangan
        if (nextDayButton != null) nextDayButton.SetActive(false);
        if (transitionPanel != null) transitionPanel.SetActive(true);
        if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = true;

        // 2. EFEK FADE IN (Layar perlahan jadi Hitam Pekat)
        float counter = 0f;
        while (counter < fadeInDuration)
        {
            counter += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(0f, 1f, counter / fadeInDuration);
            yield return null;
        }
        transitionCanvasGroup.alpha = 1f;

        // 3. PROSES GANTI HARI (Dilakukan saat layar sedang Hitam Pekat)
        currentDay++;
        UpdateDayTexts();

        // 4. JEDA LAYAR HITAM (Tahan sebentar biar ada rasa "tidur/ganti hari")
        yield return new WaitForSeconds(stayBlackDuration);

        // 5. EFEK FADE OUT (Layar perlahan kembali Terang)
        counter = 0f;
        while (counter < fadeOutDuration)
        {
            counter += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(1f, 0f, counter / fadeOutDuration);
            yield return null;
        }
        transitionCanvasGroup.alpha = 0f;

        // 6. Buka kembali kuncian raycast & munculkan tombol Next Day
        if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = false;
        if (nextDayButton != null) nextDayButton.SetActive(true);

        isTransitioning = false;
    }

    private void UpdateDayTexts()
    {
        if (mainDayText != null) mainDayText.text = $"HARI {currentDay}";
        if (overlayDayText != null) overlayDayText.text = $"HARI {currentDay}";
    }
}