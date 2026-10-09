# CombatMeter

Meter pertempuran party real-time untuk Star Resonance: **DPS**, **HPS** dan **damage yang
diterima** untuk semua orang di party-mu, dengan warna peran dan lambang kelas supaya kamu bisa
membaca kondisi party sekilas.

![Meter party real-time](media/combat-meter.png)

## Memulai

1. Pasang plugin-nya lalu jalankan game secara **Modded**.
2. Meter akan muncul sebagai jendela overlay setelah kamu masuk ke dunia. Seret bagian judulnya
   untuk memindahkannya — posisinya akan diingat.
3. Masuk pertempuran: baris akan muncul per anggota party dan ter-update secara real-time.

## Membaca meter

- Setiap baris adalah satu anggota party: damage per detik, healing per detik, dan damage yang
  diterima.
- Warna baris mengikuti **peran** anggota (tank / healer / DPS); lambangnya menunjukkan kelasnya.
- Header menampilkan timer encounter. Leader party mendapat **tombol hitung mundur tim** di
  header untuk memulai timer pull game untuk seluruh party.

## Rincian skill

Klik baris anggota party untuk membuka **rincian skill**-nya — pembagian damage per skill,
crit rate dan uptime untuk pertarungan saat ini.

![Rincian skill](media/skill-breakdown.png)

## Arsip — menyimpan pertarungan

Sebuah *arsip* menyimpan pertarungan saat ini ke Riwayat dan mereset tampilan live.

![Riwayat](media/combatmeter-history.png)

- **Arsip manual** (tombol arsip) **selalu disimpan**, apa pun yang terjadi — kamu selalu
  mendapat umpan balik yang terlihat bahwa itu sudah disimpan.
- **Arsip-otomatis** bisa menyimpan pertarungan untukmu. Buka **panel Pengaturan (ikon gir)**
  untuk mengaturnya: nyala/mati utama, toggle per-trigger (tim tumbang, fase bos, diam
  bertarung, perubahan tahap dungeon), jeda minimum antar-arsip, selesaikan, tenggang bangkit
  saat tim tumbang, dan abaikan saat solo.
- **"Simpan sebelum" fase bos** membiarkan beberapa detik ancang-ancang sebelum pukulan bos
  pertamamu ikut masuk ke segmen bos (default mati = terpotong tepat di pukulan pertama).
- Arsip otomatis dilewati hanya ketika benar-benar tidak ada yang terjadi (semua baris nol).
  Arsip yang dilewati tidak menghapus apa pun — semuanya terbawa ke arsip berikutnya.

## Unggahan dan replay run

Run yang diarsipkan diunggah ke [Stellar Logs](https://logs.stellarresonance.app), tempat kamu
mendapat halaman run lengkap: grafik damage, detail skill, fase bos — dan **replay pergerakan**
dari seluruh run, mulai dari saat kamu masuk dungeon sampai kill.

- **Salin tautan** menaruh URL pendek run tersebut di clipboard-mu untuk dibagikan ke party-mu.
- **Riwayat** menyimpan run yang diarsipkan lintas relaunch. Mengunggah ulang run yang diarsipkan
  mereproduksi unggahan asli persis sama — ringkasan, detail pertempuran lengkap, dan jejak
  pergerakan — bahkan jika data di sisi server sudah hilang.

## Tips

- Meter tetap bekerja walau data profesi belum termuat — warna peran dan lambang ditebak dari
  spec-mu sampai roster terisi penuh.
- Jika tautan run gagal diunggah, coba lagi dari Riwayat: run yang sudah diunggah akan
  mengarah ke tautannya yang sudah ada, bukan gagal.
