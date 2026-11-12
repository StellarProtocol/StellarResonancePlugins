# Target Lens

Know your target. Target Lens reads your current combat target and lays out everything that
matters — its health, shield and break gauge, the buffs and debuffs on it, who's holding aggro,
the boss's next deadly skill, its cast bar, and the monsters around you — as clean, draggable overlays.

![Target overlays in a boss fight](media/target-overlays.png)

## Getting started

1. Install the plugin and launch the game **Modded**.
2. Open the launcher overlay and click **Target Lens** to open its settings.
3. Target something in combat — the overlays appear automatically. Drag any overlay to move it and
   drag its edge to resize; positions are remembered.

## The overlays

- **Target HUD** — the target's **HP**, its **shield** ("armor"), and the boss **break / stagger
  gauge**, plus name, rank and distance.
- **Target Effects** — the buffs and debuffs on the target, each with its icon and remaining time.
  Your own effects are marked with a **★**.
- **Threat / Aggro** — who the boss is focused on and each player's **threat %**; your own row is
  highlighted.
- **Boss Skill Timers** — a live **countdown** to the boss's dangerous skills, so you can react in
  time. Appears during boss fights that use them.
- **Cast bar** — the target's channel/cast, filling as it casts; dangerous casts are accented.
- **Nearby Monsters** — a list of the enemies around you, bosses and elites first, then by distance.

## Nearby Monsters

![Nearby Monsters list with a locked target](media/nearby-monsters.png)

Each row shows the monster's name and HP inside a bar coloured by rank (**red** boss, **orange**
elite, **green** normal), with its distance on the right.

- **Click a row to lock on to that monster** — the same lock-on as the game's own lock key. Click
  the row again to release it.
- The row of your **current target** turns **blue**, and the monster you're **locked on to** shows a
  gold **padlock**.
- Turn on **Only in lockable range** to list only the monsters close enough to lock on to and stay
  locked. The distance depends on your class and is shown in metres next to the option.
- Drag the window's bottom edge to show more or fewer rows.

## Settings

![Target Lens settings](media/settings.png)

Each overlay has its own section with an **On / Off** switch in its header. Every option has a
**?** button that explains what it does. Options that don't apply right now — for example the list
size while effects are shown as Classic tiles — are greyed out instead of disappearing.

Under **Buffs / Debuffs → Display** you choose how effects are shown:

- **Classic** — compact tiles inside the Target HUD.
- **List** — a separate, resizable window with a row per effect (icon, name, and time).
- **Off** — hide the effect display entirely.

Under **Show**, pick which effects appear:

- **Applied by me** — your own effects (marked ★).
- **Applied by others** — effects from other players.
- **Monster's own / unknown** — the boss's self-buffs and effects whose source can't be told.
- **Hidden effects** — effects the game normally keeps out of sight.
- **Hide permanent effects** — leave out effects that never run out.

## Select effects

![Select effects window](media/select-effects.png)

Click **Select effects…** to choose exactly which buffs and debuffs appear.

- **Debuffs** and **Buffs** have their own tab, each with its own search box.
- Choose **Only the ticked ones** to show just the effects you tick, or **All except ticked** to
  hide the ones you tick. Each tab keeps its own choice.
- Recommended effects are marked with a **★**. Ticked effects are listed first, then recommended
  ones, then the rest.
- **Untick all** clears the current tab after you confirm.

## Tips

- **Lock the HUD to a target** — assign a key to the "Lock HUD to current target" hotkey in the
  launcher's hotkey settings. Press it in combat to pin the overlays to your current target so they
  keep showing that enemy while you fight others; press again to release. It clears on its own when
  the target dies. This only changes what the overlays show — to lock on in the game itself, click
  the monster's row in **Nearby Monsters**.
- **Size each overlay** — the effect list, threat/aggro list, cast bar, boss skill timers and nearby
  monster list each have their own size slider, and you can hide any overlay's title.
- Everything is optional — switch any overlay on or off from its section in the settings.
- Use the launcher's **"reset all HUD"** to return every overlay to its default position.
- Localized in **English, Japanese, Thai, Indonesian, Filipino and Korean** — it follows the
  launcher language.
