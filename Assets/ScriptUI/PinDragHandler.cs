using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Tempel script ini di GameObject Image pin/magnet.
/// Pastikan pin punya component: Image (raycastTarget = true) dan CanvasGroup (opsional tapi disarankan).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class PinDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Snap Settings")]
    [Tooltip("Jarak maksimum (dalam unit canvas) supaya pin dianggap 'kena' snap zone")]
    public float snapRadius = 60f;

    [Tooltip("Kalau kosong, script otomatis cari semua SnapZone di scene saat Awake")]
    public SnapZone[] snapZones;

    [Header("Optional")]
    public bool onlyMatchingTag = false;
    public string myZoneTag = "Default";

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 originalAnchoredPos;
    private SnapZone currentZone; // zone yang lagi ditempati (kalau ada)

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (snapZones == null || snapZones.Length == 0)
            snapZones = FindObjectsOfType<SnapZone>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalAnchoredPos = rectTransform.anchoredPosition;

        // Lepas dari zone lama (kalau pin ini lagi nempel di suatu zone)
        if (currentZone != null)
        {
            currentZone.isOccupied = false;
            currentZone = null;
        }

        // Biar drag gak ke-block raycast dari pin sendiri
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        // Pastikan pin tampil paling depan saat di-drag
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
                out localPoint))
        {
            // Karena pin adalah child dari canvas (atau parent lain), konversi ulang ke local parent pin
            rectTransform.position = canvasRect.TransformPoint(localPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        SnapZone nearest = FindNearestAvailableZone();

        if (nearest != null)
        {
            // Snap ke posisi zone
            rectTransform.position = nearest.RectTransform.position;
            nearest.isOccupied = true;
            currentZone = nearest;
        }
        else
        {
            // Gak ada zone yang cukup dekat -> balik ke posisi awal
            rectTransform.anchoredPosition = originalAnchoredPos;
        }
    }

    private SnapZone FindNearestAvailableZone()
    {
        SnapZone best = null;
        float bestDist = float.MaxValue;

        foreach (var zone in snapZones)
        {
            if (zone == null) continue;
            if (zone.isOccupied) continue; // sudah dipakai pin lain
            if (onlyMatchingTag && zone.zoneTag != myZoneTag) continue;

            float dist = Vector3.Distance(rectTransform.position, zone.RectTransform.position);
            if (dist <= snapRadius && dist < bestDist)
            {
                bestDist = dist;
                best = zone;
            }
        }

        return best;
    }
}