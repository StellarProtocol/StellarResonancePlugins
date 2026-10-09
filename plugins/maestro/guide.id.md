# Maestro

Pemutar otomatis MIDI untuk instrumen band (Musisi) Season 3 Star Resonance. Taruh file `.mid` Anda
di sebuah folder, pilih lagu, dan Maestro akan memainkannya untuk Anda di instrumen yang Anda panggil —
sendirian atau dalam grup — lengkap dengan playlist, sinkron grup, dan pratinjau suara lokal.

![Tampil di dunia game dengan jendela Pustaka, Pemutar Otomatis MIDI dan Pratinjau terbuka](media/maestro-in-world.png)

## Memulai

1. Pasang plugin lalu jalankan game dalam mode **Dengan mod**.
2. Buka **Maestro** dari Stellar launcher (grup Plugins). Alat band hanya bisa dipakai di dalam dunia game.
3. Taruh file `.mid` / `.midi` Anda di folder `midi\` milik game — Maestro akan membuatnya dan
   menampilkan path-nya di jendela **Pustaka**. Gunakan **Pindai ulang folder** atau **Buka lokasi**
   jika Anda memindahkannya.
4. Panggil instrumen mode bebas-main di dalam game, tambahkan lagu dari **Pustaka** ke antrean, lalu
   tekan ▶.

## Memutar lagu

Jendela utama adalah pemutar Anda: playlist, antrean yang sedang diputar, dan kontrol transport.

![Pemutar Otomatis MIDI — playlist, antrean dan kontrol pemutaran](media/auto-player.png)

- Buat **playlist** bernama dan susun ulang antrean; header menampilkan lagu dan posisi saat ini.
- **Maju otomatis**, **Ulang** (mati / semua / satu), **Acak**, dan **jeda antar lagu** semuanya tinggal
  satu klik.
- Baris status menampilkan posisi langsung dan jumlah not untuk lagu yang sedang diputar.

## Mencari lagu

![Pustaka — jelajahi folder MIDI Anda dan tambahkan lagu ke antrean](media/library.png)

- Jelajahi seluruh folder MIDI Anda, **cari** berdasarkan nama, dan klik lagu untuk menambahkannya ke
  antrean.
- Pemutaran hanya memainkan satu instrumen dalam satu waktu, jadi cukup antrekan stem yang ingin Anda
  mainkan.

## Pratinjau sebelum memutar

Dengarkan lagu lewat **suara instrumen asli dalam game** tanpa memanggil instrumen — pratinjau hanya
diputar untuk Anda sendiri, jadi orang di sekitar Anda tidak akan mendengarnya.

![Pratinjau MIDI (Lokal) — dengarkan lewat suara instrumen asli dalam game](media/preview.png)

- Satu baris per stem, dengan **bisukan** per bagian dan mode **sustain**, jadi Anda bisa mendengar
  persis bagaimana hasil akhir lagunya.
- **Sinkron Instrumen** menunggu pemutar band yang sedang live lalu mengikutinya, membisukan bagian
  yang akan Anda mainkan sendiri — berguna untuk jamming bareng orang lain.

Pratinjau adalah tempat penamaan multi-stem jadi penting: beri bagian-bagian sebuah lagu nama dasar
yang sama, diakhiri instrumen dalam tanda kurung, dan Pratinjau akan memuat **seluruh set** saat Anda
memilih salah satunya.

```
Song (Piano).mid   Song (Guitar).mid   Song (Bass).mid   Song (Bass 2).mid   Song (Drum).mid
```

Duplikat seperti `(Bass 2)` menjadi trek tersendiri pada suara instrumen yang sama.

## Kontrol per lagu & bermain berkelompok

Buka **Pengaturan** untuk penyesuaian detail dan opsi grup.

![Pengaturan — kontrol per lagu, Sinkron Jaringan dan opsi Ensembel](media/settings.png)

- **Per lagu**: Transpose, Tahan not, Tempo %, Not maksimum (batas polifoni), Jeda pukul-ulang, Volume
  monitor, Paksa sustain, dan Terapkan tone/teknik dari MIDI.
- **Sinkron Jaringan** mengalirkan not Anda lebih awal sehingga pendengar di sekitar Anda mendengar
  penampilan yang lebih stabil saat not sedang padat.
- **Ensembel**: kunci pemutaran ke ketukan bersama grup Anda (hitung-masuk ke downbeat), opsional
  cocokkan tempo ensembel, dan terima otomatis undangan supaya semua orang mulai bersamaan. Gabung
  atau mulai ensembel di dalam game dulu.

## Bermain dalam ensembel

Mode ensembel mengunci penampilan seluruh party ke ketukan yang sama, sehingga beberapa pemain bisa
memainkan bagian berbeda dari lagu yang sama secara bersamaan dan tersinkron. Semua orang harus
**berada dalam party yang sama**.

1. **Nyalakan sinkron ensembel.** Di **Pengaturan**, aktifkan **Sinkron ke ensembel** — di setiap
   pemain. Menyalakan **Terima otomatis undangan ensembel** juga bersifat opsional, supaya Anda tak
   perlu menerima tiap undangan secara manual.
2. **Tiap pemain memilih bagiannya dan menekan ▶.** Pilih stem yang akan Anda mainkan dan tekan
   putar — alih-alih langsung mulai, Maestro akan menahan dan menampilkan **"menunggu ensembel…"**.
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

Nyalakan **Terapkan tone / teknik dari instrumen MIDI** di Pengaturan, dan Maestro akan memilih efek
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
distortion Anda di kedua mode tersebut. Tidak ada pengaturan Maestro yang mengubah ini; perbaikannya
ada di tangan game. *Teknik* (Muffled, Harmonics, Slap) tidak terpengaruh dan didengar oleh semua
orang. Untuk lagu yang distortion-nya krusial, mainkan dengan Sinkron Jaringan **mati** agar
setidaknya terdengar benar bagi Anda.
