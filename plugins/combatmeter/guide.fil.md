# CombatMeter

Real-time na party combat meter para sa Star Resonance: live na **DPS**, **HPS** at **natanggap
na damage** para sa bawat isa sa party mo, may kulay ayon sa papel at sagisag ng class para
mabasa mo ang grupo nang isang tingin lang.

![Live na party meter](media/combat-meter.png)

## Pagsisimula

1. I-install ang plugin at i-launch ang laro nang **May mod**.
2. Lalabas ang meter bilang isang overlay window kapag nasa mundo ka na. I-drag ang title area
   nito para ilipat — natatandaan ang posisyon.
3. Pumasok sa labanan: lalabas ang mga row kada miyembro ng party at nag-a-update nang live.

## Pagbasa sa meter

- Bawat row ay isang miyembro ng party: damage kada segundo, heal kada segundo, at natanggap na
  damage.
- Ang kulay ng row ay sumusunod sa **papel** ng miyembro (tank / healer / DPS); ipinapakita ng
  sagisag ang kanyang class.
- Ipinapakita ng header ang timer ng encounter. May **team-countdown button** ang mga party
  leader sa header para simulan ang pull timer ng laro para sa buong party.

## Detalye ng skill

I-click ang row ng isang miyembro ng party para buksan ang **detalye ng skill** niya — bahagi ng
damage kada skill, crit rate, at uptime para sa kasalukuyang laban.

![Detalye ng skill](media/skill-breakdown.png)

## Mga archive — pag-save ng labanan

Ang isang *archive* ay nag-a-archive ng kasalukuyang labanan sa Kasaysayan at nire-reset ang live
na view.

![Kasaysayan](media/combatmeter-history.png)

- Ang **manual na archive** (ang archive button) ay **palaging na-save**, kahit ano pa mangyari
  — laging may nakikitang feedback na na-archive ito.
- Puwedeng mag-archive ang **awtomatikong archive** ng mga laban para sa iyo. Buksan ang
  **settings pane (gear icon)** para kontrolin ito: master on/off, mga toggle kada trigger
  (team wipe, yugto ng boss, idle sa laban, pagbabago ng dungeon stage), pinakamaliit na agwat
  sa pagitan ng mga archive, settle, palugit ng revive sa wipe, at huwag pansinin kapag solo.
- Ang **"panatilihin bago" ng yugto ng boss** ay nagpapahintulot ng ilang segundong takbo bago
  ang unang tama ng boss na isama sa segment ng boss (naka-off bilang default = puputol mismo sa
  unang tama).
- Nilalaktawan lang ang awtomatikong archive kapag talagang walang nangyari (lahat ng row ay
  zero). Walang binubura ang nilaktawang archive — lahat ay dala sa susunod na archive.

## Mga upload at run replay

Ang mga na-archive na run ay ina-upload sa [Stellar Logs](https://logs.stellarresonance.app),
kung saan makukuha mo ang buong run page: mga chart ng damage, detalye ng skill, yugto ng boss —
at isang **movement replay** ng buong run, mula sa sandaling pumasok ka sa dungeon hanggang sa
pagpatay.

- Ang **kopyahin ang link** ay naglalagay ng maikling URL ng run sa clipboard mo para ibahagi sa
  party mo.
- Pinapanatili ng **Kasaysayan** ang mga na-archive mong run kahit pagkatapos mag-relaunch. Ang
  muling pag-upload ng na-archive na run ay muling gagawa mismo ng orihinal na upload — buod,
  buong detalye ng labanan, at ang movement track — kahit nawala na ang data sa server.

## Mga tip

- Gumagana ang meter kahit hindi pa na-load ang data ng propesyon — hinuhulaan ang kulay ng
  papel at sagisag mula sa spec mo hanggang mapuno ang roster.
- Kung hindi ma-upload ang link ng run, subukang muli mula sa Kasaysayan: ang run na na-upload
  na ay babalik sa kasalukuyan niyang link sa halip na mabigo.
