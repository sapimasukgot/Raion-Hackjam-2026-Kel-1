using UnityEngine;
using UnityEngine.EventSystems;

public class DiaryPaperBackdrop : MonoBehaviour, IPointerClickHandler
{
    public DiaryPaperUIController diaryPaperUIController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (diaryPaperUIController != null)
        {
            diaryPaperUIController.CloseUI();

            Debug.Log("📄 Klik di luar Diary Paper → UI ditutup.");
        }
    }
}