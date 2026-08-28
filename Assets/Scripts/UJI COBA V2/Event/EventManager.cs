using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Event Aktif Hari Ini")]
    public RandomEventSO currentEvent;
    public bool currentEventResolved = false;

    [Header("Pending Event Execution")]
    public RandomEventSO pendingEvent;
    public CharacterData pendingEventTarget;
    public bool hasPendingEvent = false;

    private void Awake()
    {
        Instance = this;
    }

    // =====================================================
    // SET EVENT AKTIF
    // =====================================================

    /// <summary>
    /// Dipanggil dari ReportUIController tiap kali report/hari baru ditampilkan.
    /// Kalau newEvent = null, berarti hari ini memang tidak ada random event.
    /// </summary>
    public void SetCurrentEvent(RandomEventSO newEvent)
    {
        currentEvent = newEvent;
        currentEventResolved = false;

        // Clear pending dari hari sebelumnya
        ClearPendingEvent();

        Debug.Log("========================================");
        Debug.Log(
            currentEvent != null
                ? "Event aktif HARI INI: " + currentEvent.eventTitle
                : "Tidak ada event aktif hari ini."
        );
        Debug.Log("========================================");
    }

    public bool IsEventActive()
    {
        return currentEvent != null && !currentEventResolved;
    }

    // =====================================================
    // CAN PROCEED NEXT DAY
    // Cek apakah event requirement sudah terpenuhi
    // =====================================================

    public bool CanProceedNextDay()
    {
        // Kalau tidak ada event aktif, boleh next day
        if (!IsEventActive())
            return true;

        // Kalau event sudah resolved, boleh next day
        if (currentEventResolved)
            return true;

        // Kalau ada pending event (player sudah drag target), boleh next day
        if (hasPendingEvent)
            return true;

        // Event aktif tapi belum ada yang di-drag -> TIDAK BOLEH next day
        return false;
    }

    public string GetEventBlockReason()
    {
        if (!IsEventActive())
            return "";

        if (currentEventResolved || hasPendingEvent)
            return "";

        // Event butuh character sacrifice
        if (currentEvent.requirementType == EventRequirementType.CharacterPart)
        {
            return "Drag karakter ke Target Zone untuk menyelesaikan event!";
        }

        // Event butuh item
        if (currentEvent.requirementType == EventRequirementType.Item)
        {
            return "Selesaikan event dengan item yang dibutuhkan!";
        }

        return "Selesaikan event terlebih dahulu!";
    }

    // =====================================================
    // SAVE PENDING - ITEM REQUIREMENT (Tools / Knife)
    // =====================================================

    /// <summary>
    /// Simpan pending event yang butuh item.
    /// TIDAK langsung execute effect.
    /// Effect dijalankan saat GameManager.ExecuteAllPending()
    /// </summary>
    public bool SavePendingItemRequirement()
    {
        if (!IsEventActive())
        {
            Debug.LogWarning("Tidak ada event aktif untuk disimpan.");
            return false;
        }

        if (currentEvent.requirementType != EventRequirementType.Item)
        {
            Debug.LogWarning("Event '" + currentEvent.eventTitle + "' tidak butuh item.");
            return false;
        }

        // Cek resource dulu
        bool hasEnoughResource = false;

        switch (currentEvent.requiredItem)
        {
            case RequiredItemType.Tools:
                hasEnoughResource = GameManager.Instance.tools >= currentEvent.requiredItemAmount;
                break;

            case RequiredItemType.Knife:
                hasEnoughResource = GameManager.Instance.knife;
                break;
        }

        if (!hasEnoughResource)
        {
            Debug.Log("Resource tidak cukup untuk menyelesaikan event.");
            return false;
        }

        // Simpan pending
        pendingEvent = currentEvent;
        pendingEventTarget = null; // Item event tidak butuh target character
        hasPendingEvent = true;

        Debug.Log("Pending Event (Item): " + currentEvent.eventTitle);

        return true;
    }

    // =====================================================
    // SAVE PENDING - CHARACTER SACRIFICE
    // =====================================================

    /// <summary>
    /// Simpan pending event yang butuh pengorbanan character.
    /// TIDAK langsung execute effect.
    /// Effect dijalankan saat GameManager.ExecuteAllPending()
    /// </summary>
    public bool SavePendingCharacterSacrifice(CharacterData character)
    {
        if (!IsEventActive())
        {
            Debug.LogWarning("Tidak ada event aktif untuk disimpan.");
            return false;
        }

        if (currentEvent.requirementType != EventRequirementType.CharacterPart)
        {
            Debug.LogWarning("Event '" + currentEvent.eventTitle + "' tidak butuh pengorbanan karakter.");
            return false;
        }

        if (character == null || !character.isAlive || character.isMissing)
        {
            Debug.LogWarning("Karakter tidak valid untuk berkorban (mati / sedang Missing).");
            return false;
        }

        // Simpan pending
        pendingEvent = currentEvent;
        pendingEventTarget = character;
        hasPendingEvent = true;

        Debug.Log("========================================");
        Debug.Log("PENDING EVENT SAVED!");
        Debug.Log("Event: " + currentEvent.eventTitle);
        Debug.Log("Target: " + character.characterName);
        Debug.Log("Requirement: " + currentEvent.requiredBodyPart);
        Debug.Log("hasPendingEvent = TRUE");
        Debug.Log("========================================");

        return true;
    }

    // =====================================================
    // EXECUTE PENDING EVENT
    // Dipanggil dari GameManager.ExecuteAllPending()
    // =====================================================

    public void ExecutePendingEvent()
    {
        if (!hasPendingEvent || pendingEvent == null)
        {
            Debug.Log("Tidak ada pending event untuk dieksekusi.");
            return;
        }

        Debug.Log("========================================");
        Debug.Log("EXECUTE PENDING EVENT: " + pendingEvent.eventTitle);

        // Execute berdasarkan requirement type
        if (pendingEvent.requirementType == EventRequirementType.Item)
        {
            ExecuteItemRequirement();
        }
        else if (pendingEvent.requirementType == EventRequirementType.CharacterPart)
        {
            ExecuteCharacterSacrifice();
        }

        // Mark event resolved
        currentEventResolved = true;

        // Clear pending
        ClearPendingEvent();

        Debug.Log("========================================");
    }

    private void ExecuteItemRequirement()
    {
        bool success = false;

        switch (pendingEvent.requiredItem)
        {
            case RequiredItemType.Tools:
                success = ResourceManager.Instance.UseTools(pendingEvent.requiredItemAmount);
                break;

            case RequiredItemType.Knife:
                success = ResourceManager.Instance.UseKnife();
                break;
        }

        if (!success)
        {
            Debug.LogError("GAGAL menggunakan item untuk event! Resource tidak cukup.");
            return;
        }

        ApplyRewards();

        Debug.Log("Event '" + pendingEvent.eventTitle + "' berhasil diselesaikan dengan item.");
    }

    private void ExecuteCharacterSacrifice()
    {
        if (pendingEventTarget == null)
        {
            Debug.LogError("GAGAL execute character sacrifice! Target NULL.");
            return;
        }

        // Apply body part loss
        switch (pendingEvent.requiredBodyPart)
        {
            case BodyPart.Finger:
                pendingEventTarget.missingFinger = true;
                break;

            case BodyPart.Arm:
                pendingEventTarget.missingArm = true;
                break;

            case BodyPart.Leg:
                pendingEventTarget.missingLeg = true;
                break;
        }

        // Apply injury
        pendingEventTarget.isInjured = true;
        pendingEventTarget.injuryStartedToday = true;
        pendingEventTarget.treatmentGiven = false;

        // Apply expedition penalty
        pendingEventTarget.expeditionFailChanceBonus += pendingEvent.expeditionFailChanceBonus;

        if (pendingEvent.disableExpeditionPermanently)
        {
            pendingEventTarget.canExpedition = false;
        }

        ApplyRewards();

        Debug.Log(
            pendingEventTarget.characterName +
            " berkorban (" + pendingEvent.requiredBodyPart + ") untuk event '" +
            pendingEvent.eventTitle + "'."
        );

        Debug.Log(
            "Expedition fail chance bonus: " + pendingEventTarget.expeditionFailChanceBonus + "%"
        );
    }

    // =====================================================
    // REWARD
    // =====================================================

    private void ApplyRewards()
    {
        if (pendingEvent.gainRation > 0)
            ResourceManager.Instance.AddRation(pendingEvent.gainRation);

        if (pendingEvent.gainMedkit > 0)
            ResourceManager.Instance.AddMedkit(pendingEvent.gainMedkit);

        if (pendingEvent.gainTools > 0)
            ResourceManager.Instance.AddTools(pendingEvent.gainTools);

        Debug.Log("Rewards applied from event.");
    }

    // =====================================================
    // CLEAR PENDING
    // =====================================================

    public void ClearPendingEvent()
    {
        pendingEvent = null;
        pendingEventTarget = null;
        hasPendingEvent = false;

        Debug.Log("Pending event cleared.");
    }

    // =====================================================
    // LEGACY METHODS - DEPRECATED
    // Gunakan SavePending* methods untuk flow baru
    // =====================================================

    [System.Obsolete("Use SavePendingItemRequirement() instead")]
    public bool TryResolveItemRequirement()
    {
        return SavePendingItemRequirement();
    }

    [System.Obsolete("Use SavePendingCharacterSacrifice() instead")]
    public bool ResolveCharacterSacrifice(CharacterData character)
    {
        return SavePendingCharacterSacrifice(character);
    }
}