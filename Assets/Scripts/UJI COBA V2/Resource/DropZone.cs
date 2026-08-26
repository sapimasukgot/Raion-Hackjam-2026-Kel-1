using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public enum DropZoneType
    {
        MemoDad,
        MemoMom,
        MemoDaughter,
        MemoSon,
        Diary,
        Door
    }

    [SerializeField] private DropZoneType dropZoneType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
            return;

        DragableItem dragItem = droppedObject.GetComponent<DragableItem>();

        if (dragItem == null)
            return;

        Debug.Log(
            droppedObject.name +
            " mencoba drop ke " +
            gameObject.name
        );

        if (IsValidDrop(dragItem.GetItemType()))
        {
            Debug.Log(
                "DROP VALID: " +
                droppedObject.name +
                " → " +
                gameObject.name
            );

            dragItem.SetDropped(true);

            RectTransform droppedRect =
                droppedObject.GetComponent<RectTransform>();

            droppedRect.position =
                GetComponent<RectTransform>().position;
        }
        else
        {
            Debug.Log(
                "DROP INVALID: " +
                droppedObject.name +
                " → " +
                gameObject.name
            );
        }
    }

    private bool IsValidDrop(DragableItem.ItemType itemType)
    {
        switch (dropZoneType)
        {
            case DropZoneType.MemoDad:
                return itemType == DragableItem.ItemType.Ration ||
                       itemType == DragableItem.ItemType.Medkit;

            case DropZoneType.MemoMom:
                return itemType == DragableItem.ItemType.Ration ||
                       itemType == DragableItem.ItemType.Medkit;

            case DropZoneType.MemoDaughter:
                return itemType == DragableItem.ItemType.Ration ||
                       itemType == DragableItem.ItemType.Medkit;

            case DropZoneType.MemoSon:
                return itemType == DragableItem.ItemType.Ration ||
                       itemType == DragableItem.ItemType.Medkit;

            case DropZoneType.Diary:
                return itemType == DragableItem.ItemType.Dad ||
                       itemType == DragableItem.ItemType.Mom ||
                       itemType == DragableItem.ItemType.Daughter ||
                       itemType == DragableItem.ItemType.Son;

            case DropZoneType.Door:
                return itemType == DragableItem.ItemType.Dad ||
                       itemType == DragableItem.ItemType.Mom ||
                       itemType == DragableItem.ItemType.Daughter ||
                       itemType == DragableItem.ItemType.Son;

            default:
                return false;
        }
    }
}