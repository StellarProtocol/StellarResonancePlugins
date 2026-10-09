# Maestro

Isang MIDI auto-player para sa band (Musician) na instrumento ng Season 3 ng Star Resonance. Ilagay
ang mga `.mid` file mo sa isang folder, pumili ng kanta, at tutugtugin ito ng Maestro para sa iyo sa
na-summon mong instrumento — mag-isa o may grupo — may playlist, group sync, at local sound preview.

![Tumutugtog sa loob ng mundo habang bukas ang mga window ng Aklatan, Auto-Player at Preview](media/maestro-in-world.png)

## Pagsisimula

1. I-install ang plugin at buksan ang laro sa **Modded**.
2. Buksan ang **Maestro** mula sa Stellar launcher (grupong Plugins). Sa loob lang ng mundo
   magagamit ang mga tool ng band.
3. Ilagay ang mga `.mid` / `.midi` file mo sa folder ng laro na `midi\` — gagawin ito ni Maestro at
   ipapakita ang path nito sa window ng **Aklatan**. Gamitin ang **I-rescan ang folder** o
   **Buksan ang lokasyon** kung inilipat mo ito.
4. Mag-summon ng free-play na instrumento sa loob ng laro, magdagdag ng mga kanta mula sa **Aklatan**
   papunta sa pila, at pindutin ang ▶.

## Pagpapatugtog ng kanta

Ang pangunahing window ang iyong player: isang playlist, ang kasalukuyang pinapatugtog na pila, at
mga kontrol sa playback.

![Auto-Player — playlist, pila at mga kontrol sa playback](media/auto-player.png)

- Gumawa ng mga pinangalanang **playlist** at ayusin muli ang pila; ipinapakita ng header ang
  kasalukuyang kanta at posisyon.
- Ang **Awtomatikong sulong**, **Loop** (off / lahat / isa), **Shuffle**, at **puwang sa pagitan ng
  kanta** ay isang click lang ang layo.
- Ipinapakita ng status line ang live na posisyon at bilang ng nota para sa kantang pinapatugtog.

## Paghahanap ng kanta

![Aklatan — tingnan ang folder ng MIDI mo at magdagdag ng kanta sa pila](media/library.png)

- Tingnan ang buong folder ng MIDI mo, **maghanap** ayon sa pangalan, at i-click ang isang kanta
  para idagdag ito sa pila.
- Isang instrumento lang ang pinapatugtog sa isang pagkakataon, kaya i-pila na lang ang stem na
  gusto mong tugtugin.

## I-preview bago tumugtog

Pakinggan ang isang kanta gamit ang **tunay na tunog ng instrumento sa laro** nang hindi
nagsa-summon ng instrumento — ang preview ay tumutugtog para sa iyo lang, kaya hindi ito maririnig
ng mga nasa paligid mo.

![Local Preview — pakinggan gamit ang tunay na tunog ng instrumento sa laro](media/preview.png)

- Isang row bawat stem, may **mute** bawat bahagi at isang mode na **sustain**, kaya maririnig mo
  nang eksakto kung paano lalabas ang kanta.
- Ang **Instrument Sync** ay naghihintay sa live na band player at sumusunod dito, nagmu-mute sa
  mga bahaging tutugtugin mo mismo — kapaki-pakinabang para sa pag-jam kasama ang iba.

Dito sa Preview mahalaga ang pagpapangalan ng multi-stem: bigyan ang mga bahagi ng isang kanta ng
parehong base na pangalan, na nagtatapos sa instrumento sa loob ng panaklong, at ilo-load ng
Preview ang **buong set** kapag pinili mo ang alinman dito.

```
Song (Piano).mid   Song (Guitar).mid   Song (Bass).mid   Song (Bass 2).mid   Song (Drum).mid
```

Ang mga duplicate tulad ng `(Bass 2)` ay nagiging sarili nilang track sa parehong tunog ng
instrumento.

## Kontrol bawat kanta & pagtugtog nang grupo

Buksan ang **Mga Setting** para sa detalyadong pag-aayos at mga opsyon para sa grupo.

![Settings — kontrol bawat kanta, Network Sync at mga opsyon ng Ensemble](media/settings.png)

- **Bawat kanta**: Transpose, Hold ng nota, Tempo %, Max na nota (limitasyon sa polyphony), Puwang
  ng restrike, Vol ng monitor, Pilitin ang sustain, at Ilapat ang tone/teknik mula sa MIDI.
- Ang **Network Sync** ay nagpapadala ng mga nota mo nang maaga kaya mas matatag ang maririnig ng
  mga nakikinig sa paligid mo kahit siksikan ang mga nota.
- **Ensemble**: i-lock ang playback sa magkasanib na beat ng grupo mo (count-in papunta sa
  downbeat), opsyonal na itugma ang tempo ng ensemble, at awtomatikong tanggapin ang mga
  imbitasyon para sabay-sabay magsimula ang lahat. Sumali o magsimula muna ng ensemble sa loob ng
  laro.

## Pagtugtog sa isang ensemble

Ang ensemble mode ay nag-lo-lock sa pagtugtog ng buong party sa parehong beat, kaya maraming
manlalaro ang makakatugtog ng magkakaibang bahagi ng parehong kanta nang magkasabay, naka-sync.
Dapat nasa **parehong party** ang lahat.

1. **Buksan ang ensemble sync.** Sa **Mga Setting**, paganahin ang **I-sync sa ensemble** — sa
   bawat manlalaro. Opsyonal ring buksan ang **Awtomatikong tanggapin ang mga imbitasyon sa
   ensemble**, para hindi mo na kailangang manu-manong tanggapin ang bawat imbitasyon.
2. **Pipili ang bawat manlalaro ng kanyang bahagi at pipindutin ang ▶.** Piliin ang stem na
   tutugtugin mo at pindutin ang play — sa halip na magsimula agad, hihintayin ito ni Maestro at
   ipapakita ang **"naghihintay sa ensemble…"**.
3. **Sisimulan ng party leader ang ensemble sa loob ng laro.** Ito ang sariling pagsisimula ng
   ensemble ng laro.
4. **Magtutugtog ang lahat nang naka-sync.** Sabay na magsisimula ang lahat ng naghihintay na
   manlalaro sa downbeat, naka-lock sa beat ng ensemble.

Para sa buong band, magpa-pila ang bawat manlalaro ng **magkaibang** bahagi (Guitar, Bass, Drum,
Piano) ng parehong kanta, at opsyonal na buksan ang **Itugma ang tempo ng ensemble** para sumunod
ang playback sa BPM ng ensemble. Gamitin ang **Preview** muna para marinig kung paano magkakasya
ang buong set.

## Paghahanda ng mga MIDI file mo

Tinutugtog ni Maestro ang MIDI mo nang eksakto gaya ng nakasulat, kaya ang kaunting paghahanda ay
nagpapatunog nang tama sa mga kanta sa mga instrumento ng laro.

### Mga Epekto (tone at technique)

Buksan ang **Ilapat ang tone / teknik mula sa instrumentong MIDI** sa Mga Setting, at pipiliin ni
Maestro ang epekto ng guitar/bass mula sa **instrumento (program) na itinalaga sa track ng stem na
iyon**. Itakda ang General MIDI instrument ng track na iyon sa DAW mo:

**Mga stem ng guitar**

| Itakda ang GM instrument na ito | Program # | Tutugtog bilang |
|---|:--:|---|
| Nylon / Steel / Jazz / Clean Electric Guitar | 25–28 | Clean |
| Muted Guitar | 29 | Muffled (palm-mute) |
| Overdriven Guitar | 30 | Overdrive |
| Distortion Guitar | 31 | Distortion |
| Guitar Harmonics | 32 | Harmonics |

**Mga stem ng bass**

| Itakda ang GM instrument na ito | Program # | Tutugtog bilang |
|---|:--:|---|
| Acoustic / Finger / Pick / Fretless Bass | 33–36 | Clean |
| Slap Bass 1 / 2 | 37 / 38 | Slap |
| Synth Bass 1 / 2 | 39 / 40 | Overdrive |

- Ang mga numero ng program ay ang mga value na **1–128** na ipinapakita ng DAW mo; itugma sa
  **pangalan ng instrumento** kung hindi sigurado.
- Ang **pangunahing instrumento** lang ng stem ang binabasa, kaya panatilihin ang isang instrumento
  bawat stem. Ang isang program change sa gitna ng track ay nagpapalit ng epekto mula sa puntong
  iyon pasulong.
- Ang mga epekto ay nalalapat lamang sa **guitar at bass** — hindi pinapansin ito ng piano at drums.
- Itinutugma ang epekto sa instrumentong talagang na-summon mo. **Walang distortion ang bass** —
  tutugtog ito bilang Overdrive — at anumang teknik na hindi kaya ng na-summon na instrumento ay
  babalik sa normal.
- **Ang Overdrive / Distortion ay lokal lamang sa Network Sync** (tingnan ang *Kilalang Limitasyon*
  sa ibaba). Naririnig ng lahat ang Muffled, Harmonics at Slap.

### Drums

Ang drum kit ng laro ay isang nakatakdang **9-piece kit** sa mga key sa ibaba, at tinutugtog ni
Maestro ang mga nota mo nang eksakto gaya ng nakasulat — **hindi** nito awtomatikong kino-convert
ang General MIDI drums — kaya dapat gamitin ng isang drum stem ang mga key na ito:

| MIDI note | Piraso |
|:--:|---|
| 62 (D4) | Closed Hi-Hat |
| 65 (F4) | Kick |
| 69 (A4) | Floor Tom |
| 72 (C5) | Snare |
| 74 (D5) | Mid Tom |
| 76 (E5) | High Tom |
| 77 (F5) | Ride |
| 79 (G5) | Open Hi-Hat |
| 81 (A5) | Crash |

Tahimik ang anumang nota na wala sa mga key na ito. Nagsisimula mula sa isang standard na General
MIDI drum track? I-remap ang karaniwang mga GM note papunta sa mga key na ito — kick (GM 35/36) →
**F4**, snare (38/40) → **C5**, closed hat (42) → **D4**, open hat (46) → **G5**, ride (51) →
**F5**, crash (49) → **A5**, toms → **A4 / D5 / E5**.

### Ilan pang tip

- **Isang nota bawat pitch:** bawat instrumento ay tumutunog ng isang boses bawat pitch, kaya ang
  dalawang magkaparehong note na nagtatagpo ay binibilang na isa — iwasan ang nagtatambak na
  unison.
- **Pansinin ang range:** tahimik ang mga note na wala sa saklaw na kayang tugtugin ng
  instrumento. Gamitin ang **Transpose** (Mga Setting) para dalhin ang isang bahagi papasok sa
  saklaw.
- **Hindi naire-reproduce ang volume:** bawat nota ay tumutugtog sa parehong lakas, kaya hindi
  madadala ang velocity at dynamics.
- Suportado ang mga MIDI file na **type 0 o 1**.

## Kilalang Limitasyon

**Ang Overdrive / Distortion ay hindi umaabot sa ibang manlalaro — bug ito ng laro, hindi bagay na
kayang ayusin ni Maestro.** Ini-render lang ng laro ang *tone* ng overdrive/distortion ng
guitar/bass sa sarili mong client at hindi kailanman ipinapadala ito sa mga nasa paligid mo. Kaya
Clean ang naririnig ng lahat tuwing **naka-on ang Network Sync**, at kahit **naka-off** ang Network
Sync, *ikaw lang* ang nakaririnig ng distortion — hindi kailanman naririnig ng ibang manlalaro ang
overdrive/distortion mo sa alinmang mode. Walang setting ng Maestro na nagbabago nito; nasa laro
ang pag-aayos nito. Hindi naaapektuhan ang mga *technique* (Muffled, Harmonics, Slap) at naririnig
ito ng lahat. Para sa mga kantang kritikal ang distortion, tumugtog na naka-off ang Network Sync
para kahit papaano ay tama ang tunog para sa iyo.
