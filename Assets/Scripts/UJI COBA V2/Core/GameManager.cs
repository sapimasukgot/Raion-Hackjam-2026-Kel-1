using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // =====================================================
    // DAY
    // =====================================================

    [Header("Day")]
    public int currentDay = 1;

    // =====================================================
    // RESOURCES
    // =====================================================

    [Header("Resources")]
    public int ration = 10;
    public int medkit = 3;
    public int tools = 2;
    public bool knife = true;

    // =====================================================
    // FAMILY
    // =====================================================

    [Header("Family")]
    public CharacterData dad;
    public CharacterData mom;
    public CharacterData son;
    public CharacterData daughter;

    // =====================================================
    // MANAGERS
    // =====================================================

    [Header("Managers")]
    public FamilyManager familyManager;
    public ReportUIController reportUIController;

    // =====================================================
    // DAILY REPORTS
    // =====================================================

    [Header("Daily Reports")]
    public System.Collections.Generic.List<DailyReportSO> dailyReports;

    // =====================================================
    // PENDING FEEDING
    // =====================================================

    [Header("Pending Feeding")]
    public bool pendingFeedDad = false;
    public bool pendingFeedMom = false;
    public bool pendingFeedSon = false;
    public bool pendingFeedDaughter = false;

    // =====================================================
    // PENDING TREATMENT
    // =====================================================

    [Header("Pending Treatment")]
    public bool pendingTreatDad = false;
    public bool pendingTreatMom = false;
    public bool pendingTreatSon = false;
    public bool pendingTreatDaughter = false;

    // =====================================================
    // PENDING EXIT
    // =====================================================

    [Header("Pending Exit")]
    public bool pendingExitDad = false;
    public bool pendingExitMom = false;
    public bool pendingExitSon = false;
    public bool pendingExitDaughter = false;

    // =====================================================
    // PENDING SACRIFICE
    // =====================================================

    [Header("Pending Sacrifice")]
    public bool pendingSacrificeDad = false;
    public bool pendingSacrificeMom = false;
    public bool pendingSacrificeSon = false;
    public bool pendingSacrificeDaughter = false;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        InitializeGame();
    }


    // =====================================================
    // INITIALIZE
    // =====================================================

    private void InitializeGame()
    {
        dad = new CharacterData("Dad");
        mom = new CharacterData("Mom");
        son = new CharacterData("Son");
        daughter = new CharacterData("Daughter");

        // TEST SAJA
       
    }


    // =====================================================
    // DAILY REPORT LOOKUP
    // =====================================================

    /// <summary>
    /// Ambil DailyReportSO sesuai nomor hari (day 1 = index 0).
    /// Kalau hari melebihi jumlah report yang di-set, otomatis pakai report terakhir yang ada
    /// (biar tidak error kalau lupa isi report untuk hari-hari jauh ke depan).
    /// </summary>
    public DailyReportSO GetReportForDay(int day)
    {
        if (dailyReports == null || dailyReports.Count == 0)
        {
            Debug.LogWarning("List 'Daily Reports' di GameManager masih kosong!");
            return null;
        }

        int index = day - 1;

        if (index < 0)
            index = 0;

        if (index >= dailyReports.Count)
            index = dailyReports.Count - 1;

        return dailyReports[index];
    }


    // =====================================================
    // FEEDING
    // =====================================================

    public void SetPendingFeeding(CharacterData character)
    {
        if (character == null)
            return;

        if (character == dad)
        {
            pendingFeedDad = true;
            Debug.Log("Pending Feeding → Dad");
        }
        else if (character == mom)
        {
            pendingFeedMom = true;
            Debug.Log("Pending Feeding → Mom");
        }
        else if (character == son)
        {
            pendingFeedSon = true;
            Debug.Log("Pending Feeding → Son");
        }
        else if (character == daughter)
        {
            pendingFeedDaughter = true;
            Debug.Log("Pending Feeding → Daughter");
        }
    }


    public void ProcessPendingFeeding()
    {
        if (familyManager == null)
        {
            Debug.LogError(
                "FamilyManager belum terhubung!"
            );

            return;
        }

        ProcessFeedingForCharacter(
            dad,
            pendingFeedDad
        );

        ProcessFeedingForCharacter(
            mom,
            pendingFeedMom
        );

        ProcessFeedingForCharacter(
            son,
            pendingFeedSon
        );

        ProcessFeedingForCharacter(
            daughter,
            pendingFeedDaughter
        );
    }


    private void ProcessFeedingForCharacter(
        CharacterData character,
        bool pending
    )
    {
        if (!pending)
            return;

        if (character == null)
            return;

        if (!character.isAlive)
        {
            Debug.Log(
                "Feeding gagal → " +
                character.characterName +
                " sudah mati."
            );

            return;
        }

        if (character.isMissing)
        {
            Debug.Log(
                "Feeding gagal → " +
                character.characterName +
                " sedang Missing."
            );

            return;
        }

        // Resource sudah dikurangi saat drop
        // Langsung feed character
        familyManager.FeedCharacter(
            character
        );

        Debug.Log(
            character.characterName +
            " berhasil diberi makan."
        );
    }


    private void ClearPendingFeeding()
    {
        pendingFeedDad = false;
        pendingFeedMom = false;
        pendingFeedSon = false;
        pendingFeedDaughter = false;
    }


    // =====================================================
    // TREATMENT
    // =====================================================

    public void SetPendingTreatment(
        CharacterData character
    )
    {
        if (character == null)
            return;

        if (character == dad)
        {
            pendingTreatDad = true;
            Debug.Log("Pending Treatment → Dad");
        }
        else if (character == mom)
        {
            pendingTreatMom = true;
            Debug.Log("Pending Treatment → Mom");
        }
        else if (character == son)
        {
            pendingTreatSon = true;
            Debug.Log("Pending Treatment → Son");
        }
        else if (character == daughter)
        {
            pendingTreatDaughter = true;
            Debug.Log("Pending Treatment → Daughter");
        }
    }


    public void ProcessPendingTreatment()
    {
        if (familyManager == null)
        {
            Debug.LogError(
                "FamilyManager belum terhubung!"
            );

            return;
        }

        ProcessTreatmentForCharacter(
            dad,
            pendingTreatDad
        );

        ProcessTreatmentForCharacter(
            mom,
            pendingTreatMom
        );

        ProcessTreatmentForCharacter(
            son,
            pendingTreatSon
        );

        ProcessTreatmentForCharacter(
            daughter,
            pendingTreatDaughter
        );

        ClearPendingTreatment();
    }


    private void ProcessTreatmentForCharacter(
        CharacterData character,
        bool pending
    )
    {
        if (!pending)
            return;

        if (character == null)
            return;

        if (!character.isAlive)
        {
            Debug.Log(
                "Treatment GAGAL → " +
                character.characterName +
                " sudah mati."
            );

            return;
        }

        if (character.isMissing)
        {
            Debug.Log(
                "Treatment GAGAL → " +
                character.characterName +
                " sedang Missing."
            );

            return;
        }

        if (!character.isInjured)
        {
            Debug.Log(
                "Treatment GAGAL → " +
                character.characterName +
                " tidak sedang Injured."
            );

            return;
        }

        // Resource sudah dikurangi saat drop
        // Langsung give treatment

        // Treatment dilakukan
        familyManager.GiveMedkit(character);

        // =================================================
        // INI YANG MENANDAI TREATMENT BERHASIL
        // =================================================

        character.treatmentGiven = true;

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "TREATMENT BERHASIL → " +
            character.characterName
        );

        Debug.Log(
            "treatmentGiven = " +
            character.treatmentGiven
        );

        Debug.Log(
            "isInjured = " +
            character.isInjured
        );

        Debug.Log(
            "missingArm = " +
            character.missingArm
        );

        Debug.Log(
            "========================================"
        );
    }


    private void ClearPendingTreatment()
    {
        pendingTreatDad = false;
        pendingTreatMom = false;
        pendingTreatSon = false;
        pendingTreatDaughter = false;
    }


    // =====================================================
    // SACRIFICE / DIARY
    // =====================================================

    public void SetPendingSacrifice(
        CharacterData character
    )
    {
        if (character == null)
            return;

        if (character == dad)
        {
            pendingSacrificeDad = true;

            Debug.Log(
                "Pending Sacrifice → Dad"
            );
        }
        else if (character == mom)
        {
            pendingSacrificeMom = true;

            Debug.Log(
                "Pending Sacrifice → Mom"
            );
        }
        else if (character == son)
        {
            pendingSacrificeSon = true;

            Debug.Log(
                "Pending Sacrifice → Son"
            );
        }
        else if (character == daughter)
        {
            pendingSacrificeDaughter = true;

            Debug.Log(
                "Pending Sacrifice → Daughter"
            );
        }
    }


    private void ProcessPendingSacrifice()
    {
        ProcessSacrificeForCharacter(
            dad,
            pendingSacrificeDad
        );

        ProcessSacrificeForCharacter(
            mom,
            pendingSacrificeMom
        );

        ProcessSacrificeForCharacter(
            son,
            pendingSacrificeSon
        );

        ProcessSacrificeForCharacter(
            daughter,
            pendingSacrificeDaughter
        );

        ClearPendingSacrifice();
    }


    private void ProcessSacrificeForCharacter(
        CharacterData character,
        bool pending
    )
    {
        if (!pending)
            return;

        if (character == null)
            return;

        if (!character.isAlive)
        {
            Debug.Log(
                "Sacrifice GAGAL → " +
                character.characterName +
                " sudah mati."
            );

            return;
        }

        if (character.isMissing)
        {
            Debug.Log(
                "Sacrifice GAGAL → " +
                character.characterName +
                " sedang Missing."
            );

            return;
        }

        // =================================================
        // SACRIFICE
        // =================================================

        character.missingArm = true;

        character.isInjured = true;

        character.injuryStartedToday = true;

        character.treatmentGiven = false;

        Debug.Log(
            "SACRIFICE → " +
            character.characterName
        );

        Debug.Log(
            character.characterName +
            " kehilangan arm."
        );

        Debug.Log(
            character.characterName +
            " menjadi Injured."
        );

        Debug.Log(
            "injuryStartedToday = true"
        );

        // =================================================
        // KEMBALI KE POSISI HOME
        // =================================================

        ReturnCharacterToInitialPosition(
            character
        );
    }


    private void ClearPendingSacrifice()
    {
        pendingSacrificeDad = false;
        pendingSacrificeMom = false;
        pendingSacrificeSon = false;
        pendingSacrificeDaughter = false;
    }


    // =====================================================
    // DOOR / EXIT
    // =====================================================

    public void SetPendingExit(
        CharacterData character
    )
    {
        if (character == null)
            return;

        if (character == dad)
        {
            pendingExitDad = true;

            Debug.Log(
                "Pending Exit → Dad"
            );
        }
        else if (character == mom)
        {
            pendingExitMom = true;

            Debug.Log(
                "Pending Exit → Mom"
            );
        }
        else if (character == son)
        {
            pendingExitSon = true;

            Debug.Log(
                "Pending Exit → Son"
            );
        }
        else if (character == daughter)
        {
            pendingExitDaughter = true;

            Debug.Log(
                "Pending Exit → Daughter"
            );
        }
    }


    public void ProcessPendingExit()
    {
        ProcessExitForCharacter(
            dad,
            pendingExitDad
        );

        ProcessExitForCharacter(
            mom,
            pendingExitMom
        );

        ProcessExitForCharacter(
            son,
            pendingExitSon
        );

        ProcessExitForCharacter(
            daughter,
            pendingExitDaughter
        );

        ClearPendingExit();
    }


    private void ProcessExitForCharacter(
        CharacterData character,
        bool pending
    )
    {
        if (!pending)
            return;

        if (character == null)
            return;

        if (!character.isAlive)
        {
            Debug.Log(
                "Exit GAGAL → " +
                character.characterName +
                " sudah mati."
            );

            return;
        }

        if (character.isMissing)
        {
            Debug.Log(
                character.characterName +
                " sudah Missing."
            );

            return;
        }

        character.isMissing = true;

        character.missingDays = 0;

        Debug.Log(
            character.characterName +
            " sekarang MISSING."
        );
    }


    private void ClearPendingExit()
    {
        pendingExitDad = false;
        pendingExitMom = false;
        pendingExitSon = false;
        pendingExitDaughter = false;
    }


    // =====================================================
    // MISSING
    // =====================================================

    private void ProcessMissingCharacters()
    {
        ProcessMissingCharacter(dad);
        ProcessMissingCharacter(mom);
        ProcessMissingCharacter(son);
        ProcessMissingCharacter(daughter);
    }


    private void ProcessMissingCharacter(
        CharacterData character
    )
    {
        if (character == null)
            return;

        if (!character.isMissing)
            return;

        character.missingDays++;

        Debug.Log(
            character.characterName +
            " sudah pergi/hilang selama " +
            character.missingDays +
            " hari."
        );

        // =================================================
        // BALIK SETELAH 1 HARI (JIKA BUKAN EKSPEDISI)
        // Kalau Ekspedisi, dihandle oleh ExpeditionManager
        // =================================================

        if (character.missingDays >= 1 && !character.isExpedition)
        {
            character.isMissing = false;

            character.missingDays = 0;

            ReturnCharacterToInitialPosition(
                character
            );

            Debug.Log(
                character.characterName +
                " kembali ke rumah dari missing biasa."
            );
        }
    }


    // =====================================================
    // INJURY CONSEQUENCE
    // =====================================================

    private void ProcessInjuryConsequences()
    {
        ProcessInjuryForCharacter(dad);
        ProcessInjuryForCharacter(mom);
        ProcessInjuryForCharacter(son);
        ProcessInjuryForCharacter(daughter);
    }


    private void ProcessInjuryForCharacter(
        CharacterData character
    )
    {
        if (character == null)
            return;

        if (!character.isAlive)
            return;

        if (!character.isInjured)
            return;

        // =================================================
        // INJURY BARU TERJADI HARI INI
        // =================================================

        if (character.injuryStartedToday)
        {
            character.injuryStartedToday = false;

            Debug.Log(
                character.characterName +
                " baru mengalami Injury."
            );

            Debug.Log(
                character.characterName +
                " masih diberi kesempatan untuk Treatment."
            );

            return;
        }

        // =================================================
        // SUDAH DIBERI TREATMENT
        // =================================================

        if (character.treatmentGiven)
        {
            character.isInjured = false;

            character.treatmentGiven = false;

            Debug.Log(
                character.characterName +
                " SEMBUH dari Injury."
            );

            Debug.Log(
                "missingArm tetap = " +
                character.missingArm
            );

            return;
        }

        // =================================================
        // TIDAK DIBERI TREATMENT
        // =================================================

        character.isAlive = false;

        character.isInjured = false;

        character.hungerState = HungerState.Dead;

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            character.characterName +
            " MENINGGAL!"
        );

        Debug.Log(
            "Penyebab → Injury tidak ditangani."
        );

        Debug.Log(
            "========================================"
        );

        ReturnCharacterToInitialPosition(
            character
        );
    }


    // =====================================================
    // RETURN CHARACTER TO HOME
    // =====================================================

    private void ReturnCharacterToInitialPosition(
        CharacterData character
    )
    {
        if (character == null)
            return;

        DragableItem[] items =
            FindObjectsByType<DragableItem>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (DragableItem item in items)
        {
            if (item == null)
                continue;

            if (
                character == dad &&
                item.GetItemType() ==
                DragableItem.ItemType.Dad
            )
            {
                item.ReturnToInitialPosition();

                Debug.Log(
                    "Dad kembali ke HOME POSITION."
                );

                return;
            }

            if (
                character == mom &&
                item.GetItemType() ==
                DragableItem.ItemType.Mom
            )
            {
                item.ReturnToInitialPosition();

                Debug.Log(
                    "Mom kembali ke HOME POSITION."
                );

                return;
            }

            if (
                character == son &&
                item.GetItemType() ==
                DragableItem.ItemType.Son
            )
            {
                item.ReturnToInitialPosition();

                Debug.Log(
                    "Son kembali ke HOME POSITION."
                );

                return;
            }

            if (
                character == daughter &&
                item.GetItemType() ==
                DragableItem.ItemType.Daughter
            )
            {
                item.ReturnToInitialPosition();

                Debug.Log(
                    "Daughter kembali ke HOME POSITION."
                );

                return;
            }
        }

        Debug.LogWarning(
            character.characterName +
            " tidak menemukan DragableItem."
        );
    }


    // =====================================================
    // NEXT DAY
    // =====================================================

    public void NextDay()
    {
        Debug.Log(
            "========== NEXT DAY =========="
        );

        Debug.Log(
            "Current Day: " + currentDay + " → Next Day: " + (currentDay + 1)
        );

        // =================================================
        // 1. MISSING CHARACTERS - Process dulu sebelum yang lain
        // =================================================

        ProcessMissingCharacters();

        // =================================================
        // 2. FEEDING
        // =================================================

        ProcessPendingFeeding();

        // =================================================
        // 3. TREATMENT
        // =================================================

        ProcessPendingTreatment();

        // =================================================
        // 4. SACRIFICE (Diary)
        // =================================================

        ProcessPendingSacrifice();

        // =================================================
        // 5. DOOR / EXIT (Expedition)
        // =================================================

        ProcessPendingExit();

        // =================================================
        // 5.5. EXPEDITION DEPARTURE
        // =================================================

        if (ExpeditionManager.Instance != null)
        {
            ExpeditionManager.Instance.ExecutePendingExpedition();
        }

        // =================================================
        // 5.6. EXPEDITION RETURN
        // =================================================

        if (ExpeditionManager.Instance != null)
        {
            ExpeditionManager.Instance.ProcessReturningExpeditions();
        }

        // =================================================
        // 6. INJURY CONSEQUENCE
        // =================================================

        ProcessInjuryConsequences();

        // =================================================
        // 8. HUNGER - Process hunger untuk yang tidak diberi makan
        // =================================================

        if (familyManager != null)
        {
            familyManager.ProcessDailyHunger();
        }
        else
        {
            Debug.LogError(
                "FamilyManager belum terhubung!"
            );
        }

        // =================================================
        // 9. CLEAR FEEDING
        // =================================================

        ClearPendingFeeding();

        // =================================================
        // 10. INCREMENT DAY
        // =================================================

        currentDay++;

        // =================================================
        // 11. CHECK ENDING
        // =================================================

        bool allCharactersDead =
            EndingManager.Instance != null &&
            EndingManager.Instance.IsAllCharactersDead();

        bool reachedFinalDay =
            EndingManager.Instance != null &&
            EndingManager.Instance.ShouldEndGame(currentDay);

        if (allCharactersDead || reachedFinalDay)
        {
            Debug.Log(
                allCharactersDead
                    ? "Semua anggota keluarga meninggal sebelum hari terakhir. Triggering Bad Ending..."
                    : "Game reached final day. Triggering ending..."
            );

            // Jangan tampilkan report hari berikutnya
            // Langsung trigger ending
            EndingManager.Instance.TriggerEndingWithType();

            return; // Stop execution
        }

        // =================================================
        // 12. FORCE REFRESH CHARACTER VISUALS
        // =================================================

        RefreshAllCharacterVisuals();

        // =================================================
        // 13. TAMPILKAN DAILY REPORT HARI BARU
        // =================================================

        if (reportUIController != null)
        {
            reportUIController.ShowReportForDay(currentDay);
        }
        else if (ReportUIController.Instance != null)
        {
            ReportUIController.Instance.ShowReportForDay(currentDay);
        }
        else
        {
            Debug.LogWarning("ReportUIController belum di-assign, report hari baru tidak ditampilkan otomatis.");
        }

        // =================================================
        // DEBUG
        // =================================================

        Debug.Log(
            "Day: " +
            currentDay
        );

        Debug.Log(
            "Ration: " +
            ration
        );

        Debug.Log(
            "Medkit: " +
            medkit
        );

        Debug.Log(
            "Tools: " +
            tools
        );

        Debug.Log(
            "Knife: " +
            knife
        );

        Debug.Log(
            "Dad | Hunger: " +
            dad.hungerState +
            " | Hungry: " +
            dad.isHungry +
            " | Injured: " +
            dad.isInjured +
            " | Alive: " +
            dad.isAlive +
            " | Missing: " +
            dad.isMissing +
            " | MissingArm: " +
            dad.missingArm
        );

        Debug.Log(
            "Mom | Hunger: " +
            mom.hungerState +
            " | Hungry: " +
            mom.isHungry +
            " | Injured: " +
            mom.isInjured +
            " | Alive: " +
            mom.isAlive +
            " | Missing: " +
            mom.isMissing +
            " | MissingArm: " +
            mom.missingArm
        );

        Debug.Log(
            "Son | Hunger: " +
            son.hungerState +
            " | Hungry: " +
            son.isHungry +
            " | Injured: " +
            son.isInjured +
            " | Alive: " +
            son.isAlive +
            " | Missing: " +
            son.isMissing +
            " | MissingArm: " +
            son.missingArm
        );

        Debug.Log(
            "Daughter | Hunger: " +
            daughter.hungerState +
            " | Hungry: " +
            daughter.isHungry +
            " | Injured: " +
            daughter.isInjured +
            " | Alive: " +
            daughter.isAlive +
            " | Missing: " +
            daughter.isMissing +
            " | MissingArm: " +
            daughter.missingArm
        );

        Debug.Log(
            "=============================="
        );
    }


    // =====================================================
    // REFRESH ALL CHARACTER VISUALS
    // Dipanggil setelah NextDay untuk update sprite
    // =====================================================

    private void RefreshAllCharacterVisuals()
    {
        CharacterVisual[] allVisuals = FindObjectsByType<CharacterVisual>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (CharacterVisual visual in allVisuals)
        {
            if (visual != null)
            {
                visual.ForceRefresh();
            }
        }

        Debug.Log("All character visuals refreshed.");
    }
}