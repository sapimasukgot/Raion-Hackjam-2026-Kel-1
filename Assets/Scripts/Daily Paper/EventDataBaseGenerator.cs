using UnityEngine;
using UnityEditor;
using System.IO;

public class EventDatabaseGenerator
{
    [MenuItem("Tools/Generate All Diary Events")]
    public static void GenerateEvents()
    {
        string path = "Assets/DiaryEventsData";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // 1. Day 1 Intro
        CreateEvent(path, "Event_Day1_Intro", EventType.Story,
            "Hari ini keluarga Setiawan pindah ke rumah baru. Aneh, tadi di perjalanan, mereka melihat adanya perkumpulan warga-warga di tengah hutan. Setelah sampai ke rumah, keluarga Setiawan segera siap-siap membersihkan rumah baru mereka. Tadi pagi, seorang tetangga datang dan berbicara dengan Adrian Setiawan untuk mengajak bergabung dengan komunitas mereka namun Adrian menolak. Ketika tetangga itu mendengar penolakan itu, dia langsung menjadi gila dan teriak-teriak yang disusul dengan kemunculan para tetangga lain yang menyerbu rumah mereka dengan senjata. Adrian segera mengunci pintu dan mengumpulkan keluarganya di ruang tengah rumah.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Bertahan di Ruang Tengah" }
            });

        // 2. Pintu Rusak
        CreateEvent(path, "Event_PintuRusak", EventType.Repair,
            "Ketukan pintu para tetangga menjadi jauh lebih keras dari biasanya. Ketukan berubah menjadi dentuman yang menggoyangkan seluruh bagian depan rumah, hingga terdengar derit keras dari pintu. Engsel atas pintu itu setengah terbuka.",
            true, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Perbaiki Pintu (-1 Tools)", toolsChange = -1 },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 3. Jendela Rusak
        CreateEvent(path, "Event_JendelaRusak", EventType.Repair,
            "Terbangun di suatu pagi dengan sebuah batu di dalam rumah. Ternyata jendela rumah dilempari batu oleh tetangga. Jendela sekarang berlubang dan hampir lepas.",
            true, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Perbaiki Jendela (-1 Tools)", toolsChange = -1 },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 4. Hunting Tikus
        CreateEvent(path, "Event_HuntingTikus", EventType.Hunting,
            "Kulkas terbuka dengan makanan berjatuhan. Adrian berputar mengelilingi rumah dan menemukan bahwa seekor tikus sedang menikmati hidangan mewah dari kulkas. Tampaknya tikus ini bisa menjadi santapan untuk mereka malam nanti.",
            false, true, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Buru Tikus (-1 Knife, +1 Ration)", knifeChange = -1, rationChange = 1 },
                new ChoiceData { buttonLabel = "Biarkan (-1 Ration)", rationChange = -1 }
            });

        // 5. Minta Jari
        CreateEvent(path, "Event_MintaJari", EventType.Sacrifice,
            "Terdengar suara gesekan kertas dari bawah celah pintu utama. Sebuah surat kusam dengan bercak darah diselipkan ke dalam, bersamaan dengan sebuah pisau daging kecil: 'Desa kami selalu berbagi rezeki dengan mereka yang mau berbagi rasa sakit. Berikan kami satu jari, dan kami akan memberi kalian perbekalan.' Mengorbankan satu jari terasa mengerikan, tapi kelaparan jauh lebih nyata.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Potong Jari (+1 Ration)", rationChange = 1, expeditionFailureIncrease = 0.10f, causesAmputation = true },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 6. Minta Tangan
        CreateEvent(path, "Event_MintaTangan", EventType.Sacrifice,
            "Nyanyian para tetangga malam ini terdengar sangat dekat di teras rumah. Suara nyanyian mendadak berhenti dan digantikan oleh suara gergaji berkarat yang dilemparkan ke depan pintu. Kepala desa berteriak menawarkan perbekalan, dengan syarat persembahan daging yang lebih besar: satu lengan utuh.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Korbankan Tangan (+1 Medicine)", medicineChange = 1, expeditionFailureIncrease = 0.25f, causesAmputation = true },
                new ChoiceData { buttonLabel = "Korbankan Tangan (+1 Ration)", rationChange = 1, expeditionFailureIncrease = 0.25f, causesAmputation = true },
                new ChoiceData { buttonLabel = "Korbankan Tangan (+1 Tools)", toolsChange = 1, expeditionFailureIncrease = 0.25f, causesAmputation = true },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 7. Minta Kaki
        CreateEvent(path, "Event_MintaKaki", EventType.Sacrifice,
            "Bau anyir tercium tajam menembus ventilasi. Tetangga sedang mempersiapkan pesta besar di alun-alun. Seseorang mengetuk jendela dengan ritme lambat: 'Kami punya terlalu banyak makanan, tapi butuh sebuah kaki agar dewa kami bisa berjalan. Berikan, dan kalian tidak akan kelaparan.'",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Korbankan Kaki (+2 Rations)", rationChange = 2, disablesExpeditionPermanently = true, causesAmputation = true },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 8. Minta Orang
        CreateEvent(path, "Event_MintaOrang", EventType.Sacrifice,
            "Malam ini malam bulan purnama. Seluruh warga desa berkumpul mengelilingi rumah membawa obor merah. Suasana hening sebelum dentuman drum ritual dipukul: 'Nyawa untuk menebus dosa penolakan kalian! Serahkan diri kalian untuk ritual suci!' Keputusasaan menyelimuti ruangan.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Serahkan Anggota Keluarga", triggersGameOver = true },
                new ChoiceData { buttonLabel = "Abaikan & Bertahan" }
            });

        // 9. Ekspedisi - Rumah Tetangga
        CreateEvent(path, "Event_Ekspedisi_RumahTetangga", EventType.Expedition,
            "Malam ini, tetangga sebelah rumah sepertinya sedang mengadakan doa bersama di balai desa. Dari jendela, rumah mereka terlihat gelap dan kosong. Ini adalah kesempatan yang sangat bagus untuk menyelinap dan menggeledah rumah mereka. Namun, jika mereka tiba-tiba pulang lebih awal, orang yang dikirim mungkin tidak pernah kembali.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Kirim Ekspedisi" },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 10. Ekspedisi - Pasar Desa
        CreateEvent(path, "Event_Ekspedisi_PasarDesa", EventType.Expedition,
            "Kabut turun sangat tebal malam ini, jarak pandang hampir nol. Suara nyanyian para pemuja bulan terdengar samar dari arah hutan. Pasar desa pasti ditinggalkan tanpa penjagaan. Menembus kabut ini sangat berbahaya, tapi sisa barang di pasar mungkin bisa menyelamatkan nyawa.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Jelajahi Pasar" },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        // 11. Ekspedisi - Sesajen Alun-Alun
        CreateEvent(path, "Event_Ekspedisi_SesajenAlunAlun", EventType.Expedition,
            "Sore tadi, terlihat dari celah kayu warga desa meletakkan tumpukan barang di tengah alun-alun sebagai 'sesajen' untuk bulan purnama. Ada kotak perbekalan di sana! Sekarang tengah malam dan alun-alun terlihat sepi, meskipun penjaga mungkin bersembunyi.",
            false, false, false,
            new ChoiceData[] {
                new ChoiceData { buttonLabel = "Ambil Sesajen" },
                new ChoiceData { buttonLabel = "Abaikan" }
            });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(">>> BERHASIL! 11 Data Kejadian Kertas telah dibuat di folder Assets/DiaryEventsData <<<");
    }

    private static void CreateEvent(string folder, string fileName, EventType type, string text, bool reqTools, bool reqKnife, bool reqBandage, ChoiceData[] choices)
    {
        DiaryEvent asset = ScriptableObject.CreateInstance<DiaryEvent>();
        asset.eventID = fileName;
        asset.category = type;
        asset.narasiKejadian = text;
        asset.requiresTools = reqTools;
        asset.requiresKnife = reqKnife;
        asset.requiresBandage = reqBandage;
        asset.choices = choices;

        AssetDatabase.CreateAsset(asset, $"{folder}/{fileName}.asset");
    }
}