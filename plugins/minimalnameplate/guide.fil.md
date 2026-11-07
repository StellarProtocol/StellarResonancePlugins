# Minimal Nameplate

Pinapalitan ang mga nameplate na nasa itaas ng ulo ng laro ng malinis, may kulay-ayon-sa-role na
class badge at opsyonal na pangalan ng player — iginuguhit papunta sa sariling HUD render pass ng
laro, kaya nananatili itong malinaw at tamang nakatago sa likod ng world geometry.

![Minimal nameplates](media/minimalnameplate.png)

## Paano gamitin

1. I-install ang plugin at buksan ang laro sa mode na **May mod**.
2. Buksan ang window na **Nameplates** mula sa Stellar overlay at gawing **On** ang chip sa header.
   Ipapakita ng header ang **On · nakatago ang nameplate ng laro**; gawin itong **Off** para bumalik
   sa sariling nameplate ng laro (**Off · gamit ang nameplate ng laro**).
3. Sa **Ipakita sa itaas ng player**, i-click ang chip para gawing **On** o **Off** ang bawat isa:
   - **Class badge** — ang may kulay-ayon-sa-role na badge sa ibabaw ng bawat player.
   - **Pangalan ng player** — ang pangalan ng player sa ilalim ng badge (cyan ang mga kasama sa party).
   - **Friend icon** / **Guild icon** — maliit na puso para sa mga kaibigan, maliit na kalasag para
     sa mga kasama sa guild.
   - **Marker ng selyadong pagbuhay** — tingnan ang *Marker ng selyadong pagbuhay* sa ibaba.
4. Sa **Laki**, i-drag para i-adjust:
   - **Laki ng badge** / **Laki ng pangalan** — i-scale ang dalawa nang hiwalay.
   - **Max na player** — ilan sa mga pinakamalapit na player ang may nameplate. Babaan ito kung
     bumababa ang FPS sa mataong lugar.

May **?** na button sa tabi ng bawat setting na nagpapaliwanag kung ano ang ginagawa nito.

## Mga kulay ng badge

- **Pula**, **berde** o **asul** — ang role ng player.
- **Orange** — class na walang kulay ng role (dati ay kulay abo ang mga ito).
- **Kulay abo** — patay ang player. Pula rin ang pangalan ng mga patay na player.

## Marker ng selyadong pagbuhay

Kapag may nabuhay muli dahil sa role skill na **"Renewal Prayer"** o sa Imagine na
**"Arcane! Prayer of Life"**, magkakaroon siya ng **"Weakened: Wish Sealed"** at hindi na siya mabubuhay muli
sa ganoong paraan nang mga 60 segundo (mas matagal sa mga Nightmare party dungeon).

Kung mamatay siya sa panahong iyon, magiging lila ang badge niya at magiging lila ang pangalan niya
kasama ang natitirang segundo — halimbawa **Player (42s)** — para alam ng mga healer na huwag sayangin
ang revive sa kanya. I-off ito gamit ang chip na **Marker ng selyadong pagbuhay**.

![Marker ng selyadong pagbuhay](media/revive-sealed.png)

## Paggana

Sinusunod ng mga plate ang sariling mga panuntunan sa visibility ng nameplate ng laro — ang
global na HUD switch (HideUI, cutscenes, photo mode, menus), mga setting ng head-info kada type,
at mga per-entity hide ay lahat nag-aapply, kaya walang lalabas kung saan talaga hindi rin
magpapakita ang laro mismo ng plate.

Para panatilihing malinis ang ibabaw ng ulo mo, i-off ang sariling setting ng head-info ng laro
para sa character mo — sinusunod ng mga plate ang setting na iyon.
