# Tagapamahala ng Raid

Mga tool para sa raid coordination na ipinapakita bilang HUD overlay — compatible sa mga ZDPS
raid-call convention na ginagamit na ng inyong statics.

Buksan ang **Tagapamahala ng Raid** mula sa launcher. May tatlo itong card, at bawat isa ay
nagpapakita agad ng kasalukuyang estado nito: **Mga Preset ng Mark**, **Mga Tawag ng Mekaniko** at
**Countdown at Babala sa Raid**.

![Raid Manager](media/raid-manager-settings.png)

May **?** sa tabi ng bawat setting na nagpapaliwanag kung ano ang ginagawa nito.

## Countdown at Babala sa Raid

1. I-install ang plugin at buksan ang laro sa mode na **May mod**.
2. I-type ang `/ct <seconds>` sa chat para magsimula ng malaking on-screen pull countdown
   (magiging pula sa huling 5 segundo). Kinakansela ito ng `/ct stop`.
3. I-type ang `/rw <message>` para mag-flash ng raid-warning callout sa lahat ng gumagamit ng
   plugin.

![Countdown](media/countdown.png)

Buksan ang **Countdown at Babala sa Raid** para i-set up ang mga ito:

- I-on o i-off ang **Command ng countdown** at **Babala sa raid**, at piliin kung saang mga chat
  sila makikinig (**Party**, **Guild**, **Lokal**).
- Piliin ang **Laki** ng countdown.
- **Estilo** — ipinapakita ng **Abiso ng laro** (default) ang `/rw` bilang sariling notice banner
  ng laro kasama ang tunog nito; ginagamit ng **Pulang overlay** ang malaking pulang teksto ng
  plugin, may sarili itong **Laki ng overlay**.
- Ang **Subukan ang countdown** at **Subukan ang babala sa raid** ay nagpe-preview ng dalawa nang
  walang ipinapadala sa chat.

Ilipat ang countdown kahit saan sa HUD layout editor — may sample na lalabas doon habang nag-e-edit ka.

## Mga Preset ng Mark

I-save ang mga dungeon marker na inilagay mo at i-reload ang buong layout sa isang click — bilang
mga multi-step na phase sequence. Buksan ito mula sa card na **Mga Preset ng Mark**:

![Mark Presets](media/mark-presets.png)

**Paggawa ng preset**

1. Mag-type ng pangalan at pindutin ang **Gumawa** (iwanang blangko para sa awtomatikong pangalan),
   pagkatapos ay **I-activate** ito.
2. Sa isang dungeon, ilagay ang mga marker para sa unang phase, pagkatapos ay pindutin ang
   **I-save ang mark bilang bagong hakbang**.
3. Ilagay ang mga marker ng susunod na phase at pindutin ulit ang **I-save ang mark bilang bagong
   hakbang** — ulitin para sa bawat phase.

**Paggamit habang nasa labanan**

- Ang **Susunod ›** / **‹ Nakaraan** ay lumilipat sa mga hakbang, nililinis ang board at inilalagay
  ang mga marker ng phase na iyon.
- Ang **Ibalik sa Simula** ay bumabalik sa blangkong **Simula**.
- I-bind ang **Mga Preset ng Mark: Nakaraang hakbang**, **Mga Preset ng Mark: I-reset sa Start**,
  at **Mga Preset ng Mark: Susunod na hakbang** sa sarili mong mga key sa **Mga Setting ng
  Stellar → Mga Hotkey** (hindi pa naka-bind bilang default).

Panatilihin ang hiwalay na preset para sa bawat raid at **I-activate** ang isinasagawa mo ngayon
(pindutin ang **Itigil** para i-off ito). Gumagana ang paglalagay ng marker sa mga dungeon.

**Pag-aayos ng mga preset**

- I-click ang pangalan ng preset para palitan ito, pagkatapos ay pindutin ang Enter.
- I-drag pataas o pababa ang isang preset para baguhin ang pagkakasunod ng listahan.
- Binubura ng **✕** sa isang row ang preset pagkatapos mong kumpirmahin.
- I-click ang **lapis** sa tabi ng kasalukuyang hakbang para magdagdag ng tala — hal. "P2 —
  magsama-sama sa kanluran". Lalabas din ang tala sa pop-up kapag lumipat ka sa hakbang na iyon.

**Pagbahagi ng preset**

1. I-activate ang preset at pindutin ang **I-export**, pagkatapos ay **Kopyahin**.
2. Ipadala ang code sa inyong grupo.
3. Pipindutin nila ang **I-import**, pagkatapos ay **I-paste** at **I-import**. Makukuha nila ang
   buong preset — bawat hakbang, marker, at tala.

## Mga Tawag ng Mekaniko

Makita agad kung sino ang tinatarget ng bawat mekaniko ng boss — bilang listahan, minimap, at malaking
alerto kapag ikaw ang target. Buksan ito mula sa card na **Mga Tawag ng Mekaniko**:

![Mechanic Callouts settings](media/mechanic-callouts-settings.png)

**Mga suportadong content**

- Forgotten Dreamwild raid — Clash!, Brutal! at Purge!
- Cursed Radiant Tomb
- Sea-Ringed Reef
- Towering Ruin
- Tina's Mindrealm

Sa labas ng mga ito, mananatiling walang laman ang mga tawag.

**Listahan ng Tawag** — bawat mekaniko kasama ang countdown nito at ang mga player na tinatarget nito.
Piliin ang pagkakasunod (**Ayon sa pagdating**, **Pinakamadalian muna** o **Ayon sa talahanayan**), ang
laki ng teksto at ang opacity ng background.

![Callout list](media/mechanic-callout-list.png)

**Minimap** — ang party mo sa arena, may kulay ayon sa mekaniko, kasama ang mga party marker at mga
mapanganib na lugar. Sa raid, ipinapakita rin nito ang floor damage, ang pagkakasunod ng pagpindot sa
mga crystal, ang mga may-numerong hakbang ng electromagnetic ring, ang mga linya ng sugod ng golem, kung saan
titigil ang umiikot na pamaypay at ang mga linya ng laser ng kanyon.

Opsyonal sa mga setting ng minimap: camera cone na nagpapakita kung saan ka nakatingin, ang sariling mapa ng laro sa ilalim, opacity ng background at pag-ikot ng mapa.

![Minimap](media/mechanic-minimap.png)

**Alerto kapag ikaw** — malaking banner kapag ikaw ang tinarget ng mekaniko, at **UMALIS** kapag
nakatayo ka sa mapanganib na tile. Sa mga ring phase ng raid, sinasabi nito kung saang ring ka dapat
lumipat. Opsyonal ang tunog at may sarili itong lakas; gamitin ang **Subukan ang alerto** para
i-preview. (Naka-off bilang default.)

![Alert banner](media/mechanic-alert.png)

Ilipat ang listahan, minimap at banner kahit saan sa HUD layout editor.

## Mga Paalala

- Parehong nananatiling nakikita ang mga overlay kahit bukas ang menu ng laro (ESC).
