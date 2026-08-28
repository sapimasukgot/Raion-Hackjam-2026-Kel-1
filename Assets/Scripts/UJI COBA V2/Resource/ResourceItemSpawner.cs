using UnityEngine;
using System.Collections.Generic;

public class ResourceItemSpawner : MonoBehaviour
{
    public static ResourceItemSpawner Instance;

    [Header("Item Prefabs")]
    [SerializeField] private GameObject rationPrefab;
    [SerializeField] private GameObject medkitPrefab;
    [SerializeField] private GameObject toolsPrefab;
    [SerializeField] private GameObject knifePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform rationSpawnParent;
    [SerializeField] private Transform medkitSpawnParent;
    [SerializeField] private Transform toolsSpawnParent;
    [SerializeField] private Transform knifeSpawnParent;

    [Header("Layout Settings")]
    [SerializeField] private float itemSpacing = 100f;
    [SerializeField] private int maxItemsPerRow = 5;

    [Header("Refresh Interval")]
    [SerializeField] private float autoRefreshInterval = 0.5f;

    private List<GameObject> spawnedRations = new List<GameObject>();
    private List<GameObject> spawnedMedkits = new List<GameObject>();
    private List<GameObject> spawnedTools = new List<GameObject>();
    private GameObject spawnedKnife;

    private float lastRefreshTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefreshAllItems();
    }

    private void Update()
    {
        // Auto refresh tiap interval
        if (Time.time - lastRefreshTime > autoRefreshInterval)
        {
            RefreshAllItems();
            lastRefreshTime = Time.time;
        }
    }

    // =====================================================
    // REFRESH ALL ITEMS
    // =====================================================

    public void RefreshAllItems()
    {
        if (GameManager.Instance == null)
            return;

        RefreshRations(GameManager.Instance.ration);
        RefreshMedkits(GameManager.Instance.medkit);
        RefreshTools(GameManager.Instance.tools);
        RefreshKnife(GameManager.Instance.knife);
    }

    // =====================================================
    // REFRESH RATIONS
    // =====================================================

    private void RefreshRations(int targetCount)
    {
        // Remove destroyed items from list
        spawnedRations.RemoveAll(item => item == null);

        int currentCount = spawnedRations.Count;

        // Spawn more if needed
        if (currentCount < targetCount)
        {
            int toSpawn = targetCount - currentCount;
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnRation();
            }
        }
        // Destroy extras if needed
        else if (currentCount > targetCount)
        {
            int toDestroy = currentCount - targetCount;
            for (int i = 0; i < toDestroy; i++)
            {
                if (spawnedRations.Count > 0)
                {
                    GameObject item = spawnedRations[spawnedRations.Count - 1];
                    spawnedRations.RemoveAt(spawnedRations.Count - 1);
                    Destroy(item);
                }
            }
        }

        // Update layout
        UpdateLayout(spawnedRations, rationSpawnParent);
    }

    // =====================================================
    // REFRESH MEDKITS
    // =====================================================

    private void RefreshMedkits(int targetCount)
    {
        spawnedMedkits.RemoveAll(item => item == null);

        int currentCount = spawnedMedkits.Count;

        if (currentCount < targetCount)
        {
            int toSpawn = targetCount - currentCount;
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnMedkit();
            }
        }
        else if (currentCount > targetCount)
        {
            int toDestroy = currentCount - targetCount;
            for (int i = 0; i < toDestroy; i++)
            {
                if (spawnedMedkits.Count > 0)
                {
                    GameObject item = spawnedMedkits[spawnedMedkits.Count - 1];
                    spawnedMedkits.RemoveAt(spawnedMedkits.Count - 1);
                    Destroy(item);
                }
            }
        }

        UpdateLayout(spawnedMedkits, medkitSpawnParent);
    }

    // =====================================================
    // REFRESH TOOLS
    // =====================================================

    private void RefreshTools(int targetCount)
    {
        spawnedTools.RemoveAll(item => item == null);

        int currentCount = spawnedTools.Count;

        if (currentCount < targetCount)
        {
            int toSpawn = targetCount - currentCount;
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnTools();
            }
        }
        else if (currentCount > targetCount)
        {
            int toDestroy = currentCount - targetCount;
            for (int i = 0; i < toDestroy; i++)
            {
                if (spawnedTools.Count > 0)
                {
                    GameObject item = spawnedTools[spawnedTools.Count - 1];
                    spawnedTools.RemoveAt(spawnedTools.Count - 1);
                    Destroy(item);
                }
            }
        }

        UpdateLayout(spawnedTools, toolsSpawnParent);
    }

    // =====================================================
    // REFRESH KNIFE
    // =====================================================

    private void RefreshKnife(bool hasKnife)
    {
        if (hasKnife && spawnedKnife == null)
        {
            SpawnKnife();
        }
        else if (!hasKnife && spawnedKnife != null)
        {
            Destroy(spawnedKnife);
            spawnedKnife = null;
        }
    }

    // =====================================================
    // SPAWN METHODS
    // =====================================================

    private void SpawnRation()
    {
        if (rationPrefab == null || rationSpawnParent == null)
            return;

        GameObject item = Instantiate(rationPrefab, rationSpawnParent);
        spawnedRations.Add(item);
    }

    private void SpawnMedkit()
    {
        if (medkitPrefab == null || medkitSpawnParent == null)
            return;

        GameObject item = Instantiate(medkitPrefab, medkitSpawnParent);
        spawnedMedkits.Add(item);
    }

    private void SpawnTools()
    {
        if (toolsPrefab == null || toolsSpawnParent == null)
            return;

        GameObject item = Instantiate(toolsPrefab, toolsSpawnParent);
        spawnedTools.Add(item);
    }

    private void SpawnKnife()
    {
        if (knifePrefab == null || knifeSpawnParent == null)
            return;

        spawnedKnife = Instantiate(knifePrefab, knifeSpawnParent);
    }

    // =====================================================
    // UPDATE LAYOUT
    // Arrange items in grid
    // =====================================================

    private void UpdateLayout(List<GameObject> items, Transform parent)
    {
        if (parent == null)
            return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                continue;

            RectTransform rect = items[i].GetComponent<RectTransform>();
            if (rect == null)
                continue;

            // Calculate grid position
            int row = i / maxItemsPerRow;
            int col = i % maxItemsPerRow;

            float xPos = col * itemSpacing;
            float yPos = -row * itemSpacing;

            rect.anchoredPosition = new Vector2(xPos, yPos);
        }
    }

    // =====================================================
    // PUBLIC FORCE REFRESH
    // =====================================================

    public void ForceRefresh()
    {
        RefreshAllItems();
        Debug.Log("Resource items force refreshed.");
    }
}
