using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryUIManager : MonoBehaviour
{
    [Header("UI Component References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI storyText;
    public Transform choiceButtonContainer; // Tempat meletakkan tombol pilihan
    public GameObject buttonPrefab;          // Prefab Button UI standar

    [Header("Event Database")]
    public DiaryEvent day1Event;             // Masukkan 'Event_Day1_Intro' di sini
    public DiaryEvent day7Event;             // Masukkan 'Event_MintaOrang' (Ritual Day 7) di sini
    public List<DiaryEvent> randomEventPool;  // Masukkan Event acak lainnya (Day 2-6) di sini

    [Header("Game State")]
    public int currentDay = 1;
    public int maxDay = 7;                   // Batas akhir game hanya sampai Day 7
    
    private DiaryEvent currentActiveEvent;
    private List<DiaryEvent> availableRandomEvents = new List<DiaryEvent>();

    private void Start()
    {
        // Inisialisasi pool event acak
        InitRandomPool();

        // Tampilkan event hari saat ini (mulai dari Day 1)
        LoadEventForCurrentDay();
    }

    private void InitRandomPool()
    {
        availableRandomEvents = new List<DiaryEvent>();
        if (randomEventPool != null)
        {
            availableRandomEvents.AddRange(randomEventPool);
        }
    }

    // Fungsi utama untuk memuat event berdasarkan hari
    public void LoadEventForCurrentDay()
    {
        if (currentDay == 1)
        {
            // HARI 1: Intro Cerita
            if (day1Event != null)
            {
                LoadEvent(day1Event);
            }
            else
            {
                Debug.LogWarning("Event Day 1 belum di-assign di Inspector!");
            }
        }
        else if (currentDay >= maxDay)
        {
            // HARI 7: Ritual Puncak Minta Orang
            currentDay = maxDay;
            if (day7Event != null)
            {
                LoadEvent(day7Event);
            }
            else
            {
                Debug.LogWarning("Event Day 7 (Minta Orang) belum di-assign di Inspector!");
            }
        }
        else
        {
            // HARI 2 s/d 6: Event Acak dari Pool (tanpa duplikat berulang jika pool cukup)
            if (availableRandomEvents.Count == 0 && randomEventPool != null && randomEventPool.Count > 0)
            {
                InitRandomPool();
            }

            if (availableRandomEvents.Count > 0)
            {
                int randomIndex = Random.Range(0, availableRandomEvents.Count);
                currentActiveEvent = availableRandomEvents[randomIndex];
                availableRandomEvents.RemoveAt(randomIndex); // Hapus agar tidak muncul dua kali di rentang Day 2-6
                LoadEvent(currentActiveEvent);
            }
            else
            {
                Debug.LogWarning("Random Event Pool kosong! Masukkan Event ScriptableObject ke Random Event Pool.");
            }
        }
    }

    // Lanjut ke hari berikutnya (Maksimal Day 7)
    public void NextDay()
    {
        if (currentDay >= maxDay)
        {
            Debug.Log("Sudah mencapai batas hari terakhir (Day " + maxDay + ")!");
            return;
        }

        currentDay++;
        LoadEventForCurrentDay();
    }

    // Kompatibilitas fungsi jika dipanggil dari tempat lain
    public void DisplayNextRandomDay()
    {
        NextDay();
    }

    private void LoadEvent(DiaryEvent diaryEvent)
    {
        currentActiveEvent = diaryEvent;

        // 1. Ubah Teks Hari & Teks Cerita
        if (dayText != null) dayText.text = "HARI " + currentDay;
        if (storyText != null) storyText.text = diaryEvent.narasiKejadian;

        // 2. Bersihkan Tombol Pilihan Lama
        if (choiceButtonContainer != null)
        {
            foreach (Transform child in choiceButtonContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // 3. Generate Tombol Pilihan Secara Dinamis
        if (diaryEvent.choices != null && choiceButtonContainer != null && buttonPrefab != null)
        {
            for (int i = 0; i < diaryEvent.choices.Length; i++)
            {
                ChoiceData choice = diaryEvent.choices[i];

                // Spawn Prefab Tombol di Container
                GameObject newBtn = Instantiate(buttonPrefab, choiceButtonContainer);
                
                // Set Teks Tombol
                TextMeshProUGUI btnText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.buttonLabel;

                // Tambahkan Fungsi Klik ke Tombol
                Button btnComponent = newBtn.GetComponent<Button>();
                if (btnComponent != null)
                {
                    btnComponent.onClick.AddListener(() => OnChoiceClicked(choice));
                }
            }
        }
    }

    private void OnChoiceClicked(ChoiceData chosenChoice)
    {
        Debug.Log($"[Day {currentDay}] Pilihan diambil: {chosenChoice.buttonLabel}");

        // --- MASUKKAN LOGIKA DAMPAK RESOURCE DI SINI ---
        // Contoh: GameStats.Rations += chosenChoice.rationChange;
        
        if (chosenChoice.triggersGameOver)
        {
            Debug.Log("GAME OVER: Nyawa dikorbankan.");
            return;
        }

        // Cek jika sudah selesai di Day 7
        if (currentDay >= maxDay)
        {
            Debug.Log("SELAMAT! Anda berhasil bertahan hidup sampai akhir Day 7!");
            // Tambahkan logika game selesai / ending screen di sini
            return;
        }

        // Lanjut ke Hari Berikutnya
        NextDay();
    }
}