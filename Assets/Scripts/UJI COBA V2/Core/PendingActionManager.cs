using UnityEngine;

public class PendingActionManager : MonoBehaviour
{
    public enum Character
    {
        None,
        Dad,
        Mom,
        Daughter,
        Son
    }

    public enum ActionType
    {
        None,
        Feeding,
        Treatment,
        Sacrifice,
        Expedition
    }

    public ActionType CurrentAction { get; private set; }
    public Character TargetCharacter { get; private set; }

    public void SetFeeding(Character character)
    {
        CurrentAction = ActionType.Feeding;
        TargetCharacter = character;

        Debug.Log(
            "Pending Feeding: " + character
        );
    }

    public void SetTreatment(Character character)
    {
        CurrentAction = ActionType.Treatment;
        TargetCharacter = character;

        Debug.Log(
            "Pending Treatment: " + character
        );
    }

    public void SetSacrifice(Character character)
    {
        CurrentAction = ActionType.Sacrifice;
        TargetCharacter = character;

        Debug.Log(
            "Pending Sacrifice: " + character
        );
    }

    public void SetExpedition(Character character)
    {
        CurrentAction = ActionType.Expedition;
        TargetCharacter = character;

        Debug.Log(
            "Pending Expedition: " + character
        );
    }

    public void ClearAction()
    {
        CurrentAction = ActionType.None;
        TargetCharacter = Character.None;
    }
}