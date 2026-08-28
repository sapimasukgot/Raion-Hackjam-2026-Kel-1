using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReportUIController : MonoBehaviour
{
    public static ReportUIController Instance;

    [Header("Data")]
    public DailyReportSO currentReport;

    [Header("Hari Tanpa Event Random")]
    [Tooltip("Di hari-hari ini, section event TIDAK akan muncul sama sekali, walaupun report punya pool event.")]
    public int[] noEventDays = { 7, 8 };

    [Header("UI - Ringkasan")]
    public TMP_Text summaryTextUI;

    [Header("UI - Section Event")]
    public GameObject eventSectionParent; // GameObject pembungkus section event, di-enable/disable
    public TMP_Text eventTitleUI;
    public TMP_Text eventDescUI;
    public Image eventIconUI;

    [Header("UI - Requirement Item (opsional)")]
    [Tooltip("Muncul kalau event butuh item (Tools/Knife). Sembunyi kalau requirement-nya CharacterPart atau None.")]
    public GameObject itemRequirementUI;
    public TMP_Text itemRequirementLabel;
    public Button resolveItemButton;

    private void Awake()
    {
        Instance = this;

        if (resolveItemButton != null)
            resolveItemButton.onClick.AddListener(OnResolveItemButtonClicked);
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            ShowReportForDay(GameManager.Instance.currentDay);
        }
        else if (currentReport != null)
        {
            ShowReport(currentReport, 1);
        }
    }

    /// <summary>
    /// Ambil DailyReportSO dari GameManager sesuai nomor hari, lalu tampilkan.
    /// Panggil ini setiap kali hari berganti (misal dari GameManager.NextDay()).
    /// </summary>
    public void ShowReportForDay(int day)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager belum ada, tidak bisa ambil report untuk hari ini.");
            return;
        }

        DailyReportSO report = GameManager.Instance.GetReportForDay(day);

        if (report == null)
        {
            Debug.LogWarning("Tidak ada DailyReportSO untuk Day " + day + ". Cek list 'Daily Reports' di GameManager.");
            return;
        }

        ShowReport(report, day);
    }

    /// <summary>
    /// Tampilkan report tertentu, dengan mempertimbangkan apakah hari ini termasuk hari "tanpa event".
    /// </summary>
    public void ShowReport(DailyReportSO report, int day)
    {
        if (report == null)
        {
            Debug.LogWarning("DailyReportSO kosong / belum di-assign.");
            return;
        }

        currentReport = report;

        // Tampilkan ringkasan
        if (summaryTextUI != null)
            summaryTextUI.text = report.summaryText;

        bool forceNoEvent = IsNoEventDay(day);

        // Ambil event random dari pool (atau dipaksa null kalau hari ini termasuk noEventDays)
        RandomEventSO chosenEvent = report.GetRandomEvent(forceNoEvent);

        // Daftarkan event ini sebagai event aktif di EventManager
        // (dipakai DropZone.EventSacrifice & tombol resolve item untuk tahu event apa yang harus diselesaikan)
        if (EventManager.Instance != null)
            EventManager.Instance.SetCurrentEvent(chosenEvent);

        if (chosenEvent != null)
        {
            if (eventSectionParent != null) eventSectionParent.SetActive(true);
            if (eventTitleUI != null) eventTitleUI.text = chosenEvent.eventTitle;
            if (eventDescUI != null) eventDescUI.text = chosenEvent.eventDescription;
            if (eventIconUI != null) eventIconUI.sprite = chosenEvent.eventIcon;

            RefreshRequirementUI(chosenEvent);
        }
        else
        {
            // Tidak ada event yang muncul -> sembunyikan section event
            if (eventSectionParent != null) eventSectionParent.SetActive(false);
            if (itemRequirementUI != null) itemRequirementUI.SetActive(false);
        }
    }

    // =====================================================
    // REQUIREMENT UI (KHUSUS ITEM)
    // =====================================================

    private void RefreshRequirementUI(RandomEventSO ev)
    {
        if (itemRequirementUI == null)
            return;

        bool isItemRequirement = ev.requirementType == EventRequirementType.Item;

        itemRequirementUI.SetActive(isItemRequirement);

        if (!isItemRequirement)
            return;

        string itemName = ev.requiredItem == RequiredItemType.Tools ? "Tools" : "Knife";

        if (itemRequirementLabel != null)
        {
            itemRequirementLabel.text = ev.requiredItem == RequiredItemType.Tools
                ? "Butuh " + ev.requiredItemAmount + " " + itemName
                : "Butuh " + itemName;
        }
    }

    // Dipanggil oleh tombol "Selesaikan" untuk event bertipe Item (Tools/Knife)
    public void OnResolveItemButtonClicked()
    {
        if (EventManager.Instance == null)
            return;

        // GUNAKAN METHOD BARU - SAVE PENDING
        bool saved = EventManager.Instance.SavePendingItemRequirement();

        Debug.Log("Item requirement button clicked. Saved: " + saved);

        if (saved && itemRequirementUI != null)
        {
            itemRequirementUI.SetActive(false);
        }
    }

    private bool IsNoEventDay(int day)
    {
        if (noEventDays == null) return false;

        foreach (int d in noEventDays)
        {
            if (d == day) return true;
        }

        return false;
    }

    // Panggil ini dari tombol "Next" misalnya, untuk reroll event tanpa ganti report/hari
    public void RerollEvent()
    {
        if (currentReport != null && GameManager.Instance != null)
            ShowReport(currentReport, GameManager.Instance.currentDay);
    }
}