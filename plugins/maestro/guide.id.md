# Maestro

Pemutar otomatis MIDI untuk instrumen band (Musisi) Season 3 Star Resonance. Taruh file `.mid` Anda
di sebuah folder, pilih lagu, dan Maestro akan memainkannya untuk Anda di instrumen yang Anda panggil —
sendirian atau dalam grup — lengkap dengan playlist, sinkron grup, dan pratinjau suara lokal.

![Tampil di dunia game dengan jendela Maestro terbuka](media/maestro-in-world.png)

## Memulai

1. Pasang plugin lalu jalankan game dalam mode **Dengan mod**.
2. Buka **Maestro** dari Stellar launcher (grup PLUGIN). Alat band hanya bisa dipakai di dalam dunia game.
3. Taruh file `.mid` / `.midi` Anda di folder `midi\` milik game — Maestro membuatkannya untuk Anda.
   Di **Pustaka**, **Buka folder** langsung membuka folder itu, dan **Pindai ulang** memuat file yang
   baru Anda tambahkan.
4. Panggil instrumen mode bebas-main di dalam game, tekan **+ Tambah** untuk memilih lagu dari Pustaka,
   lalu tekan **Putar**.

## Maestro Link — main sebagai band dari situs web

Dengan Maestro Link, satu band bisa bermain bersama tanpa ensembel di dalam game. Ketua band
menjalankan room di situs [maestro.stellarresonance.app](https://maestro.stellarresonance.app),
memilih lagu dan menentukan siapa memainkan bagian apa — lalu karakter setiap pemain memasang
instrumennya dan memainkan bagiannya sendiri, sinkron dengan anggota band lainnya.

![Memainkan lagu Maestro Link dengan piano di dunia game](media/link-in-world.png)

### Bergabung ke room

1. Dapatkan **kode room 6 karakter** — buat room di situs web, atau minta ke ketua band Anda.
2. Di dalam game, buka **Maestro** → **Maestro Link**.
3. Ketik kode di kotak lalu tekan **Gabung**.
4. Selesai — saat room memutar lagu, karakter Anda memainkan bagiannya secara otomatis.

Panel **Cara kerja** merangkum langkah-langkah ini, dan **Buka situs web** membuka situs Maestro Link.

![Maestro Link — masukkan kode room lalu tekan Gabung](media/link-join.png)

### Di dalam room

![Maestro Link di dalam room — info room, lagu yang diputar, dan daftar pemain](media/link-connected.png)

- **Ruang** menampilkan kode dan nama room, ping Anda, dan bagian yang Anda mainkan.
- **Sedang diputar** menampilkan lagu saat ini, progresnya, dan lagu **Berikutnya**.
- **Pemain** mencantumkan semua orang di room beserta nama, instrumen, status, dan ping. Statusnya
  **Main**, **Siap**, **Siaga**, atau **Masalah**. *Masalah* hanya muncul saat bagian yang sedang
  diputar saat itu gagal dimainkan — belum memegang instrumen tidak apa-apa, karena Link akan
  memasangkannya untuk lagu berikutnya. **Sembunyikan** menciutkan daftar.
- **Hentikan bagian saya** (oranye) mengistirahatkan bagian Anda sementara band tetap bermain;
  tombolnya lalu berubah menjadi **Lanjutkan bagian saya**.
- **Putuskan** (merah) keluar dari room. Saat Anda logout, koneksi otomatis terputus.

### Situs web

Halaman room di situs web adalah tempat band dijalankan: menampilkan semua pemain yang bergabung,
**Now playing** (jeda, stop, lewati, ulangi), dan antrean lagu tempat Anda memilih siapa memainkan
setiap bagian. Satu bagian bisa diberikan ke beberapa pemain, dan satu pemain bisa mengambil
beberapa bagian dari instrumen yang sama. **Room settings** berlaku untuk semua pemain sekaligus, dan
Anda tetap bisa menyesuaikan tiap pemain secara terpisah.

![Situs Maestro Link — pemain, Now playing, dan antrean lagu](media/web-room.png)

## Memutar lagu

Jendela utama adalah pemutar Anda. Di bagian atas, **Sedang diputar** menampilkan lagu saat ini dan
progresnya, dengan tombol ◀◀ / **Putar** (**Jeda**) / ▶▶ / **Stop** serta baris **Berikutnya:** yang
memberi tahu lagu apa yang menyusul.

![Jendela utama — Sedang diputar, antrean, dan empat kartu](media/auto-player.png)

**Antrean**

- Pilih playlist dari dropdown, atau gunakan **+ Baru**, **Ganti nama**, dan **✕** (hapus playlist).
- **Seret lagu** ke atas atau ke bawah untuk mengubah urutannya, dan tekan **✕** di sebuah baris untuk
  menghapusnya.
- Berpindah ke playlist lain tidak menghentikan lagu yang sedang diputar — lagu itu terus berjalan
  sampai Anda memilih lagu di playlist lain dan menekan **Putar**.
- Ikon di bawah antrean menyalakan **Lanjut otomatis**, **Ulang** (mati / semua lagu / satu lagu),
  dan **Acak**. Arahkan kursor ke sebuah ikon, maka nama dan setelannya muncul di judul **Antrean**.
- **Acak** memutar setiap lagu di playlist satu kali dengan urutan acak. Jika Ulang mati, pemutaran
  berhenti setelah lagu terakhir; jika Ulang semua, urutannya diacak ulang di setiap putaran. Baris
  **Berikutnya:** selalu menampilkan lagu yang benar-benar akan diputar berikutnya.
- **+ Tambah** membuka Pustaka.

Di bagian bawah, empat kartu membuka jendela lain: **Pustaka**, **Maestro Link**, **Pratinjau**, dan
**Pengaturan**. Tiap kartu juga menampilkan status singkat, seperti jumlah lagu atau apakah Anda
sedang berada di room.

## Mencari lagu

![Pustaka — cari lagu dan klik untuk menambahkannya ke playlist](media/library.png)

- **Cari** berdasarkan nama (✕ menghapus pencarian), **Pindai ulang** untuk memuat file baru, dan
  **Buka folder** untuk langsung ke folder MIDI Anda.
- **Klik sebuah lagu** untuk menambahkannya ke playlist saat ini. Barisnya berkedip hijau dengan
  tulisan **Ditambah**, dan baris di bawah mengonfirmasinya, misalnya *✓ “Bad Apple (Piano)”
  ditambahkan ke Playlist 1 · 11 lagu*.
- Pemutaran hanya memainkan satu instrumen dalam satu waktu, jadi cukup tambahkan stem yang ingin Anda
  mainkan.

## Pratinjau sebelum memutar

Dengarkan lagu lewat **suara instrumen asli dalam game** tanpa memanggil instrumen — pratinjau hanya
diputar untuk Anda sendiri, jadi orang di sekitar Anda tidak akan mendengarnya.

![Pratinjau — lagu yang dipratinjau, opsi sinkron band, dan satu baris per stem](media/preview.png)

- **Pratinjau** di bagian atas menampilkan lagu antrean yang dipilih beserta progresnya, dengan tombol
  **Pratinjau** dan **Stop**.
- **Sinkron dengan band**: **Sinkron instrumen** menunggu permainan band Anda dimulai lalu ikut
  bermain bersamanya — bisukan bagian yang Anda mainkan sendiri. **Ofset sinkron** menunda pratinjau
  agar pas dengan yang didengar band; **= Delay band** mengaturnya untuk Anda.
- **Stem** berisi satu baris per file, masing-masing dengan sakelar bisu, instrumennya, mode
  **Sustain**, dan sakelar **Tone**, jadi Anda bisa mendengar persis bagaimana hasil akhir lagunya.

Pratinjau adalah tempat penamaan multi-stem jadi penting: beri bagian-bagian sebuah lagu nama dasar
yang sama, diakhiri instrumen dalam tanda kurung, dan Pratinjau akan memuat **seluruh set** saat Anda
memilih salah satunya.

```
Song (Piano).mid   Song (Guitar).mid   Song (Bass).mid   Song (Bass 2).mid   Song (Drum).mid
```

Duplikat seperti `(Bass 2)` menjadi trek tersendiri pada suara instrumen yang sama.

## Kontrol per lagu & bermain berkelompok

Semua penyesuaian detail dan opsi grup ada di satu jendela dua kolom yang dibuka dari kartu
**Pengaturan**. Setiap pengaturan punya tombol bulat **?** yang membuka penjelasan singkat — klik lagi,
atau ✕-nya, untuk menutup. Setiap slider juga punya tombol **↺** untuk mengembalikannya ke nilai
bawaan.

![Pengaturan — Pemutaran, Ensembel, Suara, dan Sinkron jaringan, dengan bantuan Volume monitor terbuka](media/settings.png)

- **Pemutaran**: Transpose, Tempo, Tahan not, Not maksimum (berapa banyak not yang boleh berbunyi
  bersamaan), Jeda pukul-ulang (bawaan 33 ms, agar not berulang yang cepat tidak hilang), dan Jeda
  antarlagu.
- **Ensembel**: **Sinkron ke ensembel** mengunci pemutaran ke ketukan bersama grup Anda (hitung-masuk
  ke downbeat), **Cocokkan tempo ensembel** mengikuti BPM ensembel, dan **Terima otomatis undangan
  ensembel** menerima undangan untuk Anda. Gabung atau mulai ensembel di dalam game dulu.
- **Suara**: Volume monitor, Paksa sustain, dan Terapkan nada dari instrumen MIDI.
- **Sinkron jaringan**: **Mode Sinkron Jaringan** mengalirkan not Anda lebih awal sehingga pendengar
  di sekitar Anda mendengar penampilan yang lebih stabil, bahkan saat not sedang padat. Mode ini juga
  membuka **Bisukan suara lokal saya** dan **Tampilkan animasi tuts**. Atur lebih detail lewat
  **Lihat-ke-depan**, **Kirim tiap**, dan **Lead nada**. **Solusi relay nada (eksperimental)**
  dijelaskan di *Batasan yang diketahui* di bawah.

![Bagian Sinkron jaringan di Pengaturan](media/network.png)

## Bermain dalam ensembel

Mode ensembel mengunci penampilan seluruh party ke ketukan yang sama, sehingga beberapa pemain bisa
memainkan bagian berbeda dari lagu yang sama secara bersamaan dan tersinkron. Semua orang harus
**berada dalam party yang sama**.

1. **Nyalakan sinkron ensembel.** Di **Pengaturan**, aktifkan **Sinkron ke ensembel** — di setiap
   pemain. Menyalakan **Terima otomatis undangan ensembel** juga bersifat opsional, supaya Anda tak
   perlu menerima tiap undangan secara manual.
2. **Tiap pemain memilih bagiannya dan menekan Putar.** Pilih stem yang akan Anda mainkan dan tekan
   putar — alih-alih langsung mulai, Maestro akan menahan dan menampilkan **"Menunggu ansambel…"**.
3. **Pemimpin party memulai ensembel di dalam game.** Ini adalah permulaan ensembel milik game sendiri.
4. **Semua orang bermain tersinkron.** Semua pemain yang menunggu mulai bersamaan di downbeat,
   terkunci ke ketukan ensembel.

Untuk band lengkap, mintalah tiap pemain mengantrekan bagian yang **berbeda** (Gitar, Bass, Drum,
Piano) dari lagu yang sama, dan opsional nyalakan **Cocokkan tempo ensembel** supaya pemutaran
mengikuti BPM ensembel. Gunakan **Pratinjau** terlebih dahulu untuk mendengar bagaimana seluruh set
cocok satu sama lain.

## Menyiapkan file MIDI Anda

Maestro memainkan MIDI Anda persis seperti yang tertulis, jadi sedikit persiapan membuat lagu terdengar
pas di instrumen game.

### Efek (tone & teknik)

Nyalakan **Terapkan nada dari instrumen MIDI** di Pengaturan, dan Maestro akan memilih efek
gitar/bass dari **instrumen (program) yang ditetapkan pada trek stem tersebut**. Atur instrumen
General MIDI trek itu di DAW Anda:

**Stem gitar**

| Tetapkan instrumen GM ini | Nomor Program | Diputar sebagai |
|---|:--:|---|
| Nylon / Steel / Jazz / Clean Electric Guitar | 25–28 | Clean |
| Muted Guitar | 29 | Muffled (palm-mute) |
| Overdriven Guitar | 30 | Overdrive |
| Distortion Guitar | 31 | Distortion |
| Guitar Harmonics | 32 | Harmonics |

**Stem bass**

| Tetapkan instrumen GM ini | Nomor Program | Diputar sebagai |
|---|:--:|---|
| Acoustic / Finger / Pick / Fretless Bass | 33–36 | Clean |
| Slap Bass 1 / 2 | 37 / 38 | Slap |
| Synth Bass 1 / 2 | 39 / 40 | Overdrive |

- Nomor program adalah nilai **1–128** yang ditampilkan DAW Anda; cocokkan lewat **nama instrumen**
  jika ragu.
- Hanya **instrumen utama** stem yang dibaca, jadi jaga satu instrumen per stem. Perubahan program di
  tengah trek mengganti efek mulai dari titik itu.
- Efek berlaku **hanya untuk gitar dan bass** — piano dan drum mengabaikannya.
- Efek disesuaikan dengan instrumen yang benar-benar Anda panggil. **Bass tidak punya distortion** —
  ia diputar sebagai Overdrive — dan teknik apa pun yang tak bisa dilakukan instrumen yang dipanggil
  kembali ke normal.
- **Overdrive / Distortion hanya lokal saat Sinkron Jaringan** (lihat *Batasan yang diketahui* di
  bawah). Muffled, Harmonics dan Slap didengar oleh semua orang.

### Drum

Drum kit game adalah **kit 9 bagian** tetap pada tuts di bawah ini, dan Maestro memainkan not Anda
persis seperti yang tertulis — ia **tidak** mengonversi drum General MIDI secara otomatis — jadi
stem drum harus memakai tuts berikut:

| Not MIDI | Bagian |
|:--:|---|
| 62 (D4) | Hi-Hat Tertutup |
| 65 (F4) | Kick |
| 69 (A4) | Floor Tom |
| 72 (C5) | Snare |
| 74 (D5) | Mid Tom |
| 76 (E5) | High Tom |
| 77 (F5) | Ride |
| 79 (G5) | Hi-Hat Terbuka |
| 81 (A5) | Crash |

Not di luar tuts ini akan diam. Mulai dari trek drum General MIDI standar? Petakan ulang not GM biasa
ke tuts-tuts ini — kick (GM 35/36) → **F4**, snare (38/40) → **C5**, hi-hat tertutup (42) → **D4**,
hi-hat terbuka (46) → **G5**, ride (51) → **F5**, crash (49) → **A5**, tom → **A4 / D5 / E5**.

### Beberapa tips lagi

- **Satu not per nada:** tiap instrumen hanya berbunyi satu suara per nada, jadi dua not identik yang
  tumpang tindih dihitung sebagai satu — hindari unison yang bertumpuk.
- **Perhatikan rentang:** not di luar rentang mainkan instrumen akan diam. Gunakan **Transpose**
  (Pengaturan) untuk membawa bagian ke dalam rentang.
- **Volume tidak direproduksi:** tiap not diputar dengan kekerasan tetap, jadi velocity dan dinamika
  tidak akan terbawa.
- File MIDI **tipe 0 atau 1** didukung.

## Batasan yang diketahui

**Overdrive / Distortion tidak sampai ke pemain lain — ini bug game, bukan sesuatu yang bisa
diperbaiki Maestro.** Game hanya pernah merender *tone* overdrive/distortion gitar/bass di klien Anda
sendiri dan tidak pernah mengirimkannya ke orang di sekitar Anda. Jadi ia diputar Clean untuk semua
orang lain kapan pun **Sinkron Jaringan menyala**, dan bahkan dengan Sinkron Jaringan **mati**,
hanya *Anda* yang mendengar distortion-nya — pemain lain tidak pernah mendengar overdrive/
distortion Anda di kedua mode tersebut. **Solusi relay nada (eksperimental)** di Pengaturan
mengirim nada Anda lewat jalur lain, tetapi hanya pendengar yang juga memakai Stellar yang
mendengarnya; pemain lain tetap mendengar Clean sampai game memperbaikinya. *Teknik* (Muffled, Harmonics, Slap) tidak terpengaruh dan didengar oleh semua
orang. Untuk lagu yang distortion-nya krusial, mainkan dengan Sinkron Jaringan **mati** agar
setidaknya terdengar benar bagi Anda.
