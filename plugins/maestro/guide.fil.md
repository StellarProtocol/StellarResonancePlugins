# Maestro

Isang MIDI auto-player para sa band (Musician) na instrumento ng Season 3 ng Star Resonance. Ilagay
ang mga `.mid` file mo sa isang folder, pumili ng kanta, at tutugtugin ito ng Maestro para sa iyo sa
na-summon mong instrumento — mag-isa o may grupo — may playlist, group sync, at local sound preview.

![Tumutugtog sa loob ng mundo habang bukas ang window ng Maestro](media/maestro-in-world.png)

## Pagsisimula

1. I-install ang plugin at buksan ang laro sa **May mod**.
2. Buksan ang **Maestro** mula sa Stellar launcher (grupong MGA PLUGIN). Sa loob lang ng mundo
   magagamit ang mga tool ng band.
3. Ilagay ang mga `.mid` / `.midi` file mo sa folder ng laro na `midi\` — kusa itong ginagawa ng
   Maestro. Sa **Library**, dadalhin ka agad doon ng **Buksan ang folder**, at kukunin naman ng
   **I-rescan** ang mga file na bago mong idinagdag.
4. Mag-summon ng free-play na instrumento sa loob ng laro, pindutin ang **+ Idagdag** para pumili ng
   kanta mula sa Library, at pindutin ang **I-play**.

## Maestro Link — tumugtog bilang band mula sa website

Sa Maestro Link, puwedeng tumugtog nang sabay ang buong band kahit walang ensemble sa loob ng laro.
Ang band leader ang nagpapatakbo ng room sa website na
[maestro.stellarresonance.app](https://maestro.stellarresonance.app), pumipili ng mga kanta, at
nagpapasya kung sino ang tutugtog ng aling part — tapos kusang ihahanda ng character ng bawat player
ang instrumento nito at tutugtugin ang part nito, sabay sa buong band.

![Tumutugtog ng kanta ng Maestro Link sa piano sa loob ng mundo](media/link-in-world.png)

### Pagsali sa isang room

1. Kunin ang **6-character na room code** — gumawa ng room sa website, o hingin ito sa band leader mo.
2. Sa loob ng laro, buksan ang **Maestro** → **Maestro Link**.
3. I-type ang code sa kahon at pindutin ang **Sumali**.
4. Ayan na — kapag nagpatugtog ng kanta ang room, awtomatikong tutugtugin ng character mo ang part nito.

Nakabuod ang mga hakbang na ito sa panel na **Paano ito gumagana**, at bubuksan ng **Buksan ang
website** ang site ng Maestro Link.

![Maestro Link — ilagay ang room code at pindutin ang Sumali](media/link-join.png)

### Habang nasa room

![Maestro Link habang nasa room — ang room, ang tumutugtog ngayon at ang listahan ng mga manlalaro](media/link-connected.png)

- Ipinapakita ng **Room** ang code at pangalan ng room, ang ping mo, at ang part na tinutugtog mo.
- Ipinapakita ng **Tumutugtog ngayon** ang kasalukuyang kanta, ang progreso nito, at ang **Susunod**.
- Nakalista sa **Mga manlalaro** ang lahat ng nasa room, kasama ang pangalan, instrumento, estado at
  ping. Ang estado ay **Tugtog**, **Handa**, **Hintay** o **Problema**. Lumalabas lang ang *Problema*
  kapag hindi matugtog ang part na tumutugtog mismo ngayon — ayos lang kung wala ka pang instrumento,
  dahil ihahanda ito ng Link para sa susunod na kanta. Tinitiklop ng **Itago** ang listahan.
- Ang **Itigil ang bahagi ko** (kulay amber) ay pinapahinga ang part mo habang tuloy ang tugtog ng
  band; pagkatapos, magiging **Ituloy ang bahagi ko** ang button.
- Ang **Idiskonekta** (pula) ay aalis sa room. Kapag nag-log out ka, awtomatiko kang madi-disconnect.

### Ang website

Sa room page ng website pinapatakbo ang band: nakalista ang lahat ng sumali, makikita ang
**Now playing** (pause, stop, skip, repeat), at nandoon ang pila ng mga kanta kung saan pipiliin
kung sino ang tutugtog ng bawat part. Puwedeng ibigay ang isang part sa ilang player, at puwedeng
kumuha ang isang player ng ilang part ng parehong instrumento. Ang **Room settings** ay para sa lahat
ng player nang sabay, at puwede mo pa ring i-adjust ang bawat player nang hiwalay.

![Ang website ng Maestro Link — mga player, Now playing at ang pila ng kanta](media/web-room.png)

## Pagpapatugtog ng kanta

Ang pangunahing window ang iyong player. Sa itaas, ipinapakita ng **Tumutugtog ngayon** ang
kasalukuyang kanta at ang progreso nito, kasama ang ◀◀ / **I-play** (**I-pause**) / ▶▶ / **Itigil**
at isang linyang **Susunod:** na nagsasabi kung ano ang kasunod.

![Ang pangunahing window — Tumutugtog ngayon, ang pila at ang apat na card](media/auto-player.png)

**Ang pila**

- Pumili ng playlist mula sa dropdown, o gamitin ang **+ Bago**, **I-rename** at **✕** (burahin ang
  playlist).
- **I-drag ang kanta** pataas o pababa para ayusin ang pagkakasunod, at pindutin ang **✕** sa isang
  row para alisin ito.
- Hindi titigil ang tumutugtog na kanta kapag lumipat ka sa ibang playlist — tuloy lang ito hanggang
  pumili ka ng kanta sa kabilang playlist at pindutin ang **I-play**.
- Ang mga icon sa ilalim ng pila ay para sa **Auto next**, **Loop** (off / lahat ng kanta / isang
  kanta) at **Shuffle**. Itapat ang mouse sa icon at lalabas ang pangalan at setting nito sa heading
  na **Pila**.
- Tinutugtog ng **Shuffle** ang bawat kanta sa playlist nang isang beses, sa random na pagkakasunod.
  Kapag naka-off ang Loop, hihinto ito pagkatapos ng huling kanta; kapag Loop lahat, binabalasa ulit
  ito sa bawat ikot. Laging ipinapakita ng linyang **Susunod:** ang talagang susunod na kanta.
- Binubuksan ng **+ Idagdag** ang Library.

Sa ibaba, binubuksan ng apat na card ang ibang mga window: **Library**, **Maestro Link**, **Preview**
at **Mga Setting**. Bawat card ay may maikling status, gaya ng bilang ng kanta mo o kung nasa room ka.

## Paghahanap ng kanta

![Library — hanapin ang kanta at i-click para idagdag sa playlist](media/library.png)

- **Maghanap** ayon sa pangalan (nililinis ng ✕ ang paghahanap), **I-rescan** para makuha ang mga
  bagong file, at **Buksan ang folder** para mapunta agad sa folder ng MIDI mo.
- **I-click ang isang kanta** para idagdag ito sa kasalukuyang playlist. Kikislap nang berde ang row
  na may **Naidagdag**, at kukumpirmahin ito ng linya sa ibaba, hal. *✓ Naidagdag ang “Bad Apple
  (Piano)” sa Playlist 1 · 11 kanta*.
- Isang instrumento lang ang pinapatugtog sa isang pagkakataon, kaya idagdag na lang ang stem na
  gusto mong tugtugin.

## I-preview bago tumugtog

Pakinggan ang isang kanta gamit ang **tunay na tunog ng instrumento sa laro** nang hindi
nagsa-summon ng instrumento — ang preview ay tumutugtog para sa iyo lang, kaya hindi ito maririnig
ng mga nasa paligid mo.

![Preview — ang kantang pine-preview, mga opsyon sa pag-sync sa banda at isang row bawat stem](media/preview.png)

- Ipinapakita ng **Pine-preview** ang napiling kanta sa pila at ang progreso nito, kasama ang
  **Preview** at **Itigil**.
- **I-sync sa banda**: hinihintay ng **Instrument Sync** na magsimula ang pagtugtog mo sa band, at
  saka sumasabay dito — i-mute ang mga part na ikaw mismo ang tutugtog. Inaantala ng **Sync offset**
  ang preview para tumapat sa naririnig ng band; kusa itong inaayos ng **= Band delay**.
- Sa **Mga stem**, may isang row bawat file, bawat isa ay may mute toggle, instrumento, mode ng
  **Sustain** at switch ng **Tone**, kaya maririnig mo nang eksakto kung paano lalabas ang kanta.

Dito sa Preview mahalaga ang pagpapangalan ng multi-stem: bigyan ang mga bahagi ng isang kanta ng
parehong base na pangalan, na nagtatapos sa instrumento sa loob ng panaklong, at ilo-load ng
Preview ang **buong set** kapag pinili mo ang alinman dito.

```
Song (Piano).mid   Song (Guitar).mid   Song (Bass).mid   Song (Bass 2).mid   Song (Drum).mid
```

Ang mga duplicate tulad ng `(Bass 2)` ay nagiging sarili nilang track sa parehong tunog ng
instrumento.

## Kontrol bawat kanta & pagtugtog nang grupo

Nasa iisang window na may dalawang column ang lahat ng detalyadong pag-aayos at opsyon para sa
grupo — buksan ito mula sa card na **Mga Setting**. Bawat setting ay may bilog na **?** button na
nagbubukas ng maikling paliwanag — i-click ulit, o ang ✕ nito, para isara. Bawat slider ay may
**↺** button din para ibalik ito sa default.

![Mga Setting — Pagtugtog, Ensemble, Tunog at Network sync, habang bukas ang tulong ng Volume ng monitor](media/settings.png)

- **Pagtugtog**: Transpose, Tempo, Hold ng nota, Max na nota (ilang nota ang puwedeng tumunog nang
  sabay), Puwang ng restrike (default 33 ms, para hindi nawawala ang mabilis na paulit-ulit na nota)
  at Pagitan ng kanta.
- **Ensemble**: ini-lock ng **I-sync sa ensemble** ang playback sa magkasanib na beat ng grupo mo
  (count-in papunta sa downbeat), sinusundan ng **Itugma ang tempo ng ensemble** ang BPM ng
  ensemble, at ang **Awtomatikong tanggapin ang mga imbitasyon sa ensemble** ang tatanggap para sa
  iyo. Sumali o magsimula muna ng ensemble sa loob ng laro.
- **Tunog**: Volume ng monitor, Pilitin ang sustain at Gamitin ang tono mula sa MIDI instrument.
- **Network sync**: ipinapadala ng **Network Sync Mode** ang mga nota mo nang maaga kaya mas matatag
  ang maririnig ng mga nakikinig sa paligid mo, kahit siksikan ang mga nota. Binubuksan din nito ang
  **I-mute ang lokal kong tunog** at **Ipakita ang key animation**. I-fine-tune ito gamit ang
  **Lookahead**, **Magpadala tuwing** at **Lead ng tono**. Ipinapaliwanag ang **Tone relay workaround
  (eksperimental)** sa *Kilalang Limitasyon* sa ibaba.

![Ang seksyong Network sync ng Mga Setting](media/network.png)

## Pagtugtog sa isang ensemble

Ang ensemble mode ay nag-lo-lock sa pagtugtog ng buong party sa parehong beat, kaya maraming
manlalaro ang makakatugtog ng magkakaibang bahagi ng parehong kanta nang magkasabay, naka-sync.
Dapat nasa **parehong party** ang lahat.

1. **Buksan ang ensemble sync.** Sa **Mga Setting**, paganahin ang **I-sync sa ensemble** — sa
   bawat manlalaro. Opsyonal ring buksan ang **Awtomatikong tanggapin ang mga imbitasyon sa
   ensemble**, para hindi mo na kailangang manu-manong tanggapin ang bawat imbitasyon.
2. **Pipili ang bawat manlalaro ng kanyang bahagi at pipindutin ang I-play.** Piliin ang stem na
   tutugtugin mo at pindutin ang I-play — sa halip na magsimula agad, hihintayin ito ng Maestro at
   ipapakita ang **"Hinihintay ang ensemble…"**.
3. **Sisimulan ng party leader ang ensemble sa loob ng laro.** Ito ang sariling pagsisimula ng
   ensemble ng laro.
4. **Magtutugtog ang lahat nang naka-sync.** Sabay na magsisimula ang lahat ng naghihintay na
   manlalaro sa downbeat, naka-lock sa beat ng ensemble.

Para sa buong band, magpa-pila ang bawat manlalaro ng **magkaibang** bahagi (Guitar, Bass, Drum,
Piano) ng parehong kanta, at opsyonal na buksan ang **Itugma ang tempo ng ensemble** para sumunod
ang playback sa BPM ng ensemble. Gamitin ang **Preview** muna para marinig kung paano magkakasya
ang buong set.

## Paghahanda ng mga MIDI file mo

Tinutugtog ng Maestro ang MIDI mo nang eksakto gaya ng nakasulat, kaya ang kaunting paghahanda ay
nagpapatunog nang tama sa mga kanta sa mga instrumento ng laro.

### Mga Epekto (tone at technique)

Buksan ang **Gamitin ang tono mula sa MIDI instrument** sa Mga Setting, at pipiliin ni
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
kayang ayusin ng Maestro.** Ini-render lang ng laro ang *tone* ng overdrive/distortion ng
guitar/bass sa sarili mong client at hindi kailanman ipinapadala ito sa mga nasa paligid mo. Kaya
Clean ang naririnig ng lahat tuwing **naka-on ang Network Sync**, at kahit **naka-off** ang Network
Sync, *ikaw lang* ang nakaririnig ng distortion — hindi kailanman naririnig ng ibang manlalaro ang
overdrive/distortion mo sa alinmang mode. Ipinapadala ng eksperimental na **Tone relay workaround** sa Mga
Setting ang tono mo sa ibang daan, pero ang mga nakikinig lang na gumagamit din ng Stellar ang
makaririnig nito; Clean pa rin para sa iba hanggang ayusin ito ng laro. Hindi naaapektuhan ang mga *technique* (Muffled, Harmonics, Slap) at naririnig
ito ng lahat. Para sa mga kantang kritikal ang distortion, tumugtog na **naka-off** ang Network Sync
para kahit papaano ay tama ang tunog para sa iyo.
