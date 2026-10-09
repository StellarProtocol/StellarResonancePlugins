# Raid Manager

Raid coordination tools shown as a HUD overlay — compatible with the ZDPS raid-call
conventions your statics already use.

## Countdown & Raid Warning

1. Install the plugin and launch the game **Modded**.
2. Type `/ct <seconds>` in chat to start a big on-screen pull countdown (turns red in the last 5 seconds).
3. Type `/rw <message>` to flash a raid-warning callout to everyone running the plugin.

![Countdown](images/countdown.png)

- **Use In-game Warning** (default on) — `/rw` shows the game's native notice banner with
  victory audio instead of the plugin's custom overlay, so callouts look and sound like the
  game's own alerts.

## Mark Presets

Save the dungeon markers you place and reload the whole layout in one click — as multi-step
phase sequences. Open it from **Raid Manager → Open Mark Presets**:

![Raid Manager settings](images/raid-manager-settings.png)

**Build a preset**

1. **Create** a preset, give it a name, and **Activate** it.
2. In a dungeon, place your markers for the first phase, then **+ Save Step**.
3. Place the next phase's markers and **+ Save Step** again — repeat for each phase.

**Use it during the fight**

- **Next ▶** / **◀ Prev** move through the steps, clearing the board and placing that phase's markers.
- **Reset** returns to the blank **Start**.
- Bind **Mark Presets: Previous step**, **Mark Presets: Reset to Start** and **Mark Presets: Next step**
  to your own keys in **Stellar Settings → Hotkeys** (they start unbound).

![Mark Presets](images/mark-presets.png)

Keep a separate preset for each raid and **Activate** the one you're running (the button toggles to
**Deactivate**). Marker placement works in dungeons.

**Rename and add notes**

- Click the **pencil** next to a preset to rename it.
- Under the current step, click the **pencil** to add a note — e.g. "P2 — stack west". The note also
  shows in the pop-up when you move to that step.

**Share a preset**

1. Activate the preset and click **Export Preset**, then **Copy** — or select the code and press Ctrl+C.
2. Send the code to your group.
3. They click **Import Preset**, paste the code and press **Import**. They get the whole preset —
   every step, marker and note.

## Notes

- Both overlays stay visible when the game menu (ESC) is open.
