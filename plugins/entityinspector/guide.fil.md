# Inspektor ng Entidad

Siyasatin ang kahit sinong player — stats, gear, skills, kasuotan at dye — sa iisang window na mabubuksan
mo para sa kahit sino, pati sa sarili mo.

![Ang tab na Pangkalahatan](media/entityinspector-overview.png)

## Paano ito buksan

1. I-install ang plugin at ilunsad ang laro nang **Modded**.
2. Magbukas ng pagsisiyasat mula sa alinman dito:
   - ang **Inspect** button sa profile card ng player, o
   - ang item na **Inspect** sa isang row ng CombatMeter (kapag naka-install ang CombatMeter).

## Ang header

Ipinapakita sa itaas ng window ang pangalan ng player, class at spec, level, Ability Score at
Illusion-Breaking. May status na nagsasabi kung gaano kabago ang impormasyon:

- **Malapit · live** — malapit sa iyo ang player, kaya live na nag-a-update ang lahat.
- **Malayo · profile mula N minuto ang nakalipas** — malayo sa iyo ang player; galing sa profile nila ang
  nakikita mo.
- **Session view** — tumitingin ka sa isang naka-save na session. Isara ito para bumalik sa live.

Sa kaliwa, ipinapakita ng **portrait** ang player sa 3D (**i-drag** para iikot, **mouse wheel** para mag-zoom),
kasama ang switch na **Armas: On / Off**, ang HP bar nila (may shield) at ang mga imagine nila.

## Ang mga tab

- **Pangkalahatan** — mga stat na nakagrupo sa mga panel sa dalawang column, kasama ang guild, party, suot
  na titulo at mga collection point sa fashion / mount / weapon skin.
- **Kagamitan** — ang armas sa sarili nitong row sa itaas, ang ibang piraso sa grid. **I-click ang item**
  para makita ang detalye. Ipinapakita ng sarili mong gear ang na-upgrade na gear score, refine at upgrade stage.
- **Mga Skill** — Battle Imagine at mga skill.
- **Aparador** — bawat piraso ng kasuotan kasama ang mga dye slot nito, may numero gaya sa dye screen ng
  laro (ang **Default** ay walang dye). **I-click ang kulay** para kopyahin ang color code nito.
- **Lokasyon** — kung nasaan ang player sa mapa. **I-drag** para mag-pan, **mouse wheel** para mag-zoom,
  at **Buong mapa** at **I-focus** para lumipat ng view.
- **Party** — ang party ng player. **I-click ang miyembro** para siyasatin siya.

![Ang tab na Kagamitan](media/entityinspector-gear.png)

![Ang tab na Aparador](media/entityinspector-wardrobe.png)

![Ang tab na Lokasyon](media/entityinspector-location.png)

Ang bilog na **?** sa kanang itaas ay nagpapakita ng maikling tulong.

## Ang hindi ibinabahagi ng laro

- Ilan lang sa stats ang ibinabahagi ng ibang player, kaya iyon lang ang ipinapakita, at nakatago ang mga
  seksyong walang laman.
- Lumalabas lang ang HP at mga imagine habang malapit sa iyo ang player. Lumalabas ang skills at gear
  kapag napalapit ka na sa kanila.
- Hindi ibinabahagi ng laro ang upgrade stage ng gear ng ibang player. May **+** pagkatapos ng gear score
  ang mga ma-upgrade na piraso (halimbawa **GS 260+**), ibig sabihin ay maaaring mas mataas pa.

## Mga tala

- Kailangan ng Stellar 2.32.0 o mas bago.
- Available ang plugin sa English, 日本語, ไทย, Bahasa Indonesia, Filipino at 한국어.
