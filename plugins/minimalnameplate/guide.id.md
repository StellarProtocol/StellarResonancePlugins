# Nameplate Minimal

Mengganti nameplate di atas kepala bawaan game dengan badge kelas berwarna peran yang bersih dan
nama pemain opsional — digambar langsung ke dalam render pass HUD game itu sendiri, sehingga tetap
tajam dan tersembunyi dengan benar di balik geometri dunia.

![Nameplate minimal](media/minimalnameplate.png)

## Cara menggunakan

1. Instal plugin lalu jalankan game dalam mode **Dengan mod**.
2. Buka jendela **Nameplate** dari overlay Stellar dan ubah chip di bagian header menjadi **Aktif**.
   Header akan menampilkan **Aktif · nameplate game disembunyikan**; ubah ke **Mati** untuk kembali
   ke nameplate bawaan game (**Mati · memakai nameplate game**).
3. Di bagian **Tampil di atas pemain**, klik chip untuk mengubah setiap item menjadi **Aktif** atau
   **Mati**:
   - **Lencana kelas** — lencana berwarna peran di atas setiap pemain.
   - **Nama pemain** — nama pemain di bawah lencana (anggota party berwarna sian).
   - **Ikon teman** / **Ikon guild** — hati kecil untuk teman, perisai kecil untuk sesama anggota guild.
   - **Penanda segel kebangkitan** — lihat *Penanda segel kebangkitan* di bawah.
4. Di bagian **Ukuran**, geser untuk mengatur:
   - **Ukuran lencana** / **Ukuran nama** — skalakan keduanya secara independen.
   - **Maks. pemain** — berapa banyak pemain terdekat yang mendapat nameplate. Turunkan jika FPS
     turun di tempat yang ramai.

Setiap pengaturan punya tombol **?** di sebelahnya yang menjelaskan fungsinya.

## Warna lencana

- **Merah**, **hijau**, atau **biru** — peran pemain.
- **Oranye** — kelas yang tidak punya warna peran (dulu tampil abu-abu).
- **Abu-abu** — pemain sedang mati. Pemain yang mati juga namanya berwarna merah.

## Penanda segel kebangkitan

Setelah skill peran **"Renewal Prayer"** atau Imagine **"Arcane! Prayer of Life"** menghidupkan
seseorang, orang itu mendapat **"Weakened: Wish Sealed"** dan tidak bisa dibangkitkan dengan cara
itu lagi selama sekitar 60 detik (lebih lama di dungeon party Nightmare).

Jika ia mati selama waktu itu, lencananya menjadi ungu dan namanya menjadi ungu dengan sisa detik —
misalnya **Pemain (42s)** — sehingga healer tahu agar tidak menyia-nyiakan kebangkitan untuknya.
Matikan lewat chip **Penanda segel kebangkitan**.

![Penanda segel kebangkitan](media/revive-sealed.png)

## Perilaku

Pelat ini mengikuti aturan visibilitas nameplate bawaan game itu sendiri — sakelar HUD global
(HideUI, cutscene, mode foto, menu), pengaturan info-atas-kepala per tipe, dan penyembunyian per
entitas semuanya berlaku, sehingga tidak ada yang ditampilkan di tempat yang memang tidak
ditampilkan game itu sendiri.

Untuk menjaga kepalamu sendiri tetap bersih, nonaktifkan pengaturan info-atas-kepala bawaan game
untuk karaktermu — pelat ini mengikuti pengaturan tersebut.
