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
        EventSacrifice
    }

    [SerializeField] private DropZoneType dropZoneType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
            return;

        DragableItem dragItem =
            droppedObject.GetComponent<DragableItem>();

        if (dragItem == null)
            return;

        DragableItem.ItemType itemType =
            dragItem.GetItemType();

        Debug.Log(
            droppedObject.name +
            " mencoba drop ke " +
            gameObject.name
        );

        // =====================================================
        // CEK CHARACTER MISSING
        // =====================================================

        if (IsCharacterMissing(itemType))
        {
            Debug.Log(
                "DROP INVALID: " +
                droppedObject.name +
                " → " +
                gameObject.name +
                " | Character sedang Missing."
            );

            return;
        }

        // =====================================================
        // VALIDASI DROP
        // =====================================================

        if (!IsValidDrop(itemType))
        {
            Debug.Log(
                "DROP INVALID: " +
                droppedObject.name +
                " → " +
                gameObject.name
            );

            return;
        }

        Debug.Log(
            "DROP VALID: " +
            droppedObject.name +
            " → " +
            gameObject.name
        );

        // Tandai drop berhasil
        dragItem.SetDropped(true);

        // Jangan memindahkan posisi object ke DropZone.
        // Posisi object tetap ditentukan oleh sistem drag.

        // Jalankan aksi
        SetPendingAction(itemType);
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
        DragableItem.ItemType itemType
    )
    {
        // =================================================
        // RATION → FEEDING
        // =================================================

        if (itemType == DragableItem.ItemType.Ration)
        {
            // CEK RESOURCE DULU
            if (GameManager.Instance.ration <= 0)
            {
                Debug.LogWarning("Ration habis! Tidak bisa drop.");
                return;
            }

            // LANGSUNG KURANGI RESOURCE
            GameManager.Instance.ration--;

            Debug.Log("Ration digunakan. Tersisa: " + GameManager.Instance.ration);

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
            // CEK RESOURCE DULU
            if (GameManager.Instance.medkit <= 0)
            {
                Debug.LogWarning("Medkit habis! Tidak bisa drop.");
                return;
            }

            // LANGSUNG KURANGI RESOURCE
            GameManager.Instance.medkit--;

            Debug.Log("Medkit digunakan. Tersisa: " + GameManager.Instance.medkit);

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
                // SAVE PENDING - tidak langsung execute
                bool saved = EventManager.Instance.SavePendingCharacterSacrifice(sacrificedCharacter);

                Debug.Log(
                    saved
                        ? "Pending sacrifice: " + sacrificedCharacter.characterName + " untuk event. Akan dieksekusi saat NextDay."
                        : "Gagal menyimpan pending sacrifice untuk " + sacrificedCharacter.characterName
                );
            }

            // --- Kasus 2: yang di-drop adalah ITEM (Tools / Knife) ---
            if (itemType == DragableItem.ItemType.Tools ||
                itemType == DragableItem.ItemType.Knife)
            {
                if (EventManager.Instance != null)
                {
                    // SAVE PENDING - tidak langsung execute
                    bool saved = EventManager.Instance.SavePendingItemRequirement();

                    Debug.Log(
                        saved
                            ? "Pending item requirement untuk event. Akan dieksekusi saat NextDay."
                            : "Gagal menyimpan pending item requirement."
                    );
                }
            }
        }


    }
}