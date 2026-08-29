using UnityEngine;
using UnityEngine.EventSystems;

public class DiaryPaperObject : MonoBehaviour, IPointerClickHandler
{
    [Header("Diary Paper UI Besar")]
    public DiaryPaperUIController diaryPaperUIController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("📄 Diary Paper kecil diklik.");

        if (diaryPaperUIController != null)
        {
            diaryPaperUIController.OpenUI();
        }
        else
        {
            Debug.LogError(
                "❌ DiaryPaperUIController belum di-assign di Inspector!",
                this
            );
        }
    }
}