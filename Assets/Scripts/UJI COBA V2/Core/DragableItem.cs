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

    [Header("Destroy on Drop")]
    [SerializeField] private bool destroyOnSuccessfulDrop = false;
    [SerializeField] private float destroyDelay = 0.1f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Posisi sebelum drag
    private Vector3 originalPosition;

    // Posisi rumah permanen
    private Vector3 homePosition;

    // Apakah drop terakhir berhasil
    private bool droppedSuccessfully = false;

    // Reference ke parent layout group
    private UnityEngine.UI.LayoutGroup parentLayoutGroup;

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

        // Disable parent layout group saat drag
        DisableParentLayoutGroup();

        // Notify spawner untuk pause refresh
        ResourceItemSpawner.SetDragging(true);

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

        // Re-enable parent layout group
        EnableParentLayoutGroup();

        // Notify spawner untuk resume refresh
        ResourceItemSpawner.SetDragging(false);

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
        else
        {
            // Drop berhasil
            Debug.Log(
                gameObject.name +
                " drop BERHASIL!"
            );

            // Destroy item jika setting enabled
            if (destroyOnSuccessfulDrop)
            {
                Debug.Log(
                    gameObject.name +
                    " akan dihancurkan dalam " + destroyDelay + "s"
                );

                Destroy(gameObject, destroyDelay);
            }
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

    // =====================================================
    // DISABLE/ENABLE PARENT LAYOUT GROUP
    // =====================================================

    private void DisableParentLayoutGroup()
    {
        if (transform.parent == null)
            return;

        parentLayoutGroup = transform.parent.GetComponent<UnityEngine.UI.LayoutGroup>();

        if (parentLayoutGroup != null)
        {
            parentLayoutGroup.enabled = false;
            Debug.Log("Parent layout group disabled during drag.");
        }
    }

    private void EnableParentLayoutGroup()
    {
        if (parentLayoutGroup != null)
        {
            parentLayoutGroup.enabled = true;
            Debug.Log("Parent layout group re-enabled.");
        }
    }
}