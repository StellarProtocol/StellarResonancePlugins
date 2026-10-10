# Manajer Raid

Alat koordinasi raid yang ditampilkan sebagai overlay HUD — kompatibel dengan konvensi raid-call
ZDPS yang sudah digunakan statik-mu.

## Countdown & Peringatan Raid

1. Instal plugin lalu jalankan game dalam mode **Dengan mod**.
2. Ketik `/ct <seconds>` di chat untuk memulai countdown pull besar di layar (berubah merah di
   5 detik terakhir).
3. Ketik `/rw <message>` untuk menampilkan peringatan raid ke semua orang yang menjalankan plugin
   ini.

![Countdown](media/countdown.png)

- **Gunakan Peringatan Dalam Game** (aktif secara default) — `/rw` akan menampilkan banner
  notifikasi asli game beserta audio kemenangan, menggantikan overlay kustom plugin, sehingga
  peringatan terlihat dan terdengar seperti peringatan bawaan game.

## Preset Mark

Simpan marker dungeon yang kamu tempatkan dan muat ulang seluruh layout-nya dalam satu klik —
sebagai urutan fase bertahap. Buka dari **Manajer Raid → Buka Preset Mark**:

![Raid Manager settings](media/raid-manager-settings.png)

**Membuat preset**

1. **Buat** sebuah preset, beri nama, dan **Aktifkan**.
2. Di dalam dungeon, tempatkan marker untuk fase pertama, lalu **+ Simpan Langkah**.
3. Tempatkan marker fase berikutnya dan **+ Simpan Langkah** lagi — ulangi untuk setiap fase.

**Menggunakannya saat bertarung**

- **Berikutnya ▶** / **◀ Sebelumnya** berpindah antar langkah, membersihkan papan dan menempatkan
  marker fase tersebut.
- **Reset** kembali ke **Start** yang kosong.
- Ikat **Preset Mark: Langkah sebelumnya**, **Preset Mark: Reset ke Start**, dan **Preset Mark:
  Langkah berikutnya** ke tombolmu sendiri di **Pengaturan Stellar → Hotkey** (belum diikat secara
  default).

![Mark Presets](media/mark-presets.png)

Simpan preset terpisah untuk tiap raid dan **Aktifkan** yang sedang kamu jalani (tombolnya akan
berubah menjadi **Nonaktifkan**). Penempatan marker berfungsi di dalam dungeon.

**Ganti nama dan tambahkan catatan**

- Klik ikon **pensil** di sebelah preset untuk mengganti namanya.
- Di bawah langkah saat ini, klik ikon **pensil** untuk menambahkan catatan — misalnya "P2 —
  kumpul di barat". Catatan ini juga muncul di pop-up saat kamu berpindah ke langkah tersebut.

**Membagikan preset**

1. Aktifkan preset-nya, klik **Ekspor Preset**, lalu **Salin** — atau pilih kode-nya dan tekan
   Ctrl+C.
2. Kirim kode itu ke grupmu.
3. Mereka klik **Impor Preset**, tempel kode-nya, lalu tekan **Impor**. Mereka akan mendapatkan
   seluruh preset — setiap langkah, marker, dan catatan.

## Panggilan Mekanik (Beta)

Lihat sekilas siapa yang ditargetkan oleh mekanik boss mana — sebagai daftar, minimap, dan peringatan
besar saat kamu yang kena. Buka dari **Manajer Raid → Buka Panggilan Mekanik (Beta)**:

![Mechanic Callouts settings](media/mechanic-callouts-settings.png)

**Konten yang didukung**

- Raid Forgotten Dreamwild — Clash!, Brutal!, dan Purge!
- Cursed Radiant Tomb
- Sea-Ringed Reef
- Towering Ruin
- Tina's Mindrealm

Di luar konten ini, panggilan tetap kosong.

**Daftar Panggilan** — setiap mekanik beserta hitung mundurnya dan pemain yang ditargetkan. Pilih
urutannya (**Urutan kemunculan**, **Paling mendesak dulu**, atau **Urutan tabel**), ukuran teks, dan
opasitas latar belakang.

![Callout list](media/mechanic-callout-list.png)

**Minimap** — party-mu di arena, diwarnai sesuai mekanik, dengan marker party dan area bahaya. Di raid,
minimap juga menampilkan damage lantai, urutan menekan kristal, dan langkah cincin elektromagnetik
yang bernomor.

![Minimap](media/mechanic-minimap.png)

**Peringatan Mekanik** — banner besar saat mekanik menargetkanmu, dan **PINDAH** saat kamu berdiri di
petak bahaya. Pada fase cincin di raid, peringatan memberi tahu cincin mana yang harus kamu tuju. Suara
bersifat opsional dengan volume tersendiri; gunakan **Uji peringatan** untuk pratinjau. (Nonaktif secara
default.)

![Alert banner](media/mechanic-alert.png)

Pindahkan daftar, minimap, dan banner ke mana saja di editor tata letak HUD.

## Catatan

- Kedua overlay tetap terlihat saat menu game (ESC) terbuka.
