[System.Serializable]
public class CharacterData
{
    public string characterName;

    // Status dasar
    public bool isAlive = true;
    public bool isMissing = false;

    // Kondisi
    public bool isHungry = false;
    public bool isInjured = false;

    // Bagian tubuh
    public bool missingFinger = false;
    public bool missingArm = false;
    public bool missingLeg = false;

    // Ekspedisi
    public bool canExpedition = true;

    public CharacterData(string name)
    {
        characterName = name;
    }
}