using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Day")]
    public int currentDay = 1;

    [Header("Resources")]
    public int ration = 10;
    public int medkit = 3;
    public int tools = 2;
    public bool knife = true;

    [Header("Family")]
    public CharacterData dad;
    public CharacterData mom;
    public CharacterData son;
    public CharacterData daughter;

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

    private void InitializeGame()
    {
        dad = new CharacterData("Dad");
        mom = new CharacterData("Mom");
        son = new CharacterData("Son");
        daughter = new CharacterData("Daughter");
    }

     
}