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
        Door,
        EventSacrifice,
        Expedition
    }

    [SerializeField] private DropZoneType dropZoneType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
        {
            Debug.LogWarning("DropZone: droppedObject NULL");
            return;
        }

        DragableItem dragItem =
            droppedObject.GetComponent<DragableItem>();

        if (dragItem == null)
        {
            Debug.LogWarning("DropZone: DragableItem component NULL di " + droppedObject.name);
            return;
        }

        DragableItem.ItemType itemType =
            dragItem.GetItemType();

        Debug.Log("========================================");
        Debug.Log("DROP ATTEMPT: " + droppedObject.name + " (" + itemType + ") → " + gameObject.name + " (" + dropZoneType + ")");
        Debug.Log("========================================");

        // =====================================================
        // CEK CHARACTER MISSING
        // =====================================================

        if (IsCharacterMissing(itemType))
        {
            Debug.LogWarning("DROP FAILED: Character sedang Missing.");
            return;
        }

        // =====================================================
        // VALIDASI DROP
        // =====================================================

        bool isValid = IsValidDrop(itemType);

        if (!isValid)
        {
            Debug.LogWarning("DROP FAILED: Item tidak valid untuk zone " + dropZoneType);
            Debug.LogWarning("Item type: " + itemType);
            return;
        }

        Debug.Log("DROP SUCCESS: Valid drop!");

        // Tandai drop berhasil
        dragItem.SetDropped(true);

        // Jalankan aksi
        SetPendingAction(itemType, dragItem);
    }

    // =====================================================
    // VALIDASI DROP
    // =====================================================

    private bool IsValidDrop(
        DragableItem.ItemType itemType
    )
    {
        switch (dropZoneType)
        {
            case DropZoneType.MemoDad:
            case DropZoneType.MemoMom:
            case DropZoneType.MemoDaughter:
            case DropZoneType.MemoSon:

                return itemType ==
                           DragableItem.ItemType.Ration ||
                       itemType ==
                           DragableItem.ItemType.Medkit;

            case DropZoneType.Diary:

                return itemType ==
                           DragableItem.ItemType.Dad ||
                       itemType ==
                           DragableItem.ItemType.Mom ||
                       itemType ==
                           DragableItem.ItemType.Son ||
                       itemType ==
                           DragableItem.ItemType.Daughter;

            case DropZoneType.Door:

                return itemType ==
                           DragableItem.ItemType.Dad ||
                       itemType ==
                           DragableItem.ItemType.Mom ||
                       itemType ==
                           DragableItem.ItemType.Son ||
                       itemType ==
                           DragableItem.ItemType.Daughter;

            case DropZoneType.EventSacrifice:

                // Harus ada event aktif dulu.
                if (EventManager.Instance == null ||
                    !EventManager.Instance.IsEventActive())
                {
                    return false;
                }

                RandomEventSO activeEvent = EventManager.Instance.currentEvent;

                // Event butuh pengorbanan karakter (Jari/Tangan/Kaki)
                if (activeEvent.requirementType == EventRequirementType.CharacterPart)
                {
                    return itemType ==
                               DragableItem.ItemType.Dad ||
                           itemType ==
                               DragableItem.ItemType.Mom ||
                           itemType ==
                               DragableItem.ItemType.Son ||
                           itemType ==
                               DragableItem.ItemType.Daughter;
                }

                // Event butuh item (Tools / Knife) → hanya item yang SESUAI yang valid di-drop
                if (activeEvent.requirementType == EventRequirementType.Item)
                {
                    if (activeEvent.requiredItem == RequiredItemType.Tools)
                        return itemType == DragableItem.ItemType.Tools;

                    if (activeEvent.requiredItem == RequiredItemType.Knife)
                        return itemType == DragableItem.ItemType.Knife;
                }

                return false;

            case DropZoneType.Expedition:

                // Karakter atau pisau
                return itemType ==
                           DragableItem.ItemType.Dad ||
                       itemType ==
                           DragableItem.ItemType.Mom ||
                       itemType ==
                           DragableItem.ItemType.Son ||
                       itemType ==
                           DragableItem.ItemType.Daughter ||
                       itemType ==
                           DragableItem.ItemType.Knife;

            default:
                return false;
        }
    }

    // =====================================================
    // CEK CHARACTER MISSING
    // =====================================================

    private bool IsCharacterMissing(
        DragableItem.ItemType itemType
    )
    {
        if (GameManager.Instance == null)
            return false;

        // Dad
        if (dropZoneType == DropZoneType.MemoDad &&
            (itemType == DragableItem.ItemType.Ration ||
             itemType == DragableItem.ItemType.Medkit))
        {
            return GameManager.Instance.dad != null &&
                   GameManager.Instance.dad.isMissing;
        }

        // Mom
        if (dropZoneType == DropZoneType.MemoMom &&
            (itemType == DragableItem.ItemType.Ration ||
             itemType == DragableItem.ItemType.Medkit))
        {
            return GameManager.Instance.mom != null &&
                   GameManager.Instance.mom.isMissing;
        }

        // Son
        if (dropZoneType == DropZoneType.MemoSon &&
            (itemType == DragableItem.ItemType.Ration ||
             itemType == DragableItem.ItemType.Medkit))
        {
            return GameManager.Instance.son != null &&
                   GameManager.Instance.son.isMissing;
        }

        // Daughter
        if (dropZoneType == DropZoneType.MemoDaughter &&
            (itemType == DragableItem.ItemType.Ration ||
             itemType == DragableItem.ItemType.Medkit))
        {
            return GameManager.Instance.daughter != null &&
                   GameManager.Instance.daughter.isMissing;
        }

        return false;
    }

    // =====================================================
    // PENDING ACTION
    // =====================================================

    private void SetPendingAction(
        DragableItem.ItemType itemType,
        DragableItem dragItem
    )
    {
        // =================================================
        // RATION → FEEDING
        // =================================================

        if (itemType == DragableItem.ItemType.Ration)
        {
            switch (dropZoneType)
            {
                case DropZoneType.MemoDad:

                    GameManager.Instance.SetPendingFeeding(
                        GameManager.Instance.dad
                    );

                    Debug.Log(
                        "Pending Feeding → Dad"
                    );

                    break;

                case DropZoneType.MemoMom:

                    GameManager.Instance.SetPendingFeeding(
                        GameManager.Instance.mom
                    );

                    Debug.Log(
                        "Pending Feeding → Mom"
                    );

                    break;

                case DropZoneType.MemoSon:

                    GameManager.Instance.SetPendingFeeding(
                        GameManager.Instance.son
                    );

                    Debug.Log(
                        "Pending Feeding → Son"
                    );

                    break;

                case DropZoneType.MemoDaughter:

                    GameManager.Instance.SetPendingFeeding(
                        GameManager.Instance.daughter
                    );

                    Debug.Log(
                        "Pending Feeding → Daughter"
                    );

                    break;
            }
        }

        // =================================================
        // MEDKIT → TREATMENT
        // =================================================

        if (itemType == DragableItem.ItemType.Medkit)
        {
            switch (dropZoneType)
            {
                case DropZoneType.MemoDad:

                    GameManager.Instance.SetPendingTreatment(
                        GameManager.Instance.dad
                    );

                    Debug.Log(
                        "Pending Treatment → Dad"
                    );

                    break;

                case DropZoneType.MemoMom:

                    GameManager.Instance.SetPendingTreatment(
                        GameManager.Instance.mom
                    );

                    Debug.Log(
                        "Pending Treatment → Mom"
                    );

                    break;

                case DropZoneType.MemoSon:

                    GameManager.Instance.SetPendingTreatment(
                        GameManager.Instance.son
                    );

                    Debug.Log(
                        "Pending Treatment → Son"
                    );

                    break;

                case DropZoneType.MemoDaughter:

                    GameManager.Instance.SetPendingTreatment(
                        GameManager.Instance.daughter
                    );

                    Debug.Log(
                        "Pending Treatment → Daughter"
                    );

                    break;
            }
        }

        // =================================================
        // CHARACTER → DOOR
        // =================================================

        if (dropZoneType == DropZoneType.Door)
        {
            switch (itemType)
            {
                case DragableItem.ItemType.Dad:

                    GameManager.Instance.SetPendingExit(
                        GameManager.Instance.dad
                    );

                    Debug.Log(
                        "Pending Exit → Dad"
                    );

                    break;

                case DragableItem.ItemType.Mom:

                    GameManager.Instance.SetPendingExit(
                        GameManager.Instance.mom
                    );

                    Debug.Log(
                        "Pending Exit → Mom"
                    );

                    break;

                case DragableItem.ItemType.Son:

                    GameManager.Instance.SetPendingExit(
                        GameManager.Instance.son
                    );

                    Debug.Log(
                        "Pending Exit → Son"
                    );

                    break;

                case DragableItem.ItemType.Daughter:

                    GameManager.Instance.SetPendingExit(
                        GameManager.Instance.daughter
                    );

                    Debug.Log(
                        "Pending Exit → Daughter"
                    );

                    break;
            }
        }

        // =================================================
        // CHARACTER → DIARY
        // =================================================

            // =================================================
        // CHARACTER → DIARY
        // =================================================

        if (dropZoneType == DropZoneType.Diary)
        {
            Debug.Log(
                "Character masuk Diary: " +
                itemType
            );

            switch (itemType)
            {
                case DragableItem.ItemType.Dad:

                    GameManager.Instance.SetPendingSacrifice(
                        GameManager.Instance.dad
                    );

                    Debug.Log(
                        "Pending Sacrifice → Dad"
                    );

                    break;

                case DragableItem.ItemType.Mom:

                    GameManager.Instance.SetPendingSacrifice(
                        GameManager.Instance.mom
                    );

                    Debug.Log(
                        "Pending Sacrifice → Mom"
                    );

                    break;

                case DragableItem.ItemType.Son:

                    GameManager.Instance.SetPendingSacrifice(
                        GameManager.Instance.son
                    );

                    Debug.Log(
                        "Pending Sacrifice → Son"
                    );

                    break;

                case DragableItem.ItemType.Daughter:

                    GameManager.Instance.SetPendingSacrifice(
                        GameManager.Instance.daughter
                    );

                    Debug.Log(
                        "Pending Sacrifice → Daughter"
                    );

                    break;
            }
        }

        // =================================================
        // CHARACTER / ITEM → EVENT SACRIFICE (ZONA EVENT)
        // =================================================

        if (dropZoneType == DropZoneType.EventSacrifice)
        {
            // --- Kasus 1: yang di-drop adalah KARAKTER (pengorbanan Jari/Tangan/Kaki) ---
            CharacterData sacrificedCharacter = null;

            switch (itemType)
            {
                case DragableItem.ItemType.Dad:
                    sacrificedCharacter = GameManager.Instance.dad;
                    break;

                case DragableItem.ItemType.Mom:
                    sacrificedCharacter = GameManager.Instance.mom;
                    break;

                case DragableItem.ItemType.Son:
                    sacrificedCharacter = GameManager.Instance.son;
                    break;

                case DragableItem.ItemType.Daughter:
                    sacrificedCharacter = GameManager.Instance.daughter;
                    break;
            }

            if (sacrificedCharacter != null && EventManager.Instance != null)
            {
                bool resolved = EventManager.Instance.SavePendingCharacterSacrifice(sacrificedCharacter);

                Debug.Log(
                    resolved
                        ? sacrificedCharacter.characterName + " berhasil dikorbankan untuk event."
                        : "Pengorbanan " + sacrificedCharacter.characterName + " GAGAL diproses."
                );
            }

            // --- Kasus 2: yang di-drop adalah ITEM (Tools / Knife) ---
            if (itemType == DragableItem.ItemType.Tools ||
                itemType == DragableItem.ItemType.Knife)
            {
                if (EventManager.Instance != null)
                {
                    bool resolved = EventManager.Instance.SavePendingItemRequirement();

                    Debug.Log(
                        resolved
                            ? "Event berhasil diselesaikan dengan " + itemType
                            : "Gagal menyelesaikan event dengan " + itemType
                    );
                }
            }
        }

        // =================================================
        // EXPEDITION
        // =================================================

        if (dropZoneType == DropZoneType.Expedition)
        {
            // Ensure ExpeditionManager exists
            if (ExpeditionManager.Instance == null)
            {
                GameObject expeditionManagerObj = new GameObject("ExpeditionManager");
                ExpeditionManager.Instance = expeditionManagerObj.AddComponent<ExpeditionManager>();
                Debug.Log("ExpeditionManager auto-created.");
            }

            // --- Kasus 1: yang di-drop adalah KARAKTER ---
            if (itemType == DragableItem.ItemType.Dad ||
                itemType == DragableItem.ItemType.Mom ||
                itemType == DragableItem.ItemType.Son ||
                itemType == DragableItem.ItemType.Daughter)
            {
                // CHECK: Apakah hanya 1 anggota keluarga yang hidup (3 mati)?
                if (IsOnlyOneFamilyMemberAlive())
                {
                    Debug.LogWarning("EKSPEDISI DITOLAK: Hanya 1 anggota keluarga yang masih hidup. Tidak bisa ekspedisi!");
                    
                    // Kembalikan karakter ke posisi awal
                    if (dragItem != null)
                    {
                        dragItem.SetDropped(false); // Mark as not dropped successfully
                    }
                    
                    return;
                }

                CharacterData expeditionCharacter = null;

                switch (itemType)
                {
                    case DragableItem.ItemType.Dad:
                        expeditionCharacter = GameManager.Instance.dad;
                        break;

                    case DragableItem.ItemType.Mom:
                        expeditionCharacter = GameManager.Instance.mom;
                        break;

                    case DragableItem.ItemType.Son:
                        expeditionCharacter = GameManager.Instance.son;
                        break;

                    case DragableItem.ItemType.Daughter:
                        expeditionCharacter = GameManager.Instance.daughter;
                        break;
                }

                if (expeditionCharacter != null)
                {
                    // Cek apakah membawa pisau
                    bool bringKnife = ExpeditionManager.Instance.isBringingKnife;

                    // SAVE PENDING EXPEDITION
                    bool saved = ExpeditionManager.Instance.SavePendingExpedition(expeditionCharacter, bringKnife);

                    Debug.Log(
                        saved
                            ? "Pending Expedition: " + expeditionCharacter.characterName + " | Bring Knife: " + bringKnife
                            : "Gagal menyimpan pending expedition untuk " + expeditionCharacter.characterName
                    );
                }
                else
                {
                    Debug.LogError("Expedition character NULL!");
                }
            }

            // --- Kasus 2: yang di-drop adalah PISAU (opsional) ---
            if (itemType == DragableItem.ItemType.Knife)
            {
                if (ExpeditionManager.Instance != null)
                {
                    // Set bringing knife = true
                    ExpeditionManager.Instance.isBringingKnife = true;

                    Debug.Log("Knife dibawa untuk expedition! Loot odds increased.");

                    // Hancurkan knife item (karena konsumsi)
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.knife = false;
                        Debug.Log("Knife consumed. GameManager.knife = false");
                    }
                    else
                    {
                        Debug.LogError("GameManager.Instance NULL! Knife tidak dikonsumsi.");
                    }
                }
            }
        }


    }

    // =====================================================
    // CHECK IF ONLY ONE FAMILY MEMBER ALIVE
    // =====================================================

    private bool IsOnlyOneFamilyMemberAlive()
    {
        if (GameManager.Instance == null)
            return false;

        int aliveCount = 0;

        if (GameManager.Instance.dad != null && GameManager.Instance.dad.isAlive)
            aliveCount++;

        if (GameManager.Instance.mom != null && GameManager.Instance.mom.isAlive)
            aliveCount++;

        if (GameManager.Instance.son != null && GameManager.Instance.son.isAlive)
            aliveCount++;

        if (GameManager.Instance.daughter != null && GameManager.Instance.daughter.isAlive)
            aliveCount++;

        return aliveCount <= 1;
    }
}