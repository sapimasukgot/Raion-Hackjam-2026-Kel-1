// using UnityEngine;

// /// <summary>
// /// Tempel script ini di setiap objek yang jadi "titik nempel" magnet/pin di kulkas.
// /// Objek ini cukup punya RectTransform (bisa Image transparan atau Empty GameObject
// /// dengan RectTransform), gak perlu collider atau raycast apapun.
// /// </summary>
// public class SnapZone : MonoBehaviour
// {
//     [Tooltip("Opsional, buat grouping kalau nanti ada beberapa jenis pin yang cuma boleh nempel di zone tertentu")]
//     public string zoneTag = "Default";

//     [Tooltip("Apakah zone ini masih boleh dipakai (misal 1 zone cuma boleh diisi 1 pin)")]
//     public bool isOccupied = false;

//     public RectTransform RectTransform => (RectTransform)transform;
// }