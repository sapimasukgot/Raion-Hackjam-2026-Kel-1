using UnityEngine;

public class PinSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    public GameObject pinPrefab;       // prefab yang punya ConsumablePin
    public RectTransform spawnArea;    // area (RectTransform) tempat pin disebar
    public NeedType needType = NeedType.Food;

    [Header("Sumber jumlah pin")]
    [Tooltip("Drag asset SupplyData yang sesuai (misal RationsData.asset atau MedicineData.asset)")]
    public SupplyData supplyData;

    private readonly System.Collections.Generic.List<GameObject> spawnedPins = new System.Collections.Generic.List<GameObject>();

    private void Start()
    {
        SpawnPins(supplyData.CurrentAmount);

        supplyData.OnAmountChanged.AddListener(OnSupplyAmountChanged);
    }

    private void OnDestroy()
    {
        if (supplyData != null)
            supplyData.OnAmountChanged.RemoveListener(OnSupplyAmountChanged);
    }

    private void OnSupplyAmountChanged(int newAmount)
    {
        spawnedPins.RemoveAll(p => p == null);

        int diff = newAmount - spawnedPins.Count;

        if (diff > 0)
        {
            SpawnPins(diff);
        }
        else if (diff < 0)
        {
            int removeCount = -diff;
            for (int i = 0; i < removeCount && spawnedPins.Count > 0; i++)
            {
                int lastIndex = spawnedPins.Count - 1;
                if (spawnedPins[lastIndex] != null)
                    Destroy(spawnedPins[lastIndex].gameObject);
                spawnedPins.RemoveAt(lastIndex);
            }
        }
    }

    public void SpawnPins(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject pinObj = Instantiate(pinPrefab, spawnArea);
            RectTransform rt = pinObj.GetComponent<RectTransform>();

            Rect rect = spawnArea.rect;
            float randX = Random.Range(rect.xMin, rect.xMax);
            float randY = Random.Range(rect.yMin, rect.yMax);
            rt.anchoredPosition = new Vector2(randX, randY);

            ConsumablePin pin = pinObj.GetComponent<ConsumablePin>();
            if (pin != null)
            {
                pin.needType = needType;
                pin.supplyData = supplyData; // biar pin tau harus ngurangin stok yang mana pas dipakai
            }

            spawnedPins.Add(pinObj);
        }
    }
}