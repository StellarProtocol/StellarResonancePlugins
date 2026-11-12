# CooldownBar

Cooldown skill, debuff, dan buff-mu dalam sekali lihat, jadi kamu bisa tahu apa yang akan siap
lagi tanpa harus menatap hotbar. Tampilkan semuanya di satu bilah, atau beri setiap efek tile
sendiri di mana saja di layar.

![Bilah cooldown](media/cooldownbar-bar.png)

## Cara pakai

1. Pasang plugin-nya lalu jalankan game dalam mode **Dengan mod**.
2. Bilah akan muncul sebagai overlay. Seret ke mana pun kamu mau — posisinya akan diingat.
3. Setiap efek yang aktif tampil sebagai tile: sian untuk cooldown skill, merah untuk debuff,
   hijau untuk buff. Bilah isi dan angkanya menunjukkan sisa waktu; chip kecil menunjukkan
   jumlah stack atau charge.
4. Tekan **F8**, klik ikon roda gigi di bilah, atau buka CooldownBar dari menu Stellar untuk
   membuka **Pengaturan**. Pengaturan terbuka di tengah bilah.

![CooldownBar — Pengaturan](media/cooldownbar-settings.png)

## Pengaturan

- **Skill / Debuff / Buff** — berpindah di antara tiga daftar. Setiap chip menunjukkan berapa
  yang sudah kamu centang.
- **Hanya yang dicentang / Selain yang dicentang** — tampilkan hanya yang kamu centang, atau
  semuanya kecuali yang dicentang. Setiap daftar punya pilihannya sendiri.
- **Cari** di daftar; tombol hapus mengosongkan kotaknya. Saat kamu mencentang sesuatu, barisnya
  berkedip dan catatan di bagian bawah mengonfirmasinya.
- **Hapus centang** meminta konfirmasi sebelum mengosongkan daftar.
- **Mode tampilan** — **Bilah** (satu panel berisi semua efek aktif) atau **Terpisah** (satu tile
  per efek, lihat di bawah).
- **Latar** (Bilah) mengatur seberapa pekat panel bilah. Warnanya mengikuti temamu.
- **Sembunyikan saat kosong** (Bilah, mati secara bawaan) menyembunyikan bilah selama tidak ada
  yang aktif. Bilah muncul lagi saat cooldown atau efek berikutnya, dan selalu tampil di editor
  tata letak HUD. Selama tersembunyi, buka Pengaturan dengan **F8** atau dari menu Stellar.
- Tekan **?** di samping opsi untuk penjelasan singkat.

## Mode Terpisah

1. Di Pengaturan, atur **Mode tampilan** ke **Terpisah**.
2. Centang efek yang kamu mau, maksimal 30. Masing-masing jadi tile sendiri. Mode Terpisah selalu
   hanya menampilkan efek yang dicentang.
3. Buka editor tata letak HUD. Semua tile tampil di sana, jadi seret masing-masing ke tempat yang
   kamu mau.
4. Di luar editor, tile hanya muncul selama efeknya aktif.
5. **Ukuran** mengatur semua tile ke 75%, 100%, 125%, 150%, atau 200%. Untuk memberi satu tile
   ukurannya sendiri, klik chip ukuran di barisnya dalam daftar.

![CooldownBar — Terpisah](media/cooldownbar-placed.png)

## Tips

- Sisa waktu ditampilkan dalam detik sampai 99 dtk (dengan desimal di bawah 10 dtk), lalu dalam
  menit (mis. "8m"), lalu dalam jam. Stack ditampilkan sampai 99, lalu "99+".
- Buff yang me-refresh timer-nya saat dapat stack baru akan tetap terlihat dengan hitung
  mundur yang akurat untuk seluruh durasi yang di-refresh.
