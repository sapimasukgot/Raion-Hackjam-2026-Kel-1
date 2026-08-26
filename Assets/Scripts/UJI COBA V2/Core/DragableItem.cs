using UnityEngine;
using UnityEngine.EventSystems;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ItemType
    {
        Ration,
        Medkit,
        Tools,
        Knife,
        Dad,
        Mom,
        Daughter,
        Son
    }

    [SerializeField] private ItemType itemType;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalPosition;
    private bool wasDropped = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;
        wasDropped = false;

        canvasGroup.blocksRaycasts = false;

        Debug.Log(gameObject.name + " mulai di-drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!wasDropped)
        {
            rectTransform.position = originalPosition;
        }

        Debug.Log(gameObject.name + " selesai di-drag");
    }

    public void SetDropped(bool dropped)
    {
        wasDropped = dropped;
    }

    public ItemType GetItemType()
    {
        return itemType;
    }
}