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

        if (!ResourceManager.Instance.UseRation())
        {
            Debug.Log(
                "Ration habis → " +
                character.characterName +
                " tidak mendapatkan makanan."
            );

            return;
        }

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

        if (!ResourceManager.Instance.UseMedkit())
        {
            Debug.Log(
                "Treatment GAGAL → Medkit habis."
            );

            return;
        }

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
            " sudah pergi selama " +
            character.missingDays +
            " hari."
        );

        // =================================================
        // BALIK SETELAH 1 HARI
        // =================================================

        if (character.missingDays >= 1)
        {
            character.isMissing = false;

            character.missingDays = 0;

            ReturnCharacterToInitialPosition(
                character
            );

            Debug.Log(
                character.characterName +
                " kembali ke rumah."
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

        // =================================================
        // 1. CHARACTER MISSING KEMBALI
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
        // 4. DIARY / SACRIFICE
        // =================================================

        ProcessPendingSacrifice();

        // =================================================
        // 5. DOOR / EXIT
        // =================================================

        ProcessPendingExit();

        // =================================================
        // 6. INJURY CONSEQUENCE
        // =================================================

        ProcessInjuryConsequences();

        // =================================================
        // 7. HUNGER
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
        // 8. CLEAR FEEDING
        // =================================================

        ClearPendingFeeding();

        // =================================================
        // 9. NEXT DAY
        // =================================================

        currentDay++;

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
}