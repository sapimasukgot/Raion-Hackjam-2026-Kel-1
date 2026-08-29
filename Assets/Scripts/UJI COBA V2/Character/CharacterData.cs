[System.Serializable]
public class CharacterData
{
    public string characterName;

    // =====================================================
    // STATUS DASAR
    // =====================================================

    public bool isAlive = true;
    public bool isMissing = false;

    // =====================================================
    // KONDISI
    // =====================================================

    public bool isHungry = false;
    public bool isInjured = false;

    // Treatment berhasil pada hari ini
    public bool treatmentGiven = false;

    // Injury baru terjadi pada hari ini
    public bool injuryStartedToday = false;

    // =====================================================
    // HUNGER
    // =====================================================

    public HungerState hungerState = HungerState.Normal;

    // =====================================================
    // BAGIAN TUBUH
    // Permanent state
    // =====================================================

    public bool missingFinger = false;
    public bool missingArm = false;
    public bool missingLeg = false;

    // =====================================================
    // EKSPEDISI
    // =====================================================

    public bool canExpedition = true;
    public bool isExpedition = false; // Sedang pergi ekspedisi
    public bool expeditionHasKnife = false; // Bawa pisau saat pergi?

    public float expeditionFailChanceBonus = 0f;

    // =====================================================
    // MISSING
    // =====================================================

    public int missingDays = 0;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public CharacterData(string name)
    {
        characterName = name;

        isAlive = true;
        isMissing = false;

        isHungry = false;
        isInjured = false;

        treatmentGiven = false;
        injuryStartedToday = false;

        hungerState = HungerState.Normal;

        missingFinger = false;
        missingArm = false;
        missingLeg = false;

        canExpedition = true;
        isExpedition = false;
        expeditionHasKnife = false;
        expeditionFailChanceBonus = 0f;

        missingDays = 0;
    }
}


// =========================================================
// HUNGER STATE
// =========================================================

public enum HungerState
{
    Normal,
    Hungry,
    Starving,
    Dead
}