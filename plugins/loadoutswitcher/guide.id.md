# LoadoutSwitcher

Beralih antar loadout in-game yang kamu simpan (Role Plan) dengan hotkey.

## Cara menggunakan

1. Instal plugin lalu jalankan game dalam mode **Dengan mod**.
2. Simpan loadout-mu di dalam game seperti biasa (Role Plan).
3. Buka **Pengaturan Stellar → Hotkey** dan ikat tombol untuk **Terapkan Loadout 1** sampai
   **Terapkan Loadout 10** (belum diikat secara default). Hotkey ke-*n* menerapkan loadout
   ke-*n* dalam daftar tersimpanmu.

![Hotkey bindings](media/loadoutswitcher-hotkeys.png)

4. Tekan hotkey di dunia game — plugin beralih melalui alur loadout bawaan game itu sendiri.

## Jendela

Buka jendela Loadout Switcher untuk melihat baris status di atas (pemulihan nyala atau mati,
dan loadout yang sedang kamu pakai), sakelar **Pulihkan faktor**, dan daftar loadout-mu.
Loadout yang sedang kamu pakai ditandai dengan titik. Tekan tombol **?** untuk bantuan bagian itu.

![Loadout Switcher window](media/loadoutswitcher-window.png)

## Memulihkan faktor

Setiap loadout bisa mengingat setelan Deep-Slumber (jalur kultivasi dan faktornya).

1. Pakai loadout tersebut, atur Deep-Slumber sesuai keinginanmu, lalu tekan **Ikat ini** di
   baris loadout itu. Baris tersebut lalu menampilkan jumlah area dan faktor yang terikat.
2. Ubah **Pulihkan faktor** menjadi **Nyala**. Sekarang, beralih ke sebuah loadout akan
   memulihkan faktor yang terikat padanya.
3. Menekan hotkey loadout yang sedang kamu pakai akan memulihkan faktornya sekali lagi.

- Pemulihan hanya berjalan di **Season 3**. Di season lain fitur ini tetap mati dan jendela
  memberitahumu. Ikatanmu tetap tersimpan, dan beralih loadout tetap bisa.
- Tekan **✕** di sebuah baris untuk menghapus ikatannya. Loadout-nya sendiri tetap ada.

## Menyalin sebuah loadout

1. Pakai loadout yang ingin kamu salin.
2. Tekan **Salin kemari** di baris yang ingin kamu timpa.
3. Baris itu bertanya "Timpa *baris* dengan *yang dipakai*?" (misalnya "Timpa Tank dengan DPS?").
   Tekan **Timpa**, atau **Batal** untuk membiarkannya apa adanya.

- Ini menyalin apa yang sedang kamu pakai sekarang: gear, modul, skill, talent, Battle Imagine, dan
  ikatan Deep-Slumber. Nama loadout tetap sama.
- Jika kamu punya perubahan yang belum disimpan, atau penyalinan ini akan mengubah kelas loadout
  tersebut, baris peringatan muncul di bagian bawah jendela sebelum kamu mengonfirmasi.
- Game akan mengonfirmasi dengan pesan bawaannya sendiri "Loadout saved successfully!". Jika game
  menolak (misalnya saat bertarung), tidak ada yang diubah.

## Tips

- Aktifkan **Jangan teruskan hotkey ke game** di bagian atas panel Hotkey agar tombol yang kamu
  ikat tidak ikut memicu aksi game saat kamu beralih loadout.

## Umpan balik

Hasil peralihan menggunakan banner notifikasi bawaan game: banner sukses saat plan diterapkan, dan
notifikasi pengaman saat peralihan belum bisa dilakukan sekarang (misalnya saat peralihan lain
masih berlangsung, atau API loadout belum siap).
