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

    [Header("Drag Visual Settings")]
    [SerializeField] private float dragAlpha = 0.6f; // Transparansi saat drag
    [SerializeField] private float dragScale = 0.9f; // Scale saat drag

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    // Posisi sebelum drag
    private Vector3 originalPosition;

    // Posisi rumah permanen
    private Vector3 homePosition;

    // Apakah drop terakhir berhasil
    private bool droppedSuccessfully = false;

    // Reference ke parent layout group
    private UnityEngine.UI.LayoutGroup parentLayoutGroup;

    // Track if character was alive last frame
    private bool wasAliveLastFrame = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Simpan scale original
        originalScale = transform.localScale;
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

        // Check initial status - hide if character is already dead
        CheckCharacterStatus();
    }

    private void Update()
    {
        // Continuous check for character death
        CheckCharacterStatus();
    }

    // =====================================================
    // CHECK CHARACTER STATUS
    // Sembunyikan item jika karakter mati
    // =====================================================

    private void CheckCharacterStatus()
    {
        // Only check for character items
        if (!IsCharacterItem())
            return;

        bool isAlive = !IsCharacterDead();

        // If character just died (was alive before, now dead)
        if (wasAliveLastFrame && !isAlive)
        {
            Debug.Log(gameObject.name + " CHARACTER DIED - Hiding drag item.");
            HideItem();
        }
        // If character came back to life (was dead before, now alive)
        else if (!wasAliveLastFrame && isAlive)
        {
            ShowItem();
        }

        wasAliveLastFrame = isAlive;
    }

    // =====================================================
    // HIDE ITEM
    // =====================================================

    private void HideItem()
    {
        gameObject.SetActive(false);
    }

    // =====================================================
    // SHOW ITEM
    // =====================================================

    private void ShowItem()
    {
        gameObject.SetActive(true);
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
        // Check if this is a dead family member
        if (IsCharacterItem() && IsCharacterDead())
        {
            Debug.Log(gameObject.name + " is DEAD. Cannot drag.");
            return;
        }

        originalPosition = rectTransform.position;

        droppedSuccessfully = false;

        canvasGroup.blocksRaycasts = false;

        // Visual feedback: transparansi dan scale
        canvasGroup.alpha = dragAlpha;
        transform.localScale = originalScale * dragScale;

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
        // Check if this is a dead family member
        if (IsCharacterItem() && IsCharacterDead())
        {
            return;
        }

        // Langsung set posisi mengikuti cursor (smooth karena dipanggil setiap frame)
        rectTransform.position = eventData.position;
    }

    // =====================================================
    // END DRAG
    // =====================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if this is a dead family member
        if (IsCharacterItem() && IsCharacterDead())
        {
            return;
        }

        canvasGroup.blocksRaycasts = true;

        // Kembalikan visual ke normal
        canvasGroup.alpha = 1f;
        transform.localScale = originalScale;

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

            // Untuk resource consumable: kurangi resource DAN destroy item LANGSUNG
            if (IsConsumableResource())
            {
                // Kalau item ini Ration, mainkan SFX makan
                if (itemType == ItemType.Ration && SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEating();
                }

                // KURANGI RESOURCE LANGSUNG
                ConsumeResourceImmediately();

                Debug.Log(
                    gameObject.name +
                    " dikonsumsi dan dihancurkan LANGSUNG"
                );

                // DESTROY LANGSUNG tanpa delay
                Destroy(gameObject);

                // Force refresh spawner untuk update jumlah item di UI
                if (ResourceItemSpawner.Instance != null)
                {
                    // Remove item dari spawner list agar tidak ada duplicate
                    ResourceItemSpawner.Instance.RemoveItem(itemType.ToString(), gameObject);
                    
                    // Force refresh untuk pastikan UI up to date
                    ResourceItemSpawner.Instance.ForceRefresh();
                }
            }
            // Untuk item lain yang punya setting destroyOnSuccessfulDrop
            else if (destroyOnSuccessfulDrop)
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

    // =====================================================
    // CHECK IF CHARACTER ITEM
    // =====================================================

    private bool IsCharacterItem()
    {
        return itemType == ItemType.Dad ||
               itemType == ItemType.Mom ||
               itemType == ItemType.Son ||
               itemType == ItemType.Daughter;
    }

    // =====================================================
    // CHECK IF CONSUMABLE RESOURCE
    // =====================================================

    private bool IsConsumableResource()
    {
        // Ration, Medkit, Tools langsung di-consume saat drop
        // Knife TIDAK termasuk karena di-handle khusus oleh DropZone (Expedition)
        return itemType == ItemType.Ration ||
               itemType == ItemType.Medkit ||
               itemType == ItemType.Tools;
    }

    // =====================================================
    // CONSUME RESOURCE IMMEDIATELY
    // Kurangi resource dari GameManager LANGSUNG saat item di-drop
    // =====================================================

    private void ConsumeResourceImmediately()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance NULL! Resource tidak dikurangi.");
            return;
        }

        switch (itemType)
        {
            case ItemType.Ration:
                if (GameManager.Instance.ration > 0)
                {
                    GameManager.Instance.ration -= 1;
                    Debug.Log("Ration consumed IMMEDIATELY (-1). Remaining: " + GameManager.Instance.ration);
                }
                else
                {
                    Debug.LogWarning("No Ration available to consume!");
                }
                break;

            case ItemType.Medkit:
                if (GameManager.Instance.medkit > 0)
                {
                    GameManager.Instance.medkit -= 1;
                    Debug.Log("Medkit consumed IMMEDIATELY (-1). Remaining: " + GameManager.Instance.medkit);
                }
                else
                {
                    Debug.LogWarning("No Medkit available to consume!");
                }
                break;

            case ItemType.Tools:
                if (GameManager.Instance.tools > 0)
                {
                    GameManager.Instance.tools -= 1;
                    Debug.Log("Tools consumed IMMEDIATELY (-1). Remaining: " + GameManager.Instance.tools);
                }
                else
                {
                    Debug.LogWarning("No Tools available to consume!");
                }
                break;

            case ItemType.Knife:
                if (GameManager.Instance.knife)
                {
                    GameManager.Instance.knife = false;
                    Debug.Log("Knife consumed IMMEDIATELY");
                }
                else
                {
                    Debug.LogWarning("No Knife available to consume!");
                }
                break;
        }
    }

    // =====================================================
    // CHECK IF CHARACTER IS DEAD
    // =====================================================

    private bool IsCharacterDead()
    {
        if (GameManager.Instance == null)
            return false;

        CharacterData character = GetCharacterData();
        
        if (character == null)
            return false;

        return !character.isAlive;
    }

    // =====================================================
    // GET CHARACTER DATA
    // =====================================================

    private CharacterData GetCharacterData()
    {
        if (GameManager.Instance == null)
            return null;

        switch (itemType)
        {
            case ItemType.Dad:
                return GameManager.Instance.dad;
            case ItemType.Mom:
                return GameManager.Instance.mom;
            case ItemType.Son:
                return GameManager.Instance.son;
            case ItemType.Daughter:
                return GameManager.Instance.daughter;
            default:
                return null;
        }
    }
}