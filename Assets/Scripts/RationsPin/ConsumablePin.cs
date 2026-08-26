using UnityEngine;
using UnityEngine.EventSystems;

public class ConsumablePin : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Pin Settings")]
    public NeedType needType = NeedType.Food;
    public float snapRadius = 60f;

    [Tooltip("Diisi otomatis oleh PinSpawner saat spawn - stok yang dikurangi saat pin ini dipakai")]
    public SupplyData supplyData;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 originalAnchoredPos; // posisi spawn, tempat balik kalau gagal
    private NeedZone[] needZones;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        // Simpan posisi spawn SETELAH PinSpawner menaruh pin ini di posisinya.
        originalAnchoredPos = rectTransform.anchoredPosition;
        needZones = FindObjectsOfType<NeedZone>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            rectTransform.position = canvasRect.TransformPoint(localPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        NeedZone target = FindMatchingZoneInRange();

        if (target != null)
        {
            // Berhasil: penuhi kebutuhan, kurangi stok, lalu hilangkan pin ini
            target.Fulfill();

            if (supplyData != null)
                supplyData.TryConsume(1);

            Destroy(gameObject);
        }
        else
        {
            // Gagal / salah tempat: balik ke posisi spawn awal
            rectTransform.anchoredPosition = originalAnchoredPos;
        }
    }

    private NeedZone FindMatchingZoneInRange()
    {
        NeedZone best = null;
        float bestDist = float.MaxValue;

        foreach (var zone in needZones)
        {
            if (zone == null) continue;
            if (zone.needType != needType) continue; // tipe kebutuhan harus sama
            if (!zone.IsActive) continue;             // kebutuhan itu memang lagi aktif

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