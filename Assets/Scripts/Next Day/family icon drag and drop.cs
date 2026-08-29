using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    // Menyimpan posisi dan parent awal
    private Vector2 initialAnchoredPos;
    private Transform initialParent;

    [HideInInspector] public bool isLocked = false;
    [HideInInspector] public bool isDroppedSuccessfully = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // Simpan posisi dan parent bawaan awal
        initialAnchoredPos = rectTransform.anchoredPosition;
        initialParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        isDroppedSuccessfully = false;

        // Pindah sementara ke root Canvas agar terlihat paling depan saat ditarik
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        // Gerakkan objek mengikuti kursor/jari
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = true;

        // Jika tidak jatuh/nempel di kertas, kembalikan ke posisi awal
        if (!isDroppedSuccessfully)
        {
            ReturnToInitialPosition();
        }
    }

    // Fungsi kembali ke posisi semula
    public void ReturnToInitialPosition()
    {
        transform.SetParent(initialParent, false);
        rectTransform.anchoredPosition = initialAnchoredPos;
    }

    // Fungsi magnet nempel ke tengah dan mengunci objek
    public void SnapToCenterAndLock(Transform targetPaper)
    {
        isDroppedSuccessfully = true;
        isLocked = true;

        // Jadikan kertas sebagai parent
        transform.SetParent(targetPaper, false);

        // MAGNET: Paksa posisi tepat di titik tengah kertas (0, 0)
        rectTransform.anchoredPosition = Vector2.zero;

        // Matikan raycast agar tidak bisa digeser lagi
        canvasGroup.blocksRaycasts = false;
    }
}
