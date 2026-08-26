using UnityEngine;
using UnityEngine.Events;

/// Generic supply/inventory data - dipakai untuk Rations MAUPUN Medicine
/// (dan jenis supply lain nanti) dengan cara bikin 2 asset terpisah dari
/// script yang sama, bukan bikin script baru per jenis.

public class SupplyData : ScriptableObject
{
    [Tooltip("Cuma buat label/identitas di Inspector, tidak dipakai logic")]
    public NeedType supplyType = NeedType.Food;

    [SerializeField]
    private int currentAmount = 2;

    public int CurrentAmount => currentAmount;

    // Event ini bisa didengerin script lain (misal PinSpawner) buat tau
    // kapan jumlah rations berubah, tanpa perlu saling reference langsung.
    public UnityEvent<int> OnAmountChanged;

    public void Add(int amount)
    {
        currentAmount += amount;
        OnAmountChanged?.Invoke(currentAmount);
    }

    public bool TryConsume(int amount = 1)
    {
        if (currentAmount < amount) return false;

        currentAmount -= amount;
        OnAmountChanged?.Invoke(currentAmount);
        return true;
    }
}
