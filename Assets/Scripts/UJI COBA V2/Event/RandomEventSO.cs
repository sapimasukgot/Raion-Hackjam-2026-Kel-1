using UnityEngine;

// =========================================================
// TIPE REQUIREMENT EVENT
// =========================================================

public enum EventRequirementType
{
    None,           // Event cuma informasi, tidak butuh apa-apa
    Item,           // Butuh Tools / Knife (diselesaikan lewat tombol)
    CharacterPart   // Butuh pengorbanan anggota tubuh (diselesaikan lewat drag karakter)
}

public enum RequiredItemType
{
    Tools,
    Knife,
    ToolsOrKnife   // Salah satu boleh, mana yang tersedia dipakai duluan (Tools diutamakan)
}

public enum BodyPart
{
    None,
    Finger, // Jari
    Arm,    // Tangan
    Leg     // Kaki
}

// =========================================================
// KONSEKUENSI JIKA EVENT TIDAK DISELESAIKAN
// =========================================================

public enum EventConsequenceType
{
    None,                           // Tidak ada konsekuensi khusus
    TriggerBadEndingIfUnresolved,   // Langsung Bad Ending saat klik Next Day
    OverrideNextDayEventIfUnresolved // Event besok dipaksa (dengan chance tertentu)
}

// =========================================================
// JENIS RESOURCE UNTUK REWARD RANDOM
// =========================================================

public enum RewardResourceType
{
    Ration,
    Medkit,
    Tools,
    Knife
}

[CreateAssetMenu(fileName = "NewRandomEvent", menuName = "Game/Random Event")]
public class RandomEventSO : ScriptableObject
{
    [Header("Info Event")]
    public string eventTitle;

    [TextArea(3, 10)]
    public string eventDescription;

    public Sprite eventIcon;

    [Header("Probabilitas")]
    [Tooltip("Semakin besar nilainya, semakin sering event ini muncul dibanding event lain di pool yang sama.")]
    [Range(0f, 10f)]
    public float weight = 1f;

    // =====================================================
    // REQUIREMENT (SYARAT PENYELESAIAN)
    // =====================================================

    [Header("Requirement")]
    public EventRequirementType requirementType = EventRequirementType.None;

    [Tooltip("Dipakai kalau Requirement Type = Item")]
    public RequiredItemType requiredItem = RequiredItemType.Tools;

    [Tooltip("Jumlah item yang dibutuhkan (khusus Tools, Knife selalu 1)")]
    public int requiredItemAmount = 1;

    [Tooltip("Khusus kalau Required Item = Knife (atau ToolsOrKnife dan yang dipakai Knife). Centang = pisau habis/hilang setelah dipakai. Kosongkan = pisau balik lagi setelah dipakai (misal untuk event Hunting).")]
    public bool consumeKnifeOnUse = true;

    [Tooltip("Dipakai kalau Requirement Type = CharacterPart. Karakter mana yang berkorban ditentukan lewat drag di game, bukan di sini.")]
    public BodyPart requiredBodyPart = BodyPart.None;

    // =====================================================
    // REWARD / EFEK JIKA BERHASIL DISELESAIKAN
    // =====================================================

    [Header("Reward Jika Berhasil Diselesaikan")]
    public int gainRation = 0;
    public int gainMedkit = 0;
    public int gainTools = 0;

    [Header("Reward Random (khusus event pengorbanan tubuh - Jari/Tangan/Kaki)")]
    [Tooltip("Kalau dicentang, reward di atas (gainRation/gainMedkit/gainTools) DIABAIKAN. Sistem akan random jumlah (1: 70%, 2: 29%, 3: 1%) dan random jenis supply (Ration/Medkit/Tools/Knife, masing-masing 25%).")]
    public bool useRandomSacrificeReward = false;

    [Header("Efek Khusus Untuk Pengorbanan Karakter")]
    [Tooltip("Ditambahkan ke peluang gagal ekspedisi milik karakter yang berkorban (dalam %, misal isi 10 = +10%).")]
    public float expeditionFailChanceBonus = 0f;

    [Tooltip("Kalau dicentang, karakter yang berkorban PERMANEN tidak bisa ikut ekspedisi lagi (dipakai untuk kehilangan kaki).")]
    public bool disableExpeditionPermanently = false;

    [Header("Efek Khusus: Trigger Bad Ending Langsung Saat Dikorbankan")]
    [Tooltip("Kalau dicentang: begitu pemain drag karakter untuk berkorban di event ini, Bad Ending LANGSUNG dipicu saat itu juga (body part loss & reward di atas diabaikan). Kalau pemain TIDAK melakukan apa-apa dan langsung klik Next Day, event ini tidak berefek sama sekali (lanjut normal). Cocok untuk event pilihan di hari terakhir.")]
    public bool sacrificeTriggersBadEnding = false;

    // =====================================================
    // KONSEKUENSI JIKA TIDAK DISELESAIKAN
    // =====================================================

    [Header("Konsekuensi Jika TIDAK Diselesaikan")]
    [Tooltip("Dicek saat tombol Next Day diklik. Kalau event ini masih aktif & belum diselesaikan, konsekuensi ini akan berjalan.")]
    public EventConsequenceType consequenceType = EventConsequenceType.None;

    [Header("↳ Khusus: Override Next Day Event")]
    [Tooltip("Dipakai kalau consequenceType = OverrideNextDayEventIfUnresolved. Event yang dipaksa muncul BESOK, menggantikan pool random report besok.")]
    public RandomEventSO overrideForceEvent;

    [Range(0f, 100f)]
    [Tooltip("Persen peluang (0-100) forcedEvent yang muncul besok. Sisanya (100 - nilai ini) jadi peluang Game Over instan. Contoh: isi 99 → 99% forcedEvent, 1% Game Over.")]
    public float overrideForceEventChance = 99f;
}