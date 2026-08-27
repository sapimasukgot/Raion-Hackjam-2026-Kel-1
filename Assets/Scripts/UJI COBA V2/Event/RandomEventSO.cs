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
    Knife
}

public enum BodyPart
{
    None,
    Finger, // Jari
    Arm,    // Tangan
    Leg     // Kaki
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

    [Tooltip("Dipakai kalau Requirement Type = CharacterPart. Karakter mana yang berkorban ditentukan lewat drag di game, bukan di sini.")]
    public BodyPart requiredBodyPart = BodyPart.None;

    // =====================================================
    // REWARD / EFEK JIKA BERHASIL DISELESAIKAN
    // =====================================================

    [Header("Reward Jika Berhasil Diselesaikan")]
    public int gainRation = 0;
    public int gainMedkit = 0;
    public int gainTools = 0;

    [Header("Efek Khusus Untuk Pengorbanan Karakter")]
    [Tooltip("Ditambahkan ke peluang gagal ekspedisi milik karakter yang berkorban (dalam %, misal isi 10 = +10%).")]
    public float expeditionFailChanceBonus = 0f;

    [Tooltip("Kalau dicentang, karakter yang berkorban PERMANEN tidak bisa ikut ekspedisi lagi (dipakai untuk kehilangan kaki).")]
    public bool disableExpeditionPermanently = false;
}