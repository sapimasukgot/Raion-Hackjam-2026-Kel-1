using UnityEngine;

public class FamilyManager : MonoBehaviour
{
    private void Start()
    {
        SacrificeArm(GameManager.Instance.dad);

        Debug.Log(
            "Dad - Arm: " +
            GameManager.Instance.dad.missingArm +
            " | Injured: " +
            GameManager.Instance.dad.isInjured
        );
    }
    public void FeedCharacter(CharacterData character)
    {
        if (character == null)
            return;

        if (!character.isAlive || character.isMissing)
            return;

        character.isHungry = false;

        Debug.Log(character.characterName + " sudah diberi makan.");
    }

    public void GiveMedkit(CharacterData character)
    {
        if (character == null)
            return;

        if (!character.isAlive || character.isMissing)
            return;

        if (!character.isInjured)
        {
            Debug.Log(character.characterName + " tidak sedang terluka.");
            return;
        }

        if (GameManager.Instance.medkit <= 0)
        {
            Debug.Log("Medkit habis!");
            return;
        }

        GameManager.Instance.medkit--;

        character.isInjured = false;

        Debug.Log(
            character.characterName +
            " sudah diobati. Medkit tersisa: " +
            GameManager.Instance.medkit
        );
    }

    public void SacrificeFinger(CharacterData character)
    {
        if (!CanSacrifice(character))
            return;

        character.missingFinger = true;
        character.isInjured = true;

        Debug.Log(
            character.characterName +
            " kehilangan jari dan sekarang injured."
        );
    }

    public void SacrificeArm(CharacterData character)
    {
        if (!CanSacrifice(character))
            return;

        character.missingArm = true;
        character.isInjured = true;

        character.canExpedition = false;

        Debug.Log(
            character.characterName +
            " kehilangan tangan dan sekarang injured."
        );
    }

    public void SacrificeLeg(CharacterData character)
    {
        if (!CanSacrifice(character))
            return;

        character.missingLeg = true;
        character.isInjured = true;

        character.canExpedition = false;

        Debug.Log(
            character.characterName +
            " kehilangan kaki dan sekarang injured."
        );
    }

    private bool CanSacrifice(CharacterData character)
    {
        if (character == null)
            return false;

        if (!character.isAlive)
            return false;

        if (character.isMissing)
            return false;

        return true;
    }
}