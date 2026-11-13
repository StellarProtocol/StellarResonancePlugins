# Dungeon Hub

Lahat ng dungeon sa iisang window. Pumili ng dungeon at hirap, saka pumasok nang Solo, mag-Match, o pumasok
kasama ang party mo, nang hindi na naglalakad papunta sa NPC. Pinapalitan din ng Dungeon Hub ang full-screen na
entry confirm ng laro ng maliit na window na nagpapakita kung sino ang handa na.

![Dungeon Hub](media/hub.png)

## Pagsisimula

1. I-install ang plugin at i-launch ang laro nang **May mod**.
2. Mag-log in sa isang character at buksan ang **Dungeon Hub** sa isa sa mga paraang ito:
   - ang button na **Dungeon Hub** sa kanang bahagi ng Esc menu ng laro,
   - ang tile na **Dungeon Hub** sa Stellar launcher,
   - sariling hotkey (tingnan ang *Mga Hotkey* sa ibaba).
3. Pumili ng tab, pumili ng dungeon sa kaliwa, saka piliin ang hirap at **Paano pumasok** sa kanan.

Ipinapakita ng header kung solo ka o nasa party (at kung ikaw ang leader). Bumubukas ang window sa tab na huli mong
ginamit.

## Pagpili ng dungeon

May limang tab ang hub: **Mga Dungeon**, **Raid**, **World Boss**, **Pangangaso** at **Iba pa**.

**Mga Dungeon**

- Ang **BUKAS NGAYON** sa itaas ay naglilista ng mga Unstable na dungeon na bukas ngayon, may markang **UNSTABLE**.
  Kasunod nito ang mga karaniwang dungeon sa ilalim ng **MGA DUNGEON**. Kapag walang bukas na Unstable na dungeon,
  walang heading ang listahan.
- Sa **HIRAP**, pumili ng **Normal**, **Mahirap** o **Master**. Kapag pinili ang **Master**, lalabas ang 20 level
  nito:
  - berde = natapos, karaniwan = ang susunod na level na tatapusin, malabo = naka-lock;
  - ang may kulay na guhit sa ilalim ay nagmamarka ng mga level na puwede ang **Solo** at ang **Match**.

**Raid**

- Lahat ng raid na bukas sa laro ngayon, may **Madali**, **Mahirap** at **Bangungot** kung mayroon ang raid.

**Ano ang sinasabi ng mga tile**

- **Inirerekomenda**: ang score na inirerekomenda ng laro para sa hirap na ito.
- **Iyong score**: berde kapag naabot mo ang rekomendasyon, dilaw kapag kulang pa.
- **Unang clear**: kung naghihintay pa ang premyo ng unang clear (**Handang kunin**) o **Nakuha na**.
- **Premyo**: ilang beses ka pang may makukuhang premyo. Kahit 0 na, puwede ka pa ring pumasok, wala ka lang
  makukuhang premyo.

## Paano pumasok

Tatlong button sa ibaba ng panel:

- **Solo**: pumasok nang mag-isa, kasama ang mga bot.
- **Match**: auto-match. Habang naghahanap, ipinapakita ng bar sa ibaba ang **Naghahanap**, ang tagal ng
  paghahanap at isang button na **Kanselahin**.
- **Party**: pumasok kasama ang party mo.

Kulay-abo ang button kapag hindi ito magagamit ngayon, at sinasabi ng linya sa ilalim kung bakit, halimbawa
"wala sa party" o "kailangan ng 3+ sa party (may 1 ka) - gamitin ang Match". Sa isang party, ang leader lang ang
puwedeng magsimula ng pagpasok o ng Match.

## World Boss, Pangangaso at Iba pa

- **World Boss**: ang boss ngayong linggo, natitirang premyo mo, kung bukas ang event at kailan ito susunod na
  bubukas. **Match** lang ang puwede sa World Boss.
- **Pangangaso**: ang lingguhang hunt at ang **Guild Hunt**. Puwedeng pasukin ang Guild Hunt mula rito (Normal o
  Mahirap, **Solo** o **Party**) habang bukas ito at nasa guild ka.
- **Iba pa**: nauuna ang **Bureau Commissions**. Tanggapin ang daily quest, tingnan ang mga event ngayon at kung ilan
  na ang natapos, at direktang **Pumasok** sa isang event. Dadalhin ka ng **Pumunta sa board** sa board ng Pioneer
  Bureau.
- May button na **Buksan** ang iba pang activity na magbubukas ng sariling page nito sa laro.

## Kumpirmasyon sa Dungeon

Kapag nagsimulang pumasok ang party o may nahanap na match, karaniwang tinatakpan ng laro ang buong screen para
pakumpirmahin ang lahat. Sa halip, maliit na window ang ipinapakita ng Dungeon Hub. Gumagana ito para sa 5-player na
dungeon, raid at kapag may nahanap na match.

![Kumpirmasyon sa Dungeon](media/confirm.png)

- Ang dungeon, ang hirap nito at isang countdown sa itaas.
- **MGA AFFIX**: ang mga affix ng dungeon, kung mayroon.
- **PARTY** (o **RAID**, hati sa apat na grupo): bawat player kasama ang class, score at estado nila:
  **Handa**, **Hintay…** o **Tumanggi**. May outline ang sarili mong row.
- **Kumpirmahin** at **Tanggihan** sa ibaba. Kapag may nahanap na match, magtatanong pa ulit ang Tanggihan
  (**Tanggihan?**) para hindi mo ito mapindot nang hindi sinasadya. Kung ikaw ang leader na nagsimula ng pagpasok,
  **Kanselahin** ang lalabas sa halip.
- Kapag handa na ang lahat, magsasara ang window at papasok ka. Kapag may tumanggi o naubos ang oras, sasabihin ng
  window kung sino, saka ito kusang magsasara.
- Laging nasa ibabaw ng iba mong Stellar window ang window na ito, kaya hindi ito natatakpan ng mga iyon.

## Settings

I-click ang gear button (**⚙**) sa header ng Dungeon Hub para buksan ang **Settings ng Dungeon Hub**.

![Settings ng Dungeon Hub](media/settings.png)

**SCREEN NG KUMPIRMASYON**

- **Palitan ang screen ng kumpirmasyon ng laro**: i-on o i-off ang maliit na window. Naka-on ito bilang default.
  Kapag naka-off, ang full-screen na kumpirmasyon ng laro ang lalabas.
- **Preview 5 manlalaro** / **Preview raid**: tingnan ang window na may mga halimbawang player, kahit walang party.
  Isinasara lang ng mga button ang preview.

**MGA TUNOG** (sa maliit na confirm window lang tumutunog)

- **Tunog kapag bumukas**: tunog kapag lumabas ang confirm window.
- **Tik ng countdown**: isang tik bawat segundo sa huling 10 segundo habang hindi ka pa sumasagot.
- **Lakas**: para sa dalawang tunog. Pindutin ang **▶** sa tabi ng tunog para marinig ito, at **■** para ihinto ito.
- **Buksan ang folder**: ilagay doon ang sarili mong .wav o .mp3, saka piliin sa listahan ng tunog. Ibinabalik ng
  **Default** ang kasamang tunog. Kapag hindi ma-play ang isang file, ang default na tunog ang tutunog at sasabihin
  ng settings kung aling file iyon.
- Maririnig mo ang mga tunog na ito kahit naka-mute ang laro o nasa background ito.

**WINDOW**

- **Sariling opacity ng window**: naka-off = sinusunod ng mga window ng Dungeon Hub ang Window Opacity ng theme mo.
  Naka-on = itakda ang sarili mong **Opacity** para sa lahat ng ito.

## Mga Hotkey

Parehong walang naka-bind na key sa simula. Mag-assign ng key sa **Settings ng Stellar → Mga Hotkey**:

- **Buksan/isara ang Dungeon Hub**: buksan o isara ang hub.
- **Dungeon: I-toggle ang kapalit ng kumpirmasyon**: i-on o i-off ang maliit na confirm window.

## Mga Paalala

- Bumubukas lang ang Dungeon Hub habang nasa mundo ka ng laro, at nagsasara ang mga window nito kapag nag-log out ka.
- Kapag hindi maipakita ng maliit na window ang isang entry request, lalabas gaya ng dati ang confirm screen ng laro.
