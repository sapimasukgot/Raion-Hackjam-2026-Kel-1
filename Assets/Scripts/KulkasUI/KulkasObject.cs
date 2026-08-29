using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class KulkasObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Teks di Atas Objek")]
    public GameObject clickTextObject;
    public TextMeshPro clickText;

    [Header("Referensi UI Panel Kulkas")]
    public KulkasUIController kulkasUIController;
    // ➕ 1. Tambahkan referensi animasi di sini:
    public UIAutoAnimation kulkasUIAnimation;

    [Header("Pengaturan Warna Highlight")]
    public Color normalColor = Color.white;
    public Color highlightColor = new Color(1.3f, 1.3f, 1.3f, 1f);
    [Range(1f, 20f)] public float fadeSpeed = 8f;

    private SpriteRenderer spriteRenderer;
    private Color targetColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetColor = normalColor;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }

        if (clickTextObject != null)
        {
            clickTextObject.SetActive(false);
        }

        if (kulkasUIController == null)
        {
            Debug.LogError("⚠️ KulkasUIController belum dihubungkan di Inspector!", this);
        }
    }

    private void Update()
    {
        // Transisi warna highlight
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, fadeSpeed * Time.deltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetColor = highlightColor;

        if (clickTextObject != null)
        {
            if (clickText != null) clickText.text = "<b>Click</b>";
            clickTextObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetColor = normalColor;

        if (clickTextObject != null)
        {
            clickTextObject.SetActive(false);
        }
    }

    // Dipanggil otomatis saat kulkas DIKLIK
    // Dipanggil otomatis saat kulkas DIKLIK
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("✅ Kulkas diklik (IPointerClick)!");

            // Cukup panggil controller, controller yang akan menyalakan panel dulu baru jalankan animasi
            if (kulkasUIController != null)
            {
                kulkasUIController.OpenUI();
            }
        }
    }

}
