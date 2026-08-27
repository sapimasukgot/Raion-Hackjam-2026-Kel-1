using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDailyReport", menuName = "Game/Daily Report")]
public class DailyReportSO : ScriptableObject
{
    [Header("Ringkasan (bagian atas)")]
    [TextArea(3, 8)]
    public string summaryText;

    [Header("Pool Event Random (bagian tengah/bawah)")]
    public List<RandomEventSO> possibleEvents;

    [Header("Opsional")]
    [Tooltip("Kalau dicentang, ada kemungkinan TIDAK ada event sama sekali yang muncul.")]
    public bool canHaveNoEvent = false;

    [Range(0f, 1f)]
    [Tooltip("Peluang kosong (tidak ada event) jika canHaveNoEvent = true. 0 = selalu ada event, 1 = selalu kosong.")]
    public float noEventChance = 0.2f;

    /// <summary>
    /// Pilih satu event secara random dari pool berdasarkan weight.
    /// Return null kalau memang tidak ada event yang muncul (jika canHaveNoEvent aktif dan roll-nya kosong),
    /// atau kalau pool memang kosong.
    /// </summary>
    public RandomEventSO GetRandomEvent()
    {
        return GetRandomEvent(false);
    }

    /// <summary>
    /// Sama seperti GetRandomEvent(), tapi bisa dipaksa untuk TIDAK memunculkan event sama sekali.
    /// Dipakai misalnya untuk hari-hari tertentu (hari ke-7/8) yang memang tidak boleh ada random event.
    /// </summary>
    public RandomEventSO GetRandomEvent(bool forceNoEvent)
    {
        if (forceNoEvent)
            return null;

        if (possibleEvents == null || possibleEvents.Count == 0)
            return null;

        if (canHaveNoEvent && Random.value < noEventChance)
            return null;

        float totalWeight = 0f;
        foreach (var e in possibleEvents)
        {
            if (e != null) totalWeight += Mathf.Max(0f, e.weight);
        }

        if (totalWeight <= 0f)
            return possibleEvents[Random.Range(0, possibleEvents.Count)];

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in possibleEvents)
        {
            if (e == null) continue;
            cumulative += Mathf.Max(0f, e.weight);
            if (roll <= cumulative)
                return e;
        }

        return possibleEvents[possibleEvents.Count - 1];
    }
}