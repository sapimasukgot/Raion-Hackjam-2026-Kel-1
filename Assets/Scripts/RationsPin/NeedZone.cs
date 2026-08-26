using UnityEngine;

/// <summary>
/// Tempel di area drop pada memo (misal ikon "lapar" di memo Bapak).
/// Reusable: script sama, tinggal ganti field targetMember & needType
/// di Inspector untuk tiap memo anggota keluarga yang beda.
/// </summary>
public class NeedZone : MonoBehaviour
{
    [Header("Target")]
    public FamilyMemberStatus targetMember;
    public NeedType needType;

    public RectTransform RectTransform => (RectTransform)transform;

    /// <summary>
    /// Zone ini cuma boleh dipakai kalau kebutuhan terkait memang lagi aktif.
    /// Contoh: kalau Bapak gak lagi lapar, drop pin makanan ke sini ditolak.
    /// </summary>
    public bool IsActive => targetMember != null && targetMember.IsNeedActive(needType);

    /// <summary>
    /// Dipanggil oleh ConsumablePin saat berhasil di-drop di sini.
    /// </summary>
    public void Fulfill()
    {
        targetMember.Fulfill(needType);
    }
}
