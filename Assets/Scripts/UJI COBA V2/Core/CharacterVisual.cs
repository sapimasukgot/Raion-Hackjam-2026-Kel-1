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
    // HUNGER SPRITES
    // =====================================================

    [Header("Hunger")]
    [SerializeField] private Sprite hungrySprite;
    [SerializeField] private Sprite starvingSprite;


    // =====================================================
    // INJURY SPRITE
    // =====================================================

    [Header("Injured")]
    [SerializeField] private Sprite injuredSprite;


    // =====================================================
    // MISSING ARM
    // =====================================================

    [Header("Missing Arm")]
    [SerializeField] private Sprite missingArmSprite;


    // =====================================================
    // MISSING ARM + INJURY
    // =====================================================

    [Header("Missing Arm + Injured")]
    [SerializeField] private Sprite missingArmInjuredSprite;


    // =====================================================
    // MISSING ARM + HUNGER
    // =====================================================

    [Header("Missing Arm + Hungry")]
    [SerializeField] private Sprite missingArmHungrySprite;


    // =====================================================
    // MISSING ARM + STARVING
    // =====================================================

    [Header("Missing Arm + Starving")]
    [SerializeField] private Sprite missingArmStarvingSprite;


    // =====================================================
    // DEAD
    // =====================================================

    [Header("Dead")]
    [SerializeField] private Sprite deadSprite;


    // =====================================================
    // MISSING
    // =====================================================

    [Header("Missing")]
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
        // DEAD
        // =================================================

        if (!character.isAlive)
        {
            ShowDead(character);
            return;
        }


        // =================================================
        // MISSING
        // =================================================

        if (character.isMissing)
        {
            ShowMissing(character);
            return;
        }


        // =================================================
        // ALIVE
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
        // MISSING ARM + INJURED
        // Prioritas paling tinggi
        // =================================================

        if (
            character.missingArm &&
            character.isInjured
        )
        {
            if (missingArmInjuredSprite != null)
            {
                spriteRenderer.sprite =
                    missingArmInjuredSprite;
            }
            else if (missingArmSprite != null)
            {
                spriteRenderer.sprite =
                    missingArmSprite;
            }
            else if (injuredSprite != null)
            {
                spriteRenderer.sprite =
                    injuredSprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // MISSING ARM + STARVING
        // =================================================

        if (
            character.missingArm &&
            character.hungerState ==
            HungerState.Starving
        )
        {
            if (missingArmStarvingSprite != null)
            {
                spriteRenderer.sprite =
                    missingArmStarvingSprite;
            }
            else if (missingArmSprite != null)
            {
                spriteRenderer.sprite =
                    missingArmSprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // MISSING ARM + HUNGRY
        // =================================================

        if (
            character.missingArm &&
            character.hungerState ==
            HungerState.Hungry
        )
        {
            if (missingArmHungrySprite != null)
            {
                spriteRenderer.sprite =
                    missingArmHungrySprite;
            }
            else if (missingArmSprite != null)
            {
                spriteRenderer.sprite =
                    missingArmSprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // MISSING ARM
        // =================================================

        if (character.missingArm)
        {
            if (missingArmSprite != null)
            {
                spriteRenderer.sprite =
                    missingArmSprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // INJURED
        // =================================================

        if (character.isInjured)
        {
            if (injuredSprite != null)
            {
                spriteRenderer.sprite =
                    injuredSprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // STARVING
        // =================================================

        if (
            character.hungerState ==
            HungerState.Starving
        )
        {
            if (starvingSprite != null)
            {
                spriteRenderer.sprite =
                    starvingSprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // HUNGRY
        // =================================================

        if (
            character.hungerState ==
            HungerState.Hungry
        )
        {
            if (hungrySprite != null)
            {
                spriteRenderer.sprite =
                    hungrySprite;
            }
            else
            {
                spriteRenderer.sprite =
                    normalSprite;
            }

            return;
        }


        // =================================================
        // NORMAL
        // =================================================

        spriteRenderer.sprite =
            normalSprite;
    }


    // =====================================================
    // DEAD
    // =====================================================

    private void ShowDead(
        CharacterData character
    )
    {
        spriteRenderer.enabled = true;

        if (deadSprite != null)
        {
            spriteRenderer.sprite =
                deadSprite;
        }
        else
        {
            // Kalau belum punya sprite dead,
            // sembunyikan character.
            spriteRenderer.enabled = false;
        }
    }


    // =====================================================
    // MISSING
    // =====================================================

    private void ShowMissing(
        CharacterData character
    )
    {
        if (hideWhenMissing)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;

            if (normalSprite != null)
            {
                spriteRenderer.sprite =
                    normalSprite;
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