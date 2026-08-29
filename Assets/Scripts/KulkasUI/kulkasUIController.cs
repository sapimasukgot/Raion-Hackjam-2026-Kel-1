using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class KulkasUIController : MonoBehaviour
{
    [Header("GameObject UI Kulkas")]
    public GameObject kulkasUIObject;

    [Header("Referensi Animasi UI")]
    public UIAutoAnimation uiAnimation;

    private CanvasGroup canvasGroup;
    private Coroutine closeCoroutine;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (uiAnimation == null)
        {
            uiAnimation = GetComponent<UIAutoAnimation>();
        }

        if (kulkasUIObject == null)
        {
            Debug.LogError(
                "❌ Kulkas UI Object belum di-assign di Inspector!",
                this
            );
        }
    }


    private void Start()
    {
        HideInstant();
    }


    // =====================================================
    // OPEN UI
    // =====================================================

    public void OpenUI()
    {
        // 1. HENTIKAN CLOSE YANG MASIH BERJALAN
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }


        // 2. AKTIFKAN GAMEOBJECT DARI INSPECTOR
        if (kulkasUIObject != null)
        {
            kulkasUIObject.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "❌ Kulkas UI Object belum di-assign!",
                this
            );

            return;
        }


        // 3. AKTIFKAN CANVAS GROUP
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;


        // 4. JALANKAN ANIMASI MASUK
        if (uiAnimation != null)
        {
            uiAnimation.EntranceAnimation();
        }


        Debug.Log(
            "🧊 Kulkas UI berhasil dibuka."
        );
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
            StartCoroutine(
                CloseUIRoutine()
            );
    }


    // =====================================================
    // CLOSE ROUTINE
    // =====================================================

    private IEnumerator CloseUIRoutine()
    {
        // 1. MATIKAN INTERAKSI
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;


        // 2. ANIMASI KELUAR
        if (uiAnimation != null)
        {
            uiAnimation.ExitAnimation();

            float waitTime =
                CalculateExitDuration();

            yield return new WaitForSeconds(
                waitTime
            );
        }


        // 3. SEMBUNYIKAN CANVAS GROUP
        HideInstant();


        // 4. MATIKAN GAMEOBJECT DARI INSPECTOR
        if (kulkasUIObject != null)
        {
            kulkasUIObject.SetActive(false);
        }


        closeCoroutine = null;


        Debug.Log(
            "🧊 Kulkas UI berhasil ditutup."
        );
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

        var p =
            uiAnimation.animationExitPresets;

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