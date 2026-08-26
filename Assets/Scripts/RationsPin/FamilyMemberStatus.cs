using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class NeedStatusChangedEvent : UnityEvent<NeedType, bool> { }

/// <summary>
/// Tempel 1 komponen ini di tiap anggota keluarga (Bapak, Ibu, dst).
/// Nyimpen kebutuhan mana aja yang lagi aktif (butuh dipenuhi) buat orang itu.
/// Script ini generic - sama persis dipakai ulang untuk semua anggota keluarga,
/// yang beda cuma data "activeNeeds" per instance-nya.
/// </summary>
public class FamilyMemberStatus : MonoBehaviour
{
    [Header("Identity")]
    public string memberName = "Bapak";

    [Header("Kebutuhan yang aktif saat ini (mis. lapar = true)")]
    [SerializeField]
    private List<NeedType> initialActiveNeeds = new List<NeedType> { NeedType.Food };

    private readonly HashSet<NeedType> activeNeeds = new HashSet<NeedType>();

    [Header("Events")]
    public NeedStatusChangedEvent OnNeedStatusChanged; // (needType, isActiveSekarang)

    private void Awake()
    {
        foreach (var need in initialActiveNeeds)
            activeNeeds.Add(need);
    }

    public bool IsNeedActive(NeedType type)
    {
        return activeNeeds.Contains(type);
    }

    /// <summary>
    /// Dipanggil saat pin berhasil di-drop ke NeedZone yang cocok.
    /// Misal Fulfill(NeedType.Food) -> status lapar hilang, jadi kenyang.
    /// </summary>
    public void Fulfill(NeedType type)
    {
        if (!activeNeeds.Contains(type)) return;

        activeNeeds.Remove(type);
        OnNeedStatusChanged?.Invoke(type, false);

        Debug.Log($"{memberName} : {type} sudah terpenuhi.");
    }

    /// <summary>
    /// Buat munculin kebutuhan baru lagi nanti (misal lapar lagi setelah beberapa waktu).
    /// </summary>
    public void TriggerNeed(NeedType type)
    {
        if (activeNeeds.Contains(type)) return;

        activeNeeds.Add(type);
        OnNeedStatusChanged?.Invoke(type, true);
    }
}
