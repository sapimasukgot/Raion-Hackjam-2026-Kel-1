using UnityEngine;
using System.Collections.Generic;

public class ExpeditionManager : MonoBehaviour
{
    public static ExpeditionManager Instance;

    [Header("Pending Expedition")]
    public CharacterData pendingExpeditionCharacter;
    public bool hasPendingExpedition = false;
    public bool isBringingKnife = false;

    [Header("Loot Configuration")]
    [SerializeField] private LootItem[] possibleLoots;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =====================================================
    // SAVE PENDING EXPEDITION
    // =====================================================

    public bool SavePendingExpedition(CharacterData character, bool bringKnife = false)
    {
        if (character == null)
        {
            Debug.LogWarning("Character NULL untuk ekspedisi.");
            return false;
        }

        if (!character.isAlive)
        {
            Debug.LogWarning("Karakter mati tidak bisa ekspedisi!");
            return false;
        }

        if (character.isMissing)
        {
            Debug.LogWarning("Karakter sedang Missing!");
            return false;
        }

        if (!character.canExpedition)
        {
            Debug.LogWarning("Karakter ini tidak bisa ekspedisi!");
            return false;
        }

        // Save pending
        pendingExpeditionCharacter = character;
        hasPendingExpedition = true;
        isBringingKnife = bringKnife;

        Debug.Log("========================================");
        Debug.Log("EXPEDITION PENDING SAVED!");
        Debug.Log("Character: " + character.characterName);
        Debug.Log("Bring Knife: " + bringKnife);
        Debug.Log("hasPendingExpedition = TRUE");
        Debug.Log("========================================");

        return true;
    }

    // =====================================================
    // EXECUTE EXPEDITION DEPARTURE (HARI PERTAMA)
    // Dipanggil dari GameManager.NextDay()
    // =====================================================

    public void ExecutePendingExpedition()
    {
        if (!hasPendingExpedition || pendingExpeditionCharacter == null)
        {
            return;
        }

        Debug.Log("========================================");
        Debug.Log("EKSPEDISI BERANGKAT: " + pendingExpeditionCharacter.characterName);
        Debug.Log("Bawa Pisau: " + isBringingKnife);
        Debug.Log("========================================");

        // Set status pergi
        pendingExpeditionCharacter.isMissing = true;
        pendingExpeditionCharacter.missingDays = 0; // Reset missing counter
        pendingExpeditionCharacter.isExpedition = true;
        pendingExpeditionCharacter.expeditionHasKnife = isBringingKnife;

        // Clear pending (karena sudah berangkat)
        ClearPendingExpedition();
    }

    // =====================================================
    // PROCESS RETURNING EXPEDITIONS (HARI BERIKUTNYA)
    // Dipanggil dari GameManager untuk cek siapa yang pulang
    // =====================================================

    public void ProcessReturningExpeditions()
    {
        if (GameManager.Instance == null) return;

        CheckReturn(GameManager.Instance.dad);
        CheckReturn(GameManager.Instance.mom);
        CheckReturn(GameManager.Instance.son);
        CheckReturn(GameManager.Instance.daughter);
    }

    private void CheckReturn(CharacterData character)
    {
        if (character == null) return;

        // Hanya proses yang sedang ekspedisi dan sudah hilang 1 hari
        if (character.isExpedition && character.isMissing && character.missingDays >= 1)
        {
            Debug.Log("========================================");
            Debug.Log("EKSPEDISI PULANG: " + character.characterName);
            Debug.Log("========================================");

            // Roll loot berdasarkan pisau saat dia berangkat
            List<LootItem> loots = RollExpeditionLoot(character.expeditionHasKnife);

            // Apply loot ke inventory
            ApplyLoot(loots);

            // Reset status
            character.isExpedition = false;
            character.isMissing = false;
            character.missingDays = 0;
            character.expeditionHasKnife = false;

            // Return to initial position
            ReturnCharacterToHome(character);
        }
    }

    private void ReturnCharacterToHome(CharacterData character)
    {
        if (character == null) return;

        DragableItem[] items = FindObjectsByType<DragableItem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (DragableItem item in items)
        {
            if (item == null) continue;

            // Match character type
            if (IsCharacterItem(character, item))
            {
                item.ReturnToInitialPosition();
                Debug.Log(character.characterName + " kembali ke HOME POSITION.");
                break;
            }
        }
    }

    private bool IsCharacterItem(CharacterData character, DragableItem item)
    {
        if (character == null || item == null) return false;

        return (character == GameManager.Instance.dad && item.GetItemType() == DragableItem.ItemType.Dad) ||
               (character == GameManager.Instance.mom && item.GetItemType() == DragableItem.ItemType.Mom) ||
               (character == GameManager.Instance.son && item.GetItemType() == DragableItem.ItemType.Son) ||
               (character == GameManager.Instance.daughter && item.GetItemType() == DragableItem.ItemType.Daughter);
    }

    // =====================================================
    // ROLL EXPEDITION LOOT
    // =====================================================

    private List<LootItem> RollExpeditionLoot(bool hasKnife)
    {
        List<LootItem> loots = new List<LootItem>();

        // Roll jumlah loot
        int lootCount = RollLootCount(hasKnife);

        Debug.Log("Rolled loot count: " + lootCount);

        // Roll tiap loot
        for (int i = 0; i < lootCount; i++)
        {
            LootItem loot = RollSingleLoot();
            loots.Add(loot);
            Debug.Log("Loot " + (i + 1) + ": " + loot.lootType + " (x" + loot.amount + ")");
        }

        return loots;
    }

    // =====================================================
    // ROLL LOOT COUNT
    // =====================================================

    private int RollLootCount(bool hasKnife)
    {
        float roll = Random.Range(0f, 100f);

        Debug.Log("Loot count roll: " + roll);

        if (hasKnife)
        {
            // Dengan pisau
            // 0 loot: 15%, 1 loot: 45%, 2 loot: 30%, 3 loot: 10%
            if (roll < 15f)
                return 0;
            else if (roll < 60f)  // 15 + 45
                return 1;
            else if (roll < 90f)  // 60 + 30
                return 2;
            else
                return 3;
        }
        else
        {
            // Tanpa pisau
            // 0 loot: 30%, 1 loot: 60%, 2 loot: 9%, 3 loot: 1%
            if (roll < 30f)
                return 0;
            else if (roll < 90f)  // 30 + 60
                return 1;
            else if (roll < 99f)  // 90 + 9
                return 2;
            else
                return 3;
        }
    }

    // =====================================================
    // ROLL SINGLE LOOT
    // =====================================================

    private LootItem RollSingleLoot()
    {
        // Random loot type: Ration, Medicine, Tools, Knife (each 25%)
        int lootRoll = Random.Range(0, 4);

        LootType lootType;
        switch (lootRoll)
        {
            case 0:
                lootType = LootType.Ration;
                break;
            case 1:
                lootType = LootType.Medicine;
                break;
            case 2:
                lootType = LootType.Tools;
                break;
            case 3:
                lootType = LootType.Knife;
                break;
            default:
                lootType = LootType.Ration;
                break;
        }

        // Amount
        int amount = 1;

        // Knife only gives 1, others can give 1-2
        if (lootType != LootType.Knife)
        {
            amount = Random.Range(1, 3); // 1 or 2
        }

        return new LootItem(lootType, amount);
    }

    // =====================================================
    // APPLY LOOT
    // =====================================================

    private void ApplyLoot(List<LootItem> loots)
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager tidak ditemukan!");
            return;
        }

        if (loots.Count == 0)
        {
            Debug.Log("Ekspedisi pulang dengan tangan kosong.");
            return;
        }

        foreach (LootItem loot in loots)
        {
            switch (loot.lootType)
            {
                case LootType.Ration:
                    ResourceManager.Instance.AddRation(loot.amount);
                    Debug.Log("+ " + loot.amount + " Ration");
                    break;

                case LootType.Medicine:
                    ResourceManager.Instance.AddMedkit(loot.amount);
                    Debug.Log("+ " + loot.amount + " Medicine");
                    break;

                case LootType.Tools:
                    ResourceManager.Instance.AddTools(loot.amount);
                    Debug.Log("+ " + loot.amount + " Tools");
                    break;

                case LootType.Knife:
                    ResourceManager.Instance.AddKnife();
                    Debug.Log("+ 1 Knife");
                    break;
            }
        }
    }

    // =====================================================
    // CLEAR PENDING
    // =====================================================

    public void ClearPendingExpedition()
    {
        pendingExpeditionCharacter = null;
        hasPendingExpedition = false;
        isBringingKnife = false;

        Debug.Log("Pending expedition cleared.");
    }

    // =====================================================
    // CHECK IF CAN PROCEED NEXT DAY
    // =====================================================

    public bool CanProceedNextDay()
    {
        // Expedition TIDAK BLOCK next day
        // Player bisa next day kapan saja, ekspedisi akan diproses
        return true;
    }

    public string GetExpeditionBlockReason()
    {
        return "";
    }
}

// =====================================================
// LOOT DATA STRUCTURES
// =====================================================

public enum LootType
{
    Ration,
    Medicine,
    Tools,
    Knife
}

[System.Serializable]
public class LootItem
{
    public LootType lootType;
    public int amount;

    public LootItem(LootType type, int amt)
    {
        lootType = type;
        amount = amt;
    }
}
