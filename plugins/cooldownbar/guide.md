# CooldownBar

Your skill cooldowns, debuffs and buffs at a glance, so you can see what's coming back without
staring at the hotbar. Show them together on one bar, or give each effect its own tile anywhere
on screen.

![Cooldown bar](media/cooldownbar-bar.png)

## How to use

1. Install the plugin and launch the game **Modded**.
2. The bar appears as an overlay. Drag it wherever you want — the position is remembered.
3. Every active effect is a tile: cyan for skill cooldowns, red for debuffs, green for buffs.
   The fill bar and the number show the time left; the small chip shows stacks or charges.
4. Press **F8**, click the gear on the bar, or open CooldownBar from the Stellar menu to open
   **Settings**. It opens centred on the bar.

![Settings](media/cooldownbar-settings.png)

## Settings

- **Skills / Debuffs / Buffs** — switch between the three lists. Each chip shows how many you
  have ticked.
- **Only ticked / All except ticked** — show just the ones you tick, or everything except them.
  Each list has its own choice.
- **Search** the list; the clear button empties the box. When you tick something, the row
  flashes and the note at the bottom confirms it.
- **Untick all** asks you to confirm before it clears the list.
- **Show as** — **Bar** (one panel with every active effect) or **Placed** (one tile per effect,
  see below).
- **Background** (Bar) sets how solid the bar's panel is. Its colour follows your theme.
- **Hide when empty** (Bar, off by default) hides the bar while nothing is active. It comes back
  with the next cooldown or effect, and always shows in the HUD layout editor. While it's hidden,
  open Settings with **F8** or from the Stellar menu.
- Press **?** next to an option for a short explanation.

## Placed mode

1. In Settings, set **Show as** to **Placed**.
2. Tick the effects you want, up to 30. Each one becomes its own tile. Placed mode always shows
   only the ticked effects.
3. Open the HUD layout editor. All tiles show there, so drag each one where you want it.
4. Outside the editor, a tile shows only while its effect is active.
5. **Size** sets every tile to 75 %, 100 %, 125 %, 150 % or 200 %. To give one tile its own size,
   click the size chip on its row in the list.

![Placed tiles](media/cooldownbar-placed.png)

## Tips

- The time left reads in seconds up to 99 s (with tenths under 10 s), then in minutes (e.g. "8m"),
  then in hours. Stacks show up to 99, then "99+".
- Buffs that refresh their timer on a new stack stay visible with an accurate countdown for
  the full refreshed duration.
