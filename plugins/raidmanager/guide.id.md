# Manajer Raid

Alat koordinasi raid yang ditampilkan sebagai overlay HUD — kompatibel dengan konvensi raid-call
ZDPS yang sudah digunakan statik-mu.

## Countdown & Peringatan Raid

1. Instal plugin lalu jalankan game dalam mode **Modded**.
2. Ketik `/ct <seconds>` di chat untuk memulai countdown pull besar di layar (berubah merah di
   5 detik terakhir).
3. Ketik `/rw <message>` untuk menampilkan peringatan raid ke semua orang yang menjalankan plugin
   ini.

![Countdown](images/countdown.png)

- **Gunakan Peringatan Dalam Game** (aktif secara default) — `/rw` akan menampilkan banner
  notifikasi asli game beserta audio kemenangan, menggantikan overlay kustom plugin, sehingga
  peringatan terlihat dan terdengar seperti peringatan bawaan game.

## Preset Mark

Simpan marker dungeon yang kamu tempatkan dan muat ulang seluruh layout-nya dalam satu klik —
sebagai urutan fase bertahap. Buka dari **Manajer Raid → Buka Preset Mark**:

![Raid Manager settings](images/raid-manager-settings.png)

**Membuat preset**

1. **Buat** sebuah preset, beri nama, dan **Aktifkan**.
2. Di dalam dungeon, tempatkan marker untuk fase pertama, lalu **+ Simpan Langkah**.
3. Tempatkan marker fase berikutnya dan **+ Simpan Langkah** lagi — ulangi untuk setiap fase.

**Menggunakannya saat bertarung**

- **Berikutnya ▶** / **◀ Sebelumnya** berpindah antar langkah, membersihkan papan dan menempatkan
  marker fase tersebut.
- **Reset** kembali ke **Start** yang kosong.
- Ikat **Sebelumnya / Reset / Berikutnya** ke tombolmu sendiri di pengaturan tombol game (belum
  diikat secara default).

![Mark Presets](images/mark-presets.png)

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

## Catatan

- Kedua overlay tetap terlihat saat menu game (ESC) terbuka.
