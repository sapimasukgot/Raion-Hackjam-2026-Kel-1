using UnityEngine;

public class FamilyManager : MonoBehaviour
{
     
   public void FeedCharacter(CharacterData character)
    {
        if (character == null)
            return;

        if (!character.isAlive || character.isMissing)
            return;

        character.isHungry = false;
        character.hungerState = HungerState.Normal;

        Debug.Log(
            character.characterName +
            " sudah diberi makan. Hunger kembali NORMAL."
        );
    }

    private void ProcessHunger(CharacterData character)
    {
        switch (character.hungerState)
        {
            case HungerState.Normal:

                character.hungerState = HungerState.Hungry;
                character.isHungry = true;

                Debug.Log(
                    character.characterName +
                    " sekarang HUNGRY."
                );

                break;

            case HungerState.Hungry:

                character.hungerState = HungerState.Starving;
                character.isHungry = true;

                Debug.Log(
                    character.characterName +
                    " sekarang STARVING!"
                );

                break;

            case HungerState.Starving:

                character.hungerState = HungerState.Dead;
                character.isAlive = false;
                character.isHungry = true;

                Debug.Log(
                    character.characterName +
                    " MATI karena kelaparan!"
                );

                break;
        }
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

   public void ProcessDailyHunger()
    {
        GameManager gameManager = GameManager.Instance;

        ProcessCharacterHunger(
            gameManager.dad,
            gameManager.pendingFeedDad
        );

        ProcessCharacterHunger(
            gameManager.mom,
            gameManager.pendingFeedMom
        );

        ProcessCharacterHunger(
            gameManager.son,
            gameManager.pendingFeedSon
        );

        ProcessCharacterHunger(
            gameManager.daughter,
            gameManager.pendingFeedDaughter
        );
    }

    private void ProcessCharacterHunger(
        CharacterData character,
        bool wasFed
    )
    {
        if (character == null)
            return;

        if (!character.isAlive)
            return;

        if (character.isMissing)
            return;

        // Kalau hari ini sudah diberi makan,
        // jangan naikkan hunger.
        if (wasFed)
            return;

        ProcessHunger(character);
    }
}