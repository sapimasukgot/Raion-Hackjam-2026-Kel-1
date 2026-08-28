using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    // =====================================================
    // CHARACTER TYPE
    // =====================================================

    public enum CharacterType
    {
        Dad,
        Mom,
        Son,
        Daughter
    }

    [Header("Character")]
    [SerializeField] private CharacterType characterType;


    // =====================================================
    // SPRITE RENDERER
    // =====================================================

    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;


    // =====================================================
    // NORMAL SPRITE
    // =====================================================

    [Header("Normal")]
    [SerializeField] private Sprite normalSprite;

    // =====================================================
    // MISSING LIMBS
    // =====================================================

    [Header("Missing Limbs")]
    [SerializeField] private Sprite oneArmMissingSprite;
    [SerializeField] private Sprite twoArmsMissingSprite;
    [SerializeField] private Sprite twoArmsOneLegMissingSprite;
    [SerializeField] private Sprite twoArmsTwoLegsMissingSprite;
    [SerializeField] private Sprite oneArmTwoLegsMissingSprite;


    // =====================================================
    // DEAD
    // =====================================================

    [Header("Dead")]
    [SerializeField] private Sprite deadSprite;


    // =====================================================
    // MISSING / EXPEDITION
    // =====================================================

    [Header("Missing / Expedition")]
    [SerializeField] private bool hideWhenMissing = true;


    // =====================================================
    // UPDATE INTERVAL
    // =====================================================

    [Header("Update")]
    [SerializeField] private bool updateEveryFrame = true;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponent<SpriteRenderer>();
        }

        RefreshVisual();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (!updateEveryFrame)
            return;

        RefreshVisual();
    }


    // =====================================================
    // REFRESH VISUAL
    // =====================================================

    public void RefreshVisual()
    {
        if (spriteRenderer == null)
            return;

        CharacterData character =
            GetCharacterData();

        if (character == null)
            return;


        // =================================================
        // DEAD / MISSING / EXPEDITION → HIDE
        // =================================================

        if (!character.isAlive || character.isMissing || character.isExpedition)
        {
            ShowDead(character);
            return;
        }


        // =================================================
        // ALIVE - SHOW SPRITE
        // =================================================

        spriteRenderer.enabled = true;

        UpdateAliveVisual(character);
    }


    // =====================================================
    // GET CHARACTER DATA
    // =====================================================

    private CharacterData GetCharacterData()
    {
        if (GameManager.Instance == null)
            return null;

        switch (characterType)
        {
            case CharacterType.Dad:
                return GameManager.Instance.dad;

            case CharacterType.Mom:
                return GameManager.Instance.mom;

            case CharacterType.Son:
                return GameManager.Instance.son;

            case CharacterType.Daughter:
                return GameManager.Instance.daughter;
        }

        return null;
    }


    // =====================================================
    // UPDATE ALIVE VISUAL
    // =====================================================

    private void UpdateAliveVisual(
        CharacterData character
    )
    {
        // =================================================
        // PRIORITY: MISSING LIMBS (Paling Parah → Normal)
        // =================================================

        spriteRenderer.enabled = true;

        // 1. Tangan buntung 2 + Kaki buntung 2
        if (character.missingArm && character.missingLeg)
        {
            if (twoArmsTwoLegsMissingSprite != null)
            {
                spriteRenderer.sprite = twoArmsTwoLegsMissingSprite;
                return;
            }
        }

        // 2. Tangan buntung 2 + Kaki buntung 1
        if (character.missingArm && character.missingLeg)
        {
            if (twoArmsOneLegMissingSprite != null)
            {
                spriteRenderer.sprite = twoArmsOneLegMissingSprite;
                return;
            }
        }

        // 3. Tangan buntung 2
        if (character.missingArm)
        {
            if (twoArmsMissingSprite != null)
            {
                spriteRenderer.sprite = twoArmsMissingSprite;
                return;
            }
        }

        // 4. Tangan buntung 1 + Kaki buntung 2
        if (character.missingLeg)
        {
            if (oneArmTwoLegsMissingSprite != null)
            {
                spriteRenderer.sprite = oneArmTwoLegsMissingSprite;
                return;
            }
        }

        // 5. Tangan buntung 1
        if (character.missingArm)
        {
            if (oneArmMissingSprite != null)
            {
                spriteRenderer.sprite = oneArmMissingSprite;
                return;
            }
        }

        // 6. NORMAL
        spriteRenderer.sprite = normalSprite;
    }


    // =====================================================
    // DEAD / MISSING / EXPEDITION
    // =====================================================

    private void ShowDead(
        CharacterData character
    )
    {
        // Hide untuk dead, missing, expedition
        if (hideWhenMissing || !character.isAlive)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;

            if (deadSprite != null)
            {
                spriteRenderer.sprite = deadSprite;
            }
        }
    }


    // =====================================================
    // MANUAL REFRESH
    // Bisa dipanggil dari script lain
    // =====================================================

    public void ForceRefresh()
    {
        RefreshVisual();
    }
}