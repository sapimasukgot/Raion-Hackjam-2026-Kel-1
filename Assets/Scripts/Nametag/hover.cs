using UnityEngine;
using TMPro;

public class FloatingHover : MonoBehaviour
{
    [Header("UI Teks")]
    public GameObject nameTagObject;
    public TextMeshPro nameTagText;

    [Header("Data Karakter")]
    public string characterName = "Adrian (Ayah)";
    [TextArea] public string characterStatus = "Kondisi: Sehat";

    [Header("Pengaturan Warna & Estetik")]
    public Color normalColor = Color.white; // Warna asli sprite
    // highlightColor: R: 1.5, G: 1.5, B: 1.5, A: 1 (Bikin warna jadi 'nyala' lebih terang dari putih)
    public Color highlightColor = new Color(1.5f, 1.5f, 1.5f, 1f); 
    
    [Range(1f, 20f)]
    public float fadeSpeed = 8f; // Seberapa cepat transisinya (semakin besar semakin cepat)

    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private Color targetColor; // Warna tujuan yang sedang dituju

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        // Setel warna awal ke normal
        if (spriteRenderer != null) spriteRenderer.color = normalColor;
        targetColor = normalColor;

        if (nameTagObject != null) nameTagObject.SetActive(false);
    }

    private void Update()
    {
        // 1. Cek Posisi Mouse Manual (Anti-Stress)
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2. Tentukan Warna Tujuan & Teks
        if (myCollider != null && myCollider.OverlapPoint(mousePos))
        {
            // SAAT HOVER
            targetColor = highlightColor; // Tujuan warna: Highlight terang

            if (nameTagObject != null && !nameTagObject.activeSelf)
            {
                nameTagText.text = $"<b>{characterName}</b>\n<size=80%>{characterStatus}</size>";
                nameTagObject.SetActive(true);
            }
        }
        else
        {
            // SAAT TIDAK HOVER
            targetColor = normalColor; // Tujuan warna: Kembali normal

            if (nameTagObject != null && nameTagObject.activeSelf)
            {
                nameTagObject.SetActive(false);
            }
        }

        // 3. Aplikasikan Transisi Warna Halus (Efek Estetik)
        if (spriteRenderer != null)
        {
            // Color.Lerp: Ngubah warna saat ini (spriteRenderer.color) ke warna tujuan (targetColor) secara perlahan
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, fadeSpeed * Time.deltaTime);
        }
    }
}