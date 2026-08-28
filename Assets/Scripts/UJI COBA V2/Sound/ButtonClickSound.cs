using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [Tooltip("Centang ini KHUSUS untuk tombol Next: selain SFX klik biasa, akan menyusul SFX flipping page.")]
    [SerializeField] private bool alsoPlayPageFlip = false;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.PlayClick();

        if (alsoPlayPageFlip)
        {
            SoundManager.Instance.PlayPageFlip();
        }
    }
}
