using UnityEngine;
using TMPro; // Wajib untuk TextMeshPro

public class SimpleDayManager : MonoBehaviour
{
    [Header("Data Hari")]
    public int currentDay = 1;
    public TMP_Text dayText; // Drag teks "Hari 1" ke sini

    private void Start()
    {
        UpdateDayUI();
    }

    // Fungsi ini dipanggil saat Tombol Next Day diklik
    public void NextDay()
    {
        currentDay++; // Nambah 1 hari
        UpdateDayUI();
        Debug.Log("Hari berganti ke: " + currentDay);
    }

    private void UpdateDayUI()
    {
        if (dayText != null)
        {
            dayText.text = $"HARI {currentDay}";
        }
    }
}   