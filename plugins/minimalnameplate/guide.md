# Minimal Nameplate

Replaces the game's overhead nameplates with a clean, role-coloured class badge and an
optional player name — drawn into the game's own HUD render pass, so it stays crisp and is
correctly hidden behind world geometry.

![Minimal nameplates](media/minimalnameplate.png)

## How to use

1. Install the plugin and launch the game **Modded**.
2. Open the **Nameplates** window from the Stellar overlay and turn on
   **Enable Minimal Nameplate** (this disables the game's own nameplate).
3. Tune it to taste:
   - **Show Class Icon (badge)** — the role-coloured badge over each player.
   - **Show Player Name (under badge)** — the player name below the badge.
   - **Hide My Own Badge + Name** — keep your own head clear.
   - **Badge Size / Name Size** — scale both independently.

## Behaviour

The plates mirror the game's own nameplate visibility rules — the global HUD switch
(HideUI, cutscenes, photo mode, menus), per-type head-info settings, and per-entity hides
all apply, so nothing shows where the game itself wouldn't show a plate.
