# Photo Studio

Kumuha ng magagandang screenshot: itago ang interface, igalaw ang free camera kahit saan sa
paligid ng eksena, i-freeze ang sandali, pumose ang kahit sino, kumuha ng portrait at square na
litrato, magdagdag ng mga look at ilaw — pagkatapos i-save bilang litratong mataas ang
resolusyon.

![Ang panel ng Photo Studio](media/photostudio-inworld.png)

## Mabilisang simula

1. I-install ang plugin at i-launch ang laro nang **Modded**.
2. Pindutin ang **Shift+F10** para buksan ang panel ng Photo Studio.
3. Pindutin ang **F10** para kumuha ng litrato. Nase-save ang mga litrato sa folder ng
   screenshot mo (tab na Kuha → Folder).
4. Itinatago ng **Ctrl+F10** ang lahat para sa malinis na kuha.

Puwedeng baguhin ang lahat ng hotkey sa **Stellar Mga Setting → Mga Hotkey**.

## Ang mga tab

- **Kuha** — resolution (1×, 2× o 4× ng screen mo), ang **hugis** ng litrato (Screen, portrait
  9:16, 4:5, 2:3, square 1:1, wide 21:9), PNG o JPG, ang folder, at kung ano ang itatago habang
  kumukuha.
- **Itsura** — kulay, exposure, contrast, white balance, depth of field, at iba pa. I-save ang
  mga paborito mong itsura bilang preset.
- **Kamera** — ang free camera, ang eksena (freeze / reset), mga setting ng galaw, at pagpo-pose
  ng tao.
- **Ilaw** — mga lampara, at key light saka rim para sa isang tao.
- **Mga preset** — i-load, i-save, palitan ang pangalan, at ibahagi ang mga itsura mo.

## Pagtatago ng mga bagay

Sa grupong **Itago** ng tab na **Kuha**, makikita mo ang parehong listahan tulad ng sariling
photo screen ng laro, sa parehong ayos:

- Itinatago ng **Ako** at **Sarili kong Spirit Echo** ang karakter mo at ang Spirit Echo mo —
  sa sarili mong screen lang. Puwede ka pa ring gumalaw at lumaban, at nakikita ka pa rin ng
  ibang manlalaro.
- Itinatago ng **Ibang adventurer**, **Hindi manlalaro**, **Kalaban**, **Collectible** at
  **Ibang Spirit Echo** ang mga iyon sa screen mo.
- Tunay na itinatago ng **Mga kaibigan**, **Party** at **Guild** ang mga manlalarong iyon,
  kahit nakikita pa rin ang Ibang adventurer. Laging nananalo ang grupong nakatago: isang
  kaguild na kaibigan mo rin ay itinatago kapag itinago mo ang guild mo. Nakikita pa rin ang
  mga miyembro ng party habang ipinapakita mo ang Party.
- Itinatago ng **Armas** ang armas ng lahat ng manlalaro, hindi lang sa iyo.
- Itinatago ng **Mga effect** ang effect ng skill, buff at tama ayon sa kung sino ang may dulot:
  **Akin** (sa iyo, sa pet mo, at sa Battle Imagine mo), **Party**, **Ibang manlalaro** at
  **Mga halimaw** (kasama ang mga babalang lugar ng boss). Hindi kailanman itinatago ang mga
  eksenang bagay tulad ng talon at lampara. Lagi pang nakikita ang ilang effect na hindi
  iniuugnay ng laro kaninuman.

Ikinakapit ang mga ito habang bukas ang panel at sa bawat litrato. Sa tab na **Kamera**,
ginagawa rin ng **Itago ako pagpasok** at **Itago ang mga effect pagpasok** ang parehong bagay
sa sandaling mag-on ang free camera (sinusunod ng mga effect ang mga switch sa tab na Kuha).
Ang pag-off — o pagsara ng Photo Studio — ay nagbabalik sa lahat. Dala-dala ang mga setting ng
pagtatago mo mula sa 1.6.0. Itinatago pa rin ng **Ctrl+F10** ang lahat, kasama ang Stellar
overlay.

## ReShade

Puwedeng gumamit ang Photo Studio ng mga epekto ng **ReShade** — sa screen mo at sa mga
litrato mo.

1. Sa Stellar launcher, buksan ang page ng Photo Studio at panatilihing naka-check ang
   **ReShade** sa ilalim ng **Dependencies** (sa Linux / Proton, panatilihin ding naka-check
   ang **Microsoft shader compiler**). Dina-download at sinusuri ito ng launcher bago magsimula
   ang laro.
2. I-launch ang laro nang **Modded**. Ginagamit lang ang ReShade sa Modded na launch; walang
   ReShade ang **Vanilla** na launch.
3. Buksan ang tab na **Itsura** → grupong **ReShade**: i-on ang **Gamitin ang ReShade**, pumili
   ng **Preset** (o **Wala** para walang effect), at i-on o i-off ang mga effect.
4. Sa ilalim ng **Mga preset**, magdagdag ng mga handa nang itsura sa isang click, at sa
   ilalim ng **Mga shader pack**, i-download ang mga effect pack na ginagamit nila.
   Agad lumalabas ang mga na-download na preset sa listahan ng Preset.

I-fine-tune ang bawat effect sa sariling menu ng ReShade (**Home** bilang default). Hindi
isinasama ng portrait, square at wide na litrato ang mga effect na nangangailangan ng depth;
sinasabi sa iyo ng Photo Studio kapag nangyari ito.

## Free camera

Pindutin ang **Alt+F10** (o ang button na **Free camera**) para kontrolin ang camera.

- **Ikot** (default): ibinabaling ng **right mouse** sa paligid ng tao, lumalapit o lumalayo ang
  **mouse wheel**, bumababa / umaakyat ang **Q / E**. **I-click** ang ibang karakter para umikot
  dito; bumabalik sa iyo ang **Backspace**.
- **Lipad**: pindutin ang **Tab**, pagkatapos **WASD** para gumalaw, **right mouse** para
  tumingin.
- Iniikot ng **Z / C** ang camera, nag-zoom ang **Shift + wheel** (field of view), bumabalik sa
  view ng laro ang **R**.
- Nag-zoom in ang **]** at zoom out ang **[** (i-hold para magpatuloy), nire-reset ng **\\** ang
  field of view. May slider din ang tab na **Itsura** para sa **Field of view** (10–100°).
  Nakikita ang mga key sa screen.
- Nagfi-freeze ng eksena ang **Space**, itinatago ng **H** ang mga key hint, lumalabas sa free
  camera ang **Esc** o **Alt+F10**.

## Pag-freeze ng eksena

Pinapahinto ng **Space** sa free camera (o **Freeze** sa tab na Camera) ang buong laro sa
screen mo — lahat ng tao, bawat skill at effect, at ang sarili mong karakter — tulad ng
pag-pause sa video. Nanatiling frozen ang mundo kahit umalis ka sa free camera; pindutin ang
**Resume** o **Space** ulit para magpatuloy. Patuloy na tumatakbo ang laro sa server: tumutuloy
ang labanan, at anumang nangyari ay makikita mo pagkatapos mong magpatuloy. Nananatili sa
screen ang monster na napatay habang frozen hanggang sa magpatuloy ka.

## Pagpo-pose ng tao

Sa grupong **Tao** ng tab na Camera, pumili ng tao gamit ang **‹ ›** (o i-click sila sa free
camera):

- **Pose** — pumili ng emote at i-drag ang **Moment** papunta sa eksaktong frame; nagpi-play o
  nagpa-pause ang ▶ / ❚❚.
- **Ekspresyon** — isang ekspresyon ng mukha na mananatili hanggang baguhin mo ito.
- **Ulo / Mata** — tumingin sa camera (**Lente**), malaya (**Malaya**), o normal, at i-lock ito.
  Sa **Malaya**, i-drag ang tuldok sa grid para mag-aim, o itulak gamit ang mga arrow;
  tinutukoy ng **Hakbang** (Pino · Normal · Magaspang) kung gaano kalayo ang kada pindot ng
  arrow.
- **Iikot** — ibaling sila sa direksyong gusto mo.

Pinopose ang ibang manlalaro at NPC bilang kopya na ikaw lang ang nakakakita; nakatago ang
tunay na tao hanggang i-reset mo ang eksena.

## Ilaw

Sa tab na **Ilaw**:

- Naglalagay ng lampara sa posisyon ng camera ang **＋ Lampara sa camera** (o **L** sa free
  camera); inililipat doon ng **Shift+L** ang napiling lampara. Hanggang 8 lampara, bawat isa
  may sariling kulay, lakas at saklaw, nakalagay sa paligid ng napiling tao.
- Tinutukoy ng **Ilaw sa tao** kung gaano kalakas tinitina ng lampara ang mga karakter (tinitina
  rin ang ibang tao malapit sa kahit anong lampara).
- Nagbibigay ang **Ilaw sa tao** sa isang tao ng **key light** mula sa piniling gilid, at kulay
  na **rim** sa buhok, suot sa ulo, at armas.

## Pagtatapos ng eksena

Inaalis ng **I-reset ang eksena** (tab na Camera) ang freeze, tinatanggal ang mga na-pose na
kopya at ilaw, at ibinabalik ang lahat sa normal. Ginagawa rin ito ng pagpalit ng zone, isang
cutscene, o pag-off sa Photo Studio.

## Pagliit at pagsasara

- Ginagawang maliit na strip ng **–** sa panel ang panel, walang binabago.
- Isinasara nang lubusan ng **✕** sa panel o strip, o ng **Ctrl+Shift+F10**, ang Photo Studio:
  lumalabas sa free camera, inaalis ang freeze, nire-reset ang mga na-pose na tao at lampara,
  at ipinapakita ang lahat ng itinago mo. Kung may kahit isa sa mga iyon na tumatakbo pa,
  magtatanong muna ito at ililista kung ano ang matatapos.
- Habang sarado ang Photo Studio, ipinapakita ng mga pill ng eksena at camera ang key na muling
  magbubukas nito.
