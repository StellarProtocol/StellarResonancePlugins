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

## Menyalin sebuah loadout

Buka jendela Loadout Switcher. Setiap loadout kecuali yang sedang kamu pakai menampilkan tombol
dengan nama loadout-mu saat ini, seperti "← Ici-LF". Klik tombol itu di baris yang ingin kamu
timpa, periksa pertanyaannya ("Timpa dengan Ici-LF?"), lalu tekan ✓.

- Ini menyalin apa yang sedang kamu pakai sekarang: gear, modul, skill, talent, Battle Imagine, dan
  ikatan Deep-Slumber. Nama loadout tetap sama.
- Jika kamu punya perubahan yang belum disimpan, atau penyalinan ini akan mengubah kelas loadout
  tersebut, konfirmasi akan menampilkan baris peringatan terlebih dahulu.
- Game akan mengonfirmasi dengan pesan bawaannya sendiri "Loadout saved successfully!". Jika game
  menolak (misalnya saat bertarung), tidak ada yang diubah.

## Tips

- Aktifkan **Jangan teruskan hotkey ke game** di bagian atas panel Hotkey agar tombol yang kamu
  ikat tidak ikut memicu aksi game saat kamu beralih loadout.

## Umpan balik

Hasil peralihan menggunakan banner notifikasi bawaan game: banner sukses saat plan diterapkan, dan
notifikasi pengaman saat peralihan belum bisa dilakukan sekarang (misalnya saat peralihan lain
masih berlangsung, atau API loadout belum siap).
