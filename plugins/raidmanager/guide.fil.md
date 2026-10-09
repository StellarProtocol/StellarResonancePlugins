# Tagapamahala ng Raid

Mga tool para sa raid coordination na ipinapakita bilang HUD overlay — compatible sa mga ZDPS
raid-call convention na ginagamit na ng inyong statics.

## Countdown & Raid Warning

1. I-install ang plugin at buksan ang laro sa mode na **Modded**.
2. I-type ang `/ct <seconds>` sa chat para magsimula ng malaking on-screen pull countdown
   (magiging pula sa huling 5 segundo).
3. I-type ang `/rw <message>` para mag-flash ng raid-warning callout sa lahat ng gumagamit ng
   plugin.

![Countdown](images/countdown.png)

- **Gumamit ng Babala sa Loob ng Laro** (naka-on bilang default) — ipinapakita ng `/rw` ang native
  na banner ng laro kasama ang audio ng tagumpay sa halip na ang custom overlay ng plugin, kaya ang
  callout ay mukha at tunog tulad ng sarili ng larong babala.

## Mga Preset ng Mark

I-save ang mga dungeon marker na inilagay mo at i-reload ang buong layout sa isang click — bilang
mga multi-step na phase sequence. Buksan ito mula sa **Tagapamahala ng Raid → Buksan ang Mga
Preset ng Mark**:

![Raid Manager settings](images/raid-manager-settings.png)

**Paggawa ng preset**

1. **Gumawa** ng preset, bigyan ito ng pangalan, at **I-activate** ito.
2. Sa isang dungeon, ilagay ang mga marker para sa unang phase, pagkatapos ay **+ I-save ang
   Hakbang**.
3. Ilagay ang mga marker ng susunod na phase at **+ I-save ang Hakbang** ulit — ulitin para sa
   bawat phase.

**Paggamit habang nasa labanan**

- Ang **Susunod ▶** / **◀ Nakaraan** ay lumilipat sa mga hakbang, nililinis ang board at inilalagay
  ang mga marker ng phase na iyon.
- Ang **I-reset** ay bumabalik sa blangkong **Start**.
- I-bind ang **Nakaraan / I-reset / Susunod** sa sarili mong mga key sa key settings ng laro
  (hindi pa naka-bind bilang default).

![Mark Presets](images/mark-presets.png)

Panatilihin ang hiwalay na preset para sa bawat raid at **i-activate** ang isinasagawa mo ngayon
(magiging **I-deactivate** ang button). Gumagana ang paglalagay ng marker sa mga dungeon.

**Pagpapalit ng pangalan at pagdagdag ng tala**

- I-click ang **lapis** sa tabi ng isang preset para palitan ang pangalan nito.
- Sa ilalim ng kasalukuyang hakbang, i-click ang **lapis** para magdagdag ng tala — hal. "P2 —
  magsama-sama sa kanluran". Lalabas din ang tala sa pop-up kapag lumipat ka sa hakbang na iyon.

**Pagbahagi ng preset**

1. I-activate ang preset at i-click ang **I-export ang Preset**, pagkatapos ay **Kopyahin** — o
   piliin ang code at pindutin ang Ctrl+C.
2. Ipadala ang code sa inyong grupo.
3. I-click nila ang **I-import ang Preset**, i-paste ang code, at pindutin ang **I-import**.
   Makukuha nila ang buong preset — bawat hakbang, marker, at tala.

## Mga Paalala

- Parehong nananatiling nakikita ang mga overlay kahit bukas ang menu ng laro (ESC).
