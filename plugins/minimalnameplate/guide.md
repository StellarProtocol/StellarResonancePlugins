# Minimal Nameplate

Replaces the game's overhead nameplates with a clean, role-coloured class badge and an
optional player name — drawn into the game's own HUD render pass, so it stays crisp and is
correctly hidden behind world geometry.

![Minimal nameplates](media/minimalnameplate.png)

## How to use

1. Install the plugin and launch the game **Modded**.
2. Open the **Nameplates** window from the Stellar overlay and switch the header chip **On**.
   The header shows **On · game nameplates hidden**; switch it **Off** to go back to the
   game's own nameplates (**Off · using the game's nameplates**).
3. Under **Show above players**, click a chip to switch each item **On** or **Off**:
   - **Class badge** — the role-coloured badge over each player.
   - **Player name** — the player name below the badge (party members in cyan).
   - **Friend icon** / **Guild icon** — a small heart for friends, a small shield for guildmates.
   - **Revive-sealed marker** — see *Revive-sealed marker* below.
4. Under **Size**, drag to adjust:
   - **Badge size** / **Name size** — scale both independently.
   - **Max players** — how many of the nearest players get a nameplate. Lower it if your FPS
     drops in crowded places.

Every setting has a **?** button next to it that explains what it does.

## Badge colours

- **Red**, **green** or **blue** — the player's role.
- **Orange** — a class that has no role colour (these used to show grey).
- **Grey** — the player is dead. Dead players also get a red name.

## Revive-sealed marker

After the **"Renewal Prayer"** role skill or the **"Arcane! Prayer of Life"** Imagine brings
someone back, they get **"Weakened: Wish Sealed"** and can't be revived that way again for
about 60 seconds (longer in Nightmare party dungeons).

If they die during that time, their badge turns purple and their name turns purple with the
seconds left — for example **Player (42s)** — so healers know not to waste the revive on them.
Turn it off with the **Revive-sealed marker** chip.

![Revive-sealed marker](media/revive-sealed.png)

## Behaviour

The plates mirror the game's own nameplate visibility rules — the global HUD switch
(HideUI, cutscenes, photo mode, menus), per-type head-info settings, and per-entity hides
all apply, so nothing shows where the game itself wouldn't show a plate.

To keep your own head clear, turn off the game's own head-info setting for your character —
the plates follow it.
