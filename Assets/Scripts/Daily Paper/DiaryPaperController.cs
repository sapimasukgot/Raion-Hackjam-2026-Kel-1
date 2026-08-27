using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DiaryPaperUIController : MonoBehaviour
{
    [Header("Animasi UI")]
    public UIAutoAnimation uiAnimation;

    private CanvasGroup canvasGroup;
    private Coroutine closeCoroutine;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (uiAnimation == null)
        {
            uiAnimation = GetComponent<UIAutoAnimation>();
        }
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        HideInstant();
    }


    // =====================================================
    // OPEN UI
    // =====================================================

    public void OpenUI()
    {
        // Batalkan proses close kalau masih berjalan
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        // Pastikan GameObject tetap aktif
        gameObject.SetActive(true);

        // Aktifkan UI
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Debug.Log("📄 Diary Paper besar dibuka.");

        // Jalankan animasi masuk
        if (uiAnimation != null)
        {
            uiAnimation.EntranceAnimation();
        }
    }


    // =====================================================
    // CLOSE UI
    // =====================================================

    public void CloseUI()
    {
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }

        closeCoroutine =
            StartCoroutine(CloseUIRoutine());
    }


    // =====================================================
    // CLOSE ROUTINE
    // =====================================================

    private IEnumerator CloseUIRoutine()
    {
        // Langsung matikan interaksi
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Jalankan animasi keluar
        if (uiAnimation != null)
        {
            uiAnimation.ExitAnimation();

            float waitTime =
                CalculateExitDuration();

            yield return new WaitForSeconds(waitTime);
        }

        HideInstant();

        closeCoroutine = null;

        Debug.Log("📄 Diary Paper besar ditutup.");
    }


    // =====================================================
    // HIDE INSTANT
    // =====================================================

    private void HideInstant()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    // =====================================================
    // EXIT ANIMATION DURATION
    // =====================================================

    private float CalculateExitDuration()
    {
        if (
            uiAnimation == null ||
            uiAnimation.animationExitPresets == null
        )
        {
            return 0.5f;
        }

        var p = uiAnimation.animationExitPresets;

        float alpha =
            p.useAlphaAnimation
                ? p.alphaDelay + p.alphaDuration
                : 0f;

        float pos =
            p.usePositionAnimation
                ? p.positionDelay + p.positionDuration
                : 0f;

        float scale =
            p.useScaleAnimation
                ? p.scaleDelay + p.scaleDuration
                : 0f;

        float rot =
            p.useRotationAnimation
                ? p.rotationDelay + p.rotationDuration
                : 0f;

        return Mathf.Max(
            alpha,
            pos,
            scale,
            rot
        ) + 0.1f;
    }
}