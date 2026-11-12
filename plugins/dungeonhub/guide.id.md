# Pusat Dungeon

Semua dungeon dalam satu jendela. Pilih dungeon dan tingkat kesulitan, lalu masuk Solo, mulai Cari Tim, atau
masuk bersama party Anda, tanpa harus berjalan ke NPC. Pusat Dungeon juga mengganti layar konfirmasi masuk layar
penuh dari game dengan jendela kecil yang menunjukkan siapa yang sudah siap.

![Pusat Dungeon](media/hub.png)

## Memulai

1. Pasang plugin-nya lalu jalankan game dalam mode **Dengan mod**.
2. Masuk ke karakter, lalu buka **Pusat Dungeon** dengan salah satu cara ini:
   - tombol **Pusat Dungeon** di sisi kanan menu Esc game,
   - ubin **Pusat Dungeon** di peluncur Stellar,
   - hotkey pilihan Anda sendiri (lihat *Hotkey* di bawah).
3. Pilih tab, pilih dungeon di kiri, lalu pilih tingkat kesulitan dan **Cara masuk** di kanan.

Bagian atas jendela menunjukkan apakah Anda sendirian atau dalam party (dan apakah Anda ketuanya). Jendela terbuka
di tab yang terakhir Anda pakai.

## Memilih dungeon

Ada lima tab: **Dungeon**, **Raid**, **World Boss**, **Perburuan**, dan **Lainnya**.

**Dungeon**

- **BUKA HARI INI** di paling atas berisi dungeon Labil yang buka hari ini, ditandai **LABIL**. Dungeon biasa
  menyusul di bawah **DUNGEON**. Kalau hari ini tidak ada dungeon Labil yang buka, daftar tidak punya judul.
- Di **TINGKAT KESULITAN**, pilih **Normal**, **Sulit**, atau **Master**. Memilih **Master** menampilkan 20 levelnya:
  - hijau = selesai, polos = level berikutnya yang harus diselesaikan, redup = terkunci;
  - garis bawah berwarna menandai level yang bisa **Solo** sekaligus **Cari Tim**.

**Raid**

- Semua raid yang sedang ditawarkan game, dengan **Mudah**, **Sulit**, dan **Mimpi Buruk** sesuai yang dimiliki
  raid itu.

**Arti kotak info**

- **Disarankan**: skor yang disarankan game untuk tingkat kesulitan ini.
- **Skor Anda**: hijau kalau sudah mencapai saran, kuning kalau masih di bawahnya.
- **Clear pertama**: apakah hadiah clear pertama masih menunggu (**Siap diklaim**) atau **Sudah diklaim**.
- **Sisa hadiah**: berapa kali lagi Anda bisa mendapat hadiah. Di angka 0 Anda tetap bisa masuk, hanya saja tanpa
  hadiah.

## Cara masuk

Tiga tombol di bagian bawah panel:

- **Solo**: masuk sendiri, dengan bot.
- **Cari Tim**: cari tim otomatis. Selama berjalan, bilah di bawah menampilkan **Mencari tim** beserta waktunya dan
  tombol **Batal**.
- **Party**: masuk bersama party Anda.

Tombol menjadi abu-abu kalau saat ini tidak bisa dipakai, dan baris di bawahnya menjelaskan alasannya, misalnya
"tidak dalam party" atau "butuh 3+ anggota party (ada 1) - gunakan Cari Tim". Dalam party, hanya ketua yang bisa
memulai masuk atau Cari Tim.

## World Boss, Perburuan, dan Lainnya

- **World Boss**: boss minggu ini, sisa hadiah Anda, apakah event sedang buka, dan kapan buka berikutnya. World
  Boss hanya bisa lewat **Cari Tim**.
- **Perburuan**: perburuan mingguan dan **Perburuan Guild**. Perburuan Guild bisa dimasuki dari sini (Normal atau
  Sulit, **Solo** atau **Party**) selama sedang buka dan Anda punya guild.
- **Lainnya**: yang pertama adalah **Komisi Biro**. Ambil misi hariannya, lihat event hari ini dan berapa yang sudah
  selesai, lalu **Masuk** ke event secara langsung. **Pergi ke papan** membawa Anda ke papan Biro Perintis.
- Aktivitas lainnya punya tombol **Buka** yang membuka halaman aktivitas itu di game.

## Konfirmasi Dungeon

Saat party mulai masuk atau tim ditemukan, biasanya game menutupi seluruh layar untuk meminta semua orang
konfirmasi. Pusat Dungeon menampilkan jendela kecil sebagai gantinya. Ini berlaku untuk dungeon 5 pemain, raid, dan
saat tim ditemukan.

![Konfirmasi Dungeon](media/confirm.png)

- Dungeon, tingkat kesulitan, dan hitung mundur di bagian atas.
- **AFIKS**: afiks dungeon, kalau ada.
- **PARTY** (atau **RAID**, dibagi menjadi empat grup): setiap pemain dengan kelas, skor, dan statusnya:
  **Siap**, **Menunggu…**, atau **Menolak**. Baris Anda sendiri diberi garis tepi.
- **Konfirmasi** dan **Tolak** di bagian bawah. Saat tim ditemukan, Tolak bertanya sekali lagi (**Tolak?**) supaya
  tidak terpencet tanpa sengaja. Kalau Anda ketua yang memulai masuk, yang muncul adalah **Batalkan**.
- Saat semua sudah siap, jendela tertutup dan Anda masuk. Kalau ada yang menolak atau waktu habis, jendela
  menyebutkan siapa, lalu tertutup sendiri.
- Jendela ini selalu berada di atas jendela Stellar lainnya, jadi tidak pernah tertutup olehnya.

## Pengaturan

Klik tombol roda gigi (**⚙**) di bagian atas Pusat Dungeon untuk membuka **Pengaturan Pusat Dungeon**.

![Pengaturan Pusat Dungeon](media/settings.png)

**LAYAR KONFIRMASI**

- **Ganti layar konfirmasi game**: nyalakan atau matikan jendela kecil. Bawaannya menyala. Kalau dimatikan, Anda
  mendapat layar konfirmasi layar penuh dari game.
- **Pratinjau 5 pemain** / **Pratinjau raid**: lihat jendelanya dengan pemain contoh, tanpa party. Tombol-tombolnya
  hanya menutup pratinjau.

**SUARA** (hanya diputar di jendela konfirmasi kecil)

- **Suara saat terbuka**: suara ketika jendela konfirmasi muncul.
- **Detik hitung mundur**: bunyi setiap detik pada 10 detik terakhir selama Anda belum menjawab.
- **Volume**: untuk kedua suara. Tekan **▶** di samping suara untuk mendengarnya.
- **Buka folder suara**: taruh file .wav atau .mp3 Anda di sana, lalu pilih di daftar suara. **Bawaan** kembali ke
  suara bawaan. Kalau sebuah file tidak bisa diputar, suara bawaan yang diputar dan pengaturan menyebutkan file
  mana itu.
- Anda tetap mendengar suara ini meski suara game dimatikan atau game berada di latar belakang.

**JENDELA**

- **Opasitas jendela kustom**: nonaktif = jendela Pusat Dungeon mengikuti Opasitas Jendela tema Anda. Aktif = atur
  **Opasitas** sendiri untuk semuanya.

## Hotkey

Keduanya belum diikat ke tombol. Atur tombolnya di **Pengaturan Stellar → Hotkey**:

- **Buka/tutup jendela Pusat Dungeon**: membuka atau menutup Pusat Dungeon.
- **Dungeon: Alihkan pengganti konfirmasi**: menyalakan atau mematikan jendela konfirmasi kecil.

## Catatan

- Pusat Dungeon hanya bisa dibuka saat Anda berada di dunia game, dan jendelanya tertutup saat Anda logout.
- Kalau jendela kecil tidak bisa menampilkan permintaan masuk, layar konfirmasi game muncul seperti biasa.
