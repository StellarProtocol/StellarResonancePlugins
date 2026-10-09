# Minimal Nameplate

Pinapalitan ang mga nameplate na nasa itaas ng ulo ng laro ng malinis, may kulay-ayon-sa-role na
class badge at opsyonal na pangalan ng player — iginuguhit papunta sa sariling HUD render pass ng
laro, kaya nananatili itong malinaw at tamang nakatago sa likod ng world geometry.

![Minimal nameplates](media/minimalnameplate.png)

## Paano gamitin

1. I-install ang plugin at buksan ang laro sa mode na **Modded**.
2. Buksan ang window na **Nameplates** mula sa Stellar overlay at i-on ang **Paganahin ang Minimal
   Nameplate (Huwag Paganahin ang Nameplate ng Laro)** (hindi na gagana ang sariling nameplate ng
   laro).
3. I-adjust ayon sa gusto mo:
   - **Ipakita ang Class Icon (badge)** — ang may kulay-ayon-sa-role na badge sa ibabaw ng bawat
     player.
   - **Ipakita ang Pangalan ng Player (sa ilalim ng badge)** — ang pangalan ng player sa ilalim ng
     badge.
   - **Itago ang sarili kong Badge + Pangalan** — panatilihing malinis ang ibabaw ng ulo mo.
   - **Laki ng Badge / Laki ng Pangalan** — i-scale ang dalawa nang hiwalay.

## Paggana

Sinusunod ng mga plate ang sariling mga panuntunan sa visibility ng nameplate ng laro — ang
global na HUD switch (HideUI, cutscenes, photo mode, menus), mga setting ng head-info kada type,
at mga per-entity hide ay lahat nag-aapply, kaya walang lalabas kung saan talaga hindi rin
magpapakita ang laro mismo ng plate.
