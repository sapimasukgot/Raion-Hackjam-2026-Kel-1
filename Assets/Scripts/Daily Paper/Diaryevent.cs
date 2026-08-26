using UnityEngine;

public enum EventType { Repair, Expedition, Sacrifice, Hunting, Story }

[CreateAssetMenu(fileName = "NewDiaryEvent", menuName = "Game 60s/Diary Event")]
public class DiaryEvent : ScriptableObject
{
    public string eventID;
    public EventType category;
    
    [TextArea(5, 10)]
    public string narasiKejadian;

    [Header("Resource Requirements")]
    public bool requiresTools;
    public bool requiresKnife;
    public bool requiresBandage;

    [Header("Choices & Effects")]
    public ChoiceData[] choices;
}

[System.Serializable]
public struct ChoiceData
{
    public string buttonLabel;
    
    // Changes
    public int rationChange;
    public int medicineChange;
    public int toolsChange;
    public int knifeChange;

    // Special Consequences
    public float expeditionFailureIncrease; // Misal: 0.10f untuk 10%
    public bool causesAmputation;            // Karakter butuh perban
    public bool disablesExpeditionPermanently; // Kaki dipotong
    public bool triggersGameOver;            // Minta Orang
}