# Raid Manager

Raid coordination tools shown as a HUD overlay — compatible with the ZDPS raid-call
conventions your statics already use.

Open **Raid Manager** from the launcher. It has three cards, and each one shows its current state at a glance:
**Mark Presets**, **Mechanic Callouts** and **Countdown & Raid Warning**.

![Raid Manager](media/raid-manager-settings.png)

Every setting has a **?** next to it that explains what it does.

## Countdown & Raid Warning

1. Install the plugin and launch the game **Modded**.
2. Type `/ct <seconds>` in chat to start a big on-screen pull countdown (turns red in the last 5 seconds).
   `/ct stop` cancels it.
3. Type `/rw <message>` to flash a raid-warning callout to everyone running the plugin.

![Countdown](media/countdown.png)

Open **Countdown & Raid Warning** to set them up:

- Turn **Countdown command** and **Raid warning** on or off, and pick which chats they listen in
  (**Party**, **Guild**, **Local**).
- Choose the countdown **Size**.
- **Style** — **Game notice** (default) shows `/rw` as the game's own notice banner with its sound;
  **Red overlay** uses the plugin's big red text, with its own **Overlay size**.
- **Test countdown** and **Test raid warning** preview both without sending anything to chat.

Move the countdown anywhere in the HUD layout editor — a sample shows there while you edit.

## Mark Presets

Save the dungeon markers you place and reload the whole layout in one click — as multi-step
phase sequences. Open it from the **Mark Presets** card:

![Mark Presets](media/mark-presets.png)

**Build a preset**

1. Type a name and press **Create** (leave it blank for an automatic name), then **Activate** it.
2. In a dungeon, place your markers for the first phase, then press **Save marks as new step**.
3. Place the next phase's markers and press **Save marks as new step** again — repeat for each phase.

**Use it during the fight**

- **Next ›** / **‹ Prev** move through the steps, clearing the board and placing that phase's markers.
- **Reset to Start** returns to the blank **Start**.
- Bind **Mark Presets: Previous step**, **Mark Presets: Reset to Start** and **Mark Presets: Next step**
  to your own keys in **Stellar Settings → Hotkeys** (they start unbound).

Keep a separate preset for each raid and **Activate** the one you're running (press **Stop** to turn it
off). Marker placement works in dungeons.

**Organise your presets**

- Click a preset's name to rename it, then press Enter.
- Drag a preset up or down to reorder the list.
- The **✕** on a row deletes the preset after you confirm.
- Click the **pencil** next to the current step to add a note — e.g. "P2 — stack west". The note also
  shows in the pop-up when you move to that step.

**Share a preset**

1. Activate the preset and press **Export**, then **Copy**.
2. Send the code to your group.
3. They press **Import**, then **Paste** and **Import**. They get the whole preset —
   every step, marker and note.

## Mechanic Callouts

See at a glance who is targeted by which boss mechanic — as a list, a minimap and a big alert when
it's you. Open it from the **Mechanic Callouts** card:

![Mechanic Callouts settings](media/mechanic-callouts-settings.png)

**Supported content**

- Forgotten Dreamwild raid — Clash!, Brutal! and Purge!
- Cursed Radiant Tomb
- Sea-Ringed Reef
- Towering Ruin
- Tina's Mindrealm

Outside these, the callouts stay empty.

**Callout List** — each mechanic with its countdown and the players it targets. Choose the order
(**Arrival order**, **Most urgent first** or **Table order**), the text size and the background opacity.

![Callout list](media/mechanic-callout-list.png)

**Minimap** — your party on the arena, coloured by mechanic, with party markers and danger areas.
In the raid it also shows floor damage, the crystal press order, the numbered electromagnetic ring steps, the golem dash lines, where the spinning fan will stop and the cannon laser lines.

Optional in the minimap settings: a camera cone showing where you're looking, the game's own map underneath, background opacity and map rotation.

![Minimap](media/mechanic-minimap.png)

**On-me alert** — a big banner when a mechanic targets you, and **MOVE OFF** when you stand on a
danger tile. In the raid's ring phases it tells you which ring to move to. Sound is optional, with its
own volume; use **Test alert** to preview. (Off by default.)

![Alert banner](media/mechanic-alert.png)

Move the list, minimap and banner anywhere in the HUD layout editor.

## Notes

- Both overlays stay visible when the game menu (ESC) is open.
