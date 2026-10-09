# Studio Foto

Ambil screenshot yang indah: sembunyikan antarmuka, gerakkan kamera bebas ke mana saja di
sekitar adegan, bekukan momennya, pose-kan siapa saja, foto dalam bentuk potret atau persegi,
tambahkan tampilan dan cahaya — lalu simpan foto beresolusi tinggi.

![Panel Studio Foto](media/photostudio-inworld.png)

## Mulai cepat

1. Pasang plugin-nya lalu jalankan game dalam mode **Dengan mod**.
2. Tekan **Shift+F10** untuk membuka panel Studio Foto.
3. Tekan **F10** untuk mengambil foto. Foto disimpan ke folder screenshot-mu (tab Tangkap →
   Folder).
4. **Ctrl+F10** menyembunyikan semuanya untuk hasil yang bersih.

Semua hotkey bisa diubah di **Pengaturan Stellar → Hotkey**.

## Tab-tabnya

- **Tangkap** — resolusi (1×, 2× atau 4× layarmu), **bentuk** foto (Layar, potret 9:16, 4:5,
  2:3, persegi 1:1, lebar 21:9), PNG atau JPG, foldernya, dan apa yang disembunyikan saat
  memotret.
- **Tampilan** — warna, exposure, kontras, white balance, kedalaman ruang dan lainnya. Simpan
  tampilan favoritmu sebagai preset.
- **Kamera** — kamera bebas, adegan (bekukan / reset), pengaturan gerakan, dan pose orang.
- **Cahaya** — lampu, dan key light serta rim untuk satu orang.
- **Preset** — muat, simpan, ganti nama, dan bagikan tampilanmu.

## Menyembunyikan sesuatu

Di grup **Sembunyikan** pada tab **Tangkap**, kamu akan menemukan daftar yang sama dengan layar
foto game itu sendiri, dengan urutan yang sama:

- **Saya** dan **Spirit Echo saya** menyembunyikan karaktermu dan Spirit Echo-mu — hanya di
  layarmu sendiri. Kamu masih bisa bergerak dan bertarung, dan pemain lain tetap melihatmu.
- **Petualang lain**, **Non-pemain**, **Musuh**, **Koleksi** dan **Spirit Echo lain**
  menyembunyikan itu semua di layarmu.
- **Teman**, **Party** dan **Guild** benar-benar menyembunyikan pemain itu, bahkan saat
  Petualang lain masih ditampilkan. Grup yang disembunyikan selalu menang: rekan guild yang
  juga temanmu disembunyikan saat kamu menyembunyikan guild-mu. Anggota party tetap terlihat
  selama kamu menampilkan party.
- **Senjata** menyembunyikan senjata semua pemain, bukan cuma punyamu.
- **Efek** menyembunyikan efek skill, buff dan hit berdasarkan siapa penyebabnya: **Milikku**
  (milikmu, milik pet-mu dan milik Battle Imagine-mu), **Party**, **Pemain lain** dan
  **Monster** (termasuk area peringatan bos). Scenery seperti air terjun dan lampu tidak pernah
  disembunyikan. Beberapa efek yang tidak dikaitkan game dengan siapa pun tetap selalu terlihat.

Ini berlaku selama panel terbuka dan di setiap foto. Di tab **Kamera**, **Sembunyikan saya saat
masuk** dan **Sembunyikan efek saat masuk** melakukan hal yang sama begitu kamera bebas
dinyalakan (efeknya mengikuti switch di tab Tangkap). Mematikannya — atau menutup Studio Foto —
mengembalikan semuanya. Pengaturan sembunyikan dari versi 1.6.0-mu tetap terbawa. **Ctrl+F10**
tetap menyembunyikan semuanya, termasuk overlay Stellar.

## ReShade

Studio Foto bisa memakai efek **ReShade** — di layar dan di foto-fotomu.

1. Di launcher Stellar, buka halaman Studio Foto dan biarkan **ReShade** tercentang di bawah
   **Dependensi** (di Linux / Proton, biarkan juga **Microsoft shader compiler** tercentang).
   Launcher akan mengunduh dan memeriksanya sebelum game dimulai.
2. Jalankan game dalam mode **Dengan mod**. ReShade hanya dipakai saat Dengan mod; peluncuran
   **Vanilla** tetap tanpa ReShade.
3. Buka tab **Tampilan** → grup **ReShade**: nyalakan **Gunakan ReShade**, pilih **Preset**
   (atau **Tidak ada** untuk tanpa efek) dan nyalakan/matikan efeknya.
4. Di bawah **Preset**, tambahkan tampilan siap pakai dengan satu klik, dan di bawah **Paket
   shader** unduh paket efek yang mereka pakai. Preset yang sudah diunduh langsung muncul di
   daftar Preset.

Sesuaikan tiap efek di menu ReShade-nya sendiri (default-nya **Home**). Foto potret, persegi
dan lebar melewatkan efek yang butuh kedalaman; Studio Foto memberitahumu saat itu terjadi.

## Kamera bebas

Tekan **Alt+F10** (atau tombol **Kamera bebas**) untuk mengambil alih kamera.

- **Orbit** (default): **klik kanan mouse** berputar mengelilingi orang, **scroll mouse**
  mendekat atau menjauh, **Q / E** turun / naik. **Klik** karakter lain untuk orbit ke mereka;
  **Backspace** kembali ke dirimu.
- **Terbang**: tekan **Tab**, lalu **WASD** untuk bergerak, **klik kanan mouse** untuk melihat
  sekeliling, **Q / E** untuk turun / naik.
- **Z / C** memiringkan kamera, **Shift + scroll** zoom (field of view), **R** kembali ke
  tampilan game.
- **]** zoom in dan **[** zoom out (tahan untuk terus), **\\** mereset field of view. Tab
  **Tampilan** juga punya slider **Sudut pandang** (10–100°). Tombol-tombolnya ditampilkan di
  layar.
- **Space** membekukan adegan, **H** menyembunyikan petunjuk tombol, **Esc** atau **Alt+F10**
  keluar dari kamera bebas.

## Membekukan adegan

**Space** di kamera bebas (atau **Bekukan** di tab Kamera) menjeda seluruh game di layarmu —
semua orang, semua skill dan efek, dan karaktermu sendiri. Keluar dari kamera bebas membuat
dunia tetap beku; tekan **Lanjutkan** atau **Space** lagi untuk melanjutkan. Game tetap berjalan
di server: pertarungan terus berlangsung, dan apa pun yang terjadi akan terlihat saat kamu
melanjutkan. Monster yang terbunuh saat beku tetap ada di layar sampai kamu melanjutkan.

## Memose orang

Di grup **Orang** pada tab Kamera, pilih seseorang dengan **‹ ›** (atau klik mereka di kamera
bebas):

- **Pose** — pilih emote dan seret **Momen** ke frame yang tepat; ▶ / ❚❚ memutar atau
  menahannya.
- **Ekspresi** — ekspresi wajah yang bertahan sampai kamu mengubahnya.
- **Kepala / Mata** — melihat ke kamera (Lensa), bebas (Bebas) atau normal, dan
  menguncinya. Di **Bebas**, seret titik di grid untuk membidik, atau geser dengan panah;
  **Langkah** (Halus · Normal · Kasar) mengatur seberapa jauh tiap tekan panah bergerak.
- **Putar** — memutar mereka menghadap ke arah yang kamu mau.

Pemain lain dan NPC di-pose sebagai salinan yang hanya kamu bisa lihat; orang aslinya
disembunyikan sampai kamu mereset adegan.

## Cahaya

Di tab **Cahaya**:

- **＋ Lampu di kamera** (atau **L** di kamera bebas) menaruh lampu di posisi kamera;
  **Shift+L** memindahkan lampu yang dipilih ke sana. Sampai 8 lampu, masing-masing dengan
  warna, kekuatan dan jangkauan sendiri, ditempatkan di sekitar orang yang dipilih.
- **Cahaya orang** mengatur seberapa kuat lampu mewarnai karakter (orang lain di dekat lampu
  mana pun juga ikut terwarnai).
- **Cahaya orang** memberi satu orang **key light** dari sisi yang dipilih dan **rim** berwarna
  di rambut, penutup kepala dan senjata.

## Mengakhiri adegan

**Atur ulang adegan** (tab Kamera) membatalkan pembekuan, menghapus salinan yang di-pose dan
cahaya, dan mengembalikan semua orang ke normal. Pindah zona, cutscene, atau mematikan Studio
Foto melakukan hal yang sama.

## Meminimalkan dan menutup

- **–** di panel mengecilkannya jadi strip kecil tanpa mengubah apa pun.
- **✕** di panel atau strip, atau **Ctrl+Shift+F10**, menutup Studio Foto sepenuhnya: keluar
  dari kamera bebas, membatalkan pembekuan, mereset orang dan lampu yang di-pose, dan
  menampilkan semua yang kamu sembunyikan. Kalau ada salah satu dari itu yang masih berjalan,
  ia akan bertanya dulu dan menyebutkan apa yang akan berakhir.
- Saat Studio Foto ditutup, pil adegan dan kamera menunjukkan tombol untuk membukanya lagi.
