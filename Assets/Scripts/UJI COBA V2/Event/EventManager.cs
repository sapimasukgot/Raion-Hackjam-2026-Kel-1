using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Event Aktif Hari Ini")]
    public RandomEventSO currentEvent;
    public bool currentEventResolved = false;

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

        Debug.Log(
            currentEvent != null
                ? "Event aktif: " + currentEvent.eventTitle
                : "Tidak ada event aktif hari ini."
        );
    }

    public bool IsEventActive()
    {
        return currentEvent != null && !currentEventResolved;
    }

    // =====================================================
    // RESOLVE - ITEM REQUIREMENT (Tools / Knife)
    // =====================================================

    /// <summary>
    /// Panggil ini dari tombol UI (misal "Perbaiki" / "Gunakan Item") untuk event
    /// yang requirement-nya berupa item (Tools/Knife).
    /// </summary>
    public bool TryResolveItemRequirement()
    {
        if (!IsEventActive())
        {
            Debug.LogWarning("Tidak ada event aktif untuk diselesaikan.");
            return false;
        }

        if (currentEvent.requirementType != EventRequirementType.Item)
        {
            Debug.LogWarning("Event '" + currentEvent.eventTitle + "' tidak butuh item.");
            return false;
        }

        bool success = false;

        switch (currentEvent.requiredItem)
        {
            case RequiredItemType.Tools:
                success = ResourceManager.Instance.UseTools(currentEvent.requiredItemAmount);
                break;

            case RequiredItemType.Knife:
                success = ResourceManager.Instance.UseKnife();
                break;
        }

        if (!success)
        {
            Debug.Log("Gagal menyelesaikan event '" + currentEvent.eventTitle + "' → item tidak cukup.");
            return false;
        }

        ApplyRewards();

        currentEventResolved = true;

        Debug.Log("Event '" + currentEvent.eventTitle + "' berhasil diselesaikan dengan item.");

        return true;
    }

    // =====================================================
    // RESOLVE - CHARACTER SACRIFICE (Jari / Tangan / Kaki)
    // =====================================================

    /// <summary>
    /// Dipanggil dari DropZone (tipe EventSacrifice) saat player men-drag karakter
    /// ke drop zone pengorbanan untuk menyelesaikan event.
    /// </summary>
    public bool ResolveCharacterSacrifice(CharacterData character)
    {
        if (!IsEventActive())
        {
            Debug.LogWarning("Tidak ada event aktif untuk diselesaikan.");
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

        switch (currentEvent.requiredBodyPart)
        {
            case BodyPart.Finger:
                character.missingFinger = true;
                break;

            case BodyPart.Arm:
                character.missingArm = true;
                break;

            case BodyPart.Leg:
                character.missingLeg = true;
                break;
        }

        character.expeditionFailChanceBonus += currentEvent.expeditionFailChanceBonus;

        if (currentEvent.disableExpeditionPermanently)
        {
            character.canExpedition = false;
        }

        ApplyRewards();

        currentEventResolved = true;

        Debug.Log(
            character.characterName +
            " berkorban (" + currentEvent.requiredBodyPart + ") untuk event '" +
            currentEvent.eventTitle + "'. " +
            "Bonus gagal ekspedisi sekarang: " + character.expeditionFailChanceBonus + "%"
        );

        return true;
    }

    // =====================================================
    // REWARD
    // =====================================================

    private void ApplyRewards()
    {
        if (currentEvent.gainRation > 0)
            ResourceManager.Instance.AddRation(currentEvent.gainRation);

        if (currentEvent.gainMedkit > 0)
            ResourceManager.Instance.AddMedkit(currentEvent.gainMedkit);

        if (currentEvent.gainTools > 0)
            ResourceManager.Instance.AddTools(currentEvent.gainTools);
    }
}