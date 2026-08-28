using UnityEngine;
using UnityEngine.UI;

public class CharacterMemo : MonoBehaviour
{
    // =====================================================
    // CHARACTER YANG DIWAKILI MEMO
    // =====================================================

    public enum MemoCharacter
    {
        Dad,
        Mom,
        Son,
        Daughter
    }

    [Header("Character")]
    [SerializeField] private MemoCharacter characterType;


    // =====================================================
    // BUTTON
    // =====================================================

    private Button button;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError(
                "CharacterMemo → " +
                gameObject.name +
                " tidak memiliki Button."
            );

            return;
        }

        button.onClick.RemoveListener(
            OnMemoClicked
        );

        button.onClick.AddListener(
            OnMemoClicked
        );
    }


    // =====================================================
    // CLICK MEMO
    // =====================================================

    private void OnMemoClicked()
    {
        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "CHARACTER MEMO CLICK"
        );

        Debug.Log(
            "Memo → " +
            characterType
        );


        CharacterData character =
            FindCharacter();


        // -------------------------------------------------
        // CHARACTER TIDAK DITEMUKAN
        // -------------------------------------------------

        if (character == null)
        {
            Debug.LogError(
                "CharacterMemo → Character " +
                characterType +
                " tidak ditemukan."
            );

            Debug.Log(
                "Pastikan GameManager.Instance " +
                "sudah aktif."
            );

            return;
        }


        Debug.Log(
            "Character ditemukan → " +
            character.characterName
        );


        // -------------------------------------------------
        // POPUP
        // -------------------------------------------------

        if (CharacterStatusPopup.Instance == null)
        {
            Debug.LogError(
                "CharacterMemo → " +
                "CharacterStatusPopup.Instance tidak ditemukan."
            );

            return;
        }


        CharacterStatusPopup.Instance.ShowCharacter(
            character
        );


        Debug.Log(
            "Character Status Popup dibuka."
        );

        Debug.Log(
            "========================================"
        );
    }


    // =====================================================
    // FIND CHARACTER
    // =====================================================

    private CharacterData FindCharacter()
    {
        // -------------------------------------------------
        // GAME MANAGER
        // -------------------------------------------------

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "CharacterMemo → " +
                "GameManager.Instance tidak ditemukan."
            );

            return null;
        }


        // -------------------------------------------------
        // AMBIL CHARACTER DARI GAMEMANAGER
        // -------------------------------------------------

        switch (characterType)
        {
            case MemoCharacter.Dad:

                return GameManager.Instance.dad;


            case MemoCharacter.Mom:

                return GameManager.Instance.mom;


            case MemoCharacter.Son:

                return GameManager.Instance.son;


            case MemoCharacter.Daughter:

                return GameManager.Instance.daughter;
        }


        return null;
    }


    // =====================================================
    // PUBLIC GETTER
    // =====================================================

    public MemoCharacter GetCharacterType()
    {
        return characterType;
    }
}