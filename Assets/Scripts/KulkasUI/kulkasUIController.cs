 using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class KulkasUIController : MonoBehaviour
{
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
    }

    private void Start()
    {
        // Sembunyikan UI di awal tanpa mematikan GameObject
        HideInstant();
    }

    public void OpenUI()
    {
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        // Buka interaksi UI
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Jalankan animasi Entrance
        if (uiAnimation != null)
        {
            uiAnimation.EntranceAnimation();
        }
    }

    public void CloseUI()
    {
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }
        closeCoroutine = StartCoroutine(CloseUIRoutine());
    }

    private IEnumerator CloseUIRoutine()
    {
        // Matikan interaksi agar tidak bisa diklik saat animasi keluar
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (uiAnimation != null)
        {
            uiAnimation.ExitAnimation();

            // Tunggu durasi animasi selesai
            float waitTime = CalculateExitDuration();
            yield return new WaitForSeconds(waitTime);
        }

        HideInstant();
        closeCoroutine = null;
    }

    private void HideInstant()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private float CalculateExitDuration()
    {
        if (uiAnimation == null || uiAnimation.animationExitPresets == null) return 0.5f;

        var p = uiAnimation.animationExitPresets;
        float alpha = p.useAlphaAnimation ? (p.alphaDelay + p.alphaDuration) : 0f;
        float pos = p.usePositionAnimation ? (p.positionDelay + p.positionDuration) : 0f;
        float scale = p.useScaleAnimation ? (p.scaleDelay + p.scaleDuration) : 0f;
        float rot = p.useRotationAnimation ? (p.rotationDelay + p.rotationDuration) : 0f;

        return Mathf.Max(alpha, pos, scale, rot) + 0.1f;
    }
}
