using UnityEngine;
using UnityEngine.EventSystems;

public class DragableItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public enum ItemType
    {
        Ration,
        Medkit,
        Tools,
        Knife,

        Dad,
        Mom,
        Son,
        Daughter
    }

    [Header("Item")]
    [SerializeField] private ItemType itemType;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Posisi sebelum drag
    private Vector3 originalPosition;

    // Posisi rumah permanen
    private Vector3 homePosition;

    // Apakah drop terakhir berhasil
    private bool droppedSuccessfully = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // Simpan posisi rumah setelah UI selesai di-layout
        homePosition = rectTransform.position;

        Debug.Log(
            gameObject.name +
            " HOME POSITION disimpan: " +
            homePosition
        );
    }

    // =====================================================
    // GET ITEM TYPE
    // =====================================================

    public ItemType GetItemType()
    {
        return itemType;
    }

    // =====================================================
    // BEGIN DRAG
    // =====================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;

        droppedSuccessfully = false;

        canvasGroup.blocksRaycasts = false;

        Debug.Log(
            gameObject.name +
            " mulai di-drag."
        );
    }

    // =====================================================
    // DRAG
    // =====================================================

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    // =====================================================
    // END DRAG
    // =====================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Debug.Log(
            gameObject.name +
            " selesai di-drag."
        );

        // Drop tidak valid
        if (!droppedSuccessfully)
        {
            rectTransform.position = homePosition;

            Debug.Log(
                gameObject.name +
                " drop tidak valid → kembali ke HOME POSITION."
            );
        }
    }

    // =====================================================
    // DROP STATUS
    // =====================================================

    public void SetDropped(bool value)
    {
        droppedSuccessfully = value;
    }

    // =====================================================
    // RETURN KE HOME
    // =====================================================

    public void ReturnToInitialPosition()
    {
        if (rectTransform == null)
            return;

        rectTransform.position = homePosition;

        droppedSuccessfully = false;

        Debug.Log(
            gameObject.name +
            " kembali ke HOME POSITION: " +
            homePosition
        );
    }
}