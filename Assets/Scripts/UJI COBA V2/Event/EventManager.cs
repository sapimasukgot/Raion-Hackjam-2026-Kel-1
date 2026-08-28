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

    [Header("Override Event Besok (dari konsekuensi event yang tidak diselesaikan)")]
    public bool overrideNextDayEvent = false;
    public RandomEventSO overrideForcedEvent;
    public float overrideForcedEventChance = 99f;

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

    public bool ExecutePendingEvent()
    {
        if (!hasPendingEvent || pendingEvent == null)
        {
            Debug.Log("Tidak ada pending event untuk dieksekusi.");
            return false;
        }

        Debug.Log("========================================");
        Debug.Log("EXECUTE PENDING EVENT: " + pendingEvent.eventTitle);

        bool gameOverTriggered = false;

        // Execute berdasarkan requirement type
        bool itemSuccess = true;

        if (pendingEvent.requirementType == EventRequirementType.Item)
        {
            itemSuccess = ExecuteItemRequirement();
        }
        else if (pendingEvent.requirementType == EventRequirementType.CharacterPart)
        {
            gameOverTriggered = ExecuteCharacterSacrifice();
        }

        currentEventResolved = itemSuccess; // hanya resolved kalau memang berhasil

        // Clear pending
        ClearPendingEvent();

        Debug.Log("========================================");

        return gameOverTriggered;
    }

    private bool ExecuteItemRequirement()
{
    bool success = false;

    switch (pendingEvent.requiredItem)
    {
        case RequiredItemType.Tools:
            success = ResourceManager.Instance.UseTools(pendingEvent.requiredItemAmount);
            break;

        case RequiredItemType.Knife:
            success = ResourceManager.Instance.UseKnife();

            // Kalau event ini tidak menghabiskan knife (misal Hunting),
            // langsung balikin lagi setelah dipakai.
            if (success && !pendingEvent.consumeKnifeOnUse)
            {
                ResourceManager.Instance.AddKnife();

                Debug.Log("Knife dipakai untuk '" + pendingEvent.eventTitle + "' lalu dikembalikan lagi (tidak habis).");
            }
            break;
    }

    if (!success)
    {
        Debug.LogError("GAGAL menggunakan item untuk event! Resource tidak cukup.");
        return false; 
    }

    ApplyRewards();

    Debug.Log("Event '" + pendingEvent.eventTitle + "' berhasil diselesaikan dengan item.");

    return true;  
}

    private bool ExecuteCharacterSacrifice()
    {
        if (pendingEventTarget == null)
        {
            Debug.LogError("GAGAL execute character sacrifice! Target NULL.");
            return false;
        }

        // ================================================
        // KASUS KHUSUS: event ini langsung trigger Bad Ending
        // saat dikorbankan (misal event pilihan hari terakhir).
        // Tidak ada body part loss / reward biasa untuk kasus ini.
        // ================================================

        if (pendingEvent.sacrificeTriggersBadEnding)
        {
            Debug.Log(
                pendingEventTarget.characterName +
                " dikorbankan untuk event '" + pendingEvent.eventTitle +
                "' → Bad Ending dipicu langsung."
            );

            if (EndingManager.Instance != null)
                EndingManager.Instance.TriggerBadEndingForced();

            return true; // beritahu GameManager: game over sudah terpicu
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

        return false; // tidak trigger game over
    }

    // =====================================================
    // REWARD
    // =====================================================

    private void ApplyRewards()
    {
        // Reward random khusus event pengorbanan (Jari/Tangan/Kaki)
        if (pendingEvent.useRandomSacrificeReward)
        {
            ApplyRandomSacrificeReward();
            return;
        }

        if (pendingEvent.gainRation > 0)
            ResourceManager.Instance.AddRation(pendingEvent.gainRation);

        if (pendingEvent.gainMedkit > 0)
            ResourceManager.Instance.AddMedkit(pendingEvent.gainMedkit);

        if (pendingEvent.gainTools > 0)
            ResourceManager.Instance.AddTools(pendingEvent.gainTools);

        Debug.Log("Rewards applied from event.");
    }

    // =====================================================
    // REWARD RANDOM - KHUSUS PENGORBANAN TUBUH
    // Jumlah: 1 (70%) / 2 (29%) / 3 (1%)
    // Jenis: Ration / Medkit / Tools / Knife (masing-masing 25%)
    // =====================================================

    private void ApplyRandomSacrificeReward()
    {
        int amount = RollRewardAmount();
        RewardResourceType type = RollRewardType();

        switch (type)
        {
            case RewardResourceType.Ration:
                ResourceManager.Instance.AddRation(amount);
                break;

            case RewardResourceType.Medkit:
                ResourceManager.Instance.AddMedkit(amount);
                break;

            case RewardResourceType.Tools:
                ResourceManager.Instance.AddTools(amount);
                break;

            case RewardResourceType.Knife:
                // Knife itu bool (punya/tidak), jumlah tidak berlaku di sini.
                ResourceManager.Instance.AddKnife();
                break;
        }

        Debug.Log(
            "Random Sacrifice Reward → " +
            (type == RewardResourceType.Knife ? "1x Knife" : amount + "x " + type)
        );
    }

    private int RollRewardAmount()
    {
        float roll = Random.Range(0f, 100f);

        if (roll < 70f) return 1;   // 0 - 70%
        if (roll < 99f) return 2;   // 70 - 99%
        return 3;                   // 99 - 100% (sisa 1%)
    }

    private RewardResourceType RollRewardType()
    {
        float roll = Random.Range(0f, 100f);

        if (roll < 25f) return RewardResourceType.Ration;
        if (roll < 50f) return RewardResourceType.Medkit;
        if (roll < 75f) return RewardResourceType.Tools;
        return RewardResourceType.Knife;
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
    // KONSEKUENSI JIKA EVENT TIDAK DISELESAIKAN
    // Panggil ini SETELAH ExecutePendingEvent() di GameManager.NextDay(),
    // karena currentEventResolved baru valid setelah itu.
    // =====================================================

    /// <summary>
    /// Return true kalau konsekuensinya langsung memicu Game Over
    /// (GameManager wajib "return" segera setelah ini kalau hasilnya true).
    /// </summary>
    public bool ApplyUnresolvedEventConsequence()
    {
        if (currentEvent == null || currentEventResolved)
            return false;

        switch (currentEvent.consequenceType)
        {
            case EventConsequenceType.TriggerBadEndingIfUnresolved:

                Debug.Log(
                    "Event '" + currentEvent.eventTitle +
                    "' TIDAK diselesaikan → Bad Ending dipicu."
                );

                if (EndingManager.Instance != null)
                    EndingManager.Instance.TriggerBadEndingForced();

                return true;

            case EventConsequenceType.OverrideNextDayEventIfUnresolved:

                Debug.Log(
                    "Event '" + currentEvent.eventTitle +
                    "' TIDAK diselesaikan → event besok akan di-override."
                );

                overrideNextDayEvent = true;
                overrideForcedEvent = currentEvent.overrideForceEvent;
                overrideForcedEventChance = currentEvent.overrideForceEventChance;

                return false;

            default:
                return false;
        }
    }

    // =====================================================
    // KONSUMSI OVERRIDE EVENT BESOK
    // Dipanggil dari ReportUIController saat memilih event untuk hari baru.
    // =====================================================

    /// <summary>
    /// Kalau ada override aktif dari kemarin, roll chance-nya di sini (sekali pakai).
    /// - forcedEvent terisi kalau roll masuk ke persentase forced event.
    /// - gameOverTriggered = true kalau roll masuk ke sisa persentase (Game Over instan).
    /// Return false kalau memang tidak ada override yang perlu dikonsumsi.
    /// </summary>
    public bool TryConsumeOverrideEvent(out RandomEventSO forcedEvent, out bool gameOverTriggered)
    {
        forcedEvent = null;
        gameOverTriggered = false;

        if (!overrideNextDayEvent)
            return false;

        // Konsumsi sekali pakai
        overrideNextDayEvent = false;

        float roll = Random.Range(0f, 100f);

        Debug.Log("Override Next Day Event → roll: " + roll + " / chance forced event: " + overrideForcedEventChance + "%");

        if (roll < overrideForcedEventChance)
        {
            forcedEvent = overrideForcedEvent;

            Debug.Log("Override hasil: FORCED EVENT → " + (forcedEvent != null ? forcedEvent.eventTitle : "NULL"));
        }
        else
        {
            gameOverTriggered = true;

            Debug.Log("Override hasil: GAME OVER (masuk sisa persentase).");

            if (EndingManager.Instance != null)
                EndingManager.Instance.TriggerBadEndingForced();
        }

        return true;
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