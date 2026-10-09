# Nameplate Minimal

Mengganti nameplate di atas kepala bawaan game dengan badge kelas berwarna peran yang bersih dan
nama pemain opsional — digambar langsung ke dalam render pass HUD game itu sendiri, sehingga tetap
tajam dan tersembunyi dengan benar di balik geometri dunia.

![Minimal nameplates](media/minimalnameplate.png)

## Cara menggunakan

1. Instal plugin lalu jalankan game dalam mode **Modded**.
2. Buka jendela **Nameplate** dari overlay Stellar dan aktifkan **Aktifkan Nameplate Minimal
   (Nonaktifkan Nameplate Game)** (ini menonaktifkan nameplate bawaan game).
3. Sesuaikan sesuai selera:
   - **Tampilkan Ikon Kelas (lencana)** — lencana berwarna peran di atas setiap pemain.
   - **Tampilkan Nama Pemain (di bawah lencana)** — nama pemain di bawah lencana.
   - **Sembunyikan Lencana + Nama milik sendiri** — menjaga kepalamu sendiri tetap bersih.
   - **Ukuran Lencana / Ukuran Nama** — skalakan keduanya secara independen.

## Perilaku

Pelat ini mengikuti aturan visibilitas nameplate bawaan game itu sendiri — sakelar HUD global
(HideUI, cutscene, mode foto, menu), pengaturan info-atas-kepala per tipe, dan penyembunyian per
entitas semuanya berlaku, sehingga tidak ada yang ditampilkan di tempat yang memang tidak
ditampilkan game itu sendiri.
