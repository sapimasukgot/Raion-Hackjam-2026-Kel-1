using UnityEngine;
using UnityEngine.EventSystems;

public class PaperDropZone : MonoBehaviour, IDropHandler
{
    private RectTransform paperRectTransform;

    // Jarak toleransi magnet (opsional, jika ingin area magnet lebih luas dari ukuran kertas)
    [Header("Magnetic Settings")]
    [SerializeField] private float snapDistanceThreshold = 150f;

    private void Awake()
    {
        paperRectTransform = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj != null)
        {
            DraggableItem draggable = droppedObj.GetComponent<DraggableItem>();
            if (draggable != null && !draggable.isLocked)
            {
                // Cek apakah posisi objek cukup dekat / bersinggungan dengan kertas
                float distance = Vector2.Distance(
                    paperRectTransform.position, 
                    droppedObj.transform.position
                );

                // Selama OnDrop terpanggil (kena area kertas), langsung magnet ke tengah
                draggable.SnapToCenterAndLock(this.transform);
            }
        }
    }
}
