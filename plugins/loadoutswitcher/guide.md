# LoadoutSwitcher

Switch between your saved in-game loadouts (Role Plans) with a hotkey.

## How to use

1. Install the plugin and launch the game **Modded**.
2. Save your loadouts in the game as usual (Role Plans).
3. Open **Stellar Settings → Hotkeys** and bind keys to **Apply Loadout 1** through
   **Apply Loadout 10** (they ship unbound). Hotkey *n* applies the *n*-th loadout in your
   saved list.

![Hotkey bindings](media/loadoutswitcher-hotkeys.png)

4. Press the hotkey in the world — the plugin switches through the game's own loadout flow.

## The window

Open the Loadout Switcher window to see the status line at the top (restore on or off, and
the loadout you're wearing), the **Restore factors** switch, and your loadouts. The loadout
you're wearing is marked with a dot. Press a **?** button for help on that section.

![Loadout Switcher window](media/loadoutswitcher-window.png)

## Restoring factors

Each loadout can remember a Deep-Slumber setup (its cultivate line and phantom factors).

1. Wear the loadout, set up Deep-Slumber the way you want it, and press **Bind current** on
   that loadout's row. The row then shows how many areas and factors are bound.
2. Turn **Restore factors** **On**. Switching to a loadout now restores its bound factors.
3. Pressing the hotkey of the loadout you're already wearing restores its factors again.

- Restoring only works in **Season 3**. In any other season it stays off and the window says
  so. Your bindings are kept, and switching loadouts still works.
- Press **✕** on a row to remove its binding. The loadout itself stays.

## Copying a loadout

1. Wear the loadout you want to copy.
2. Press **Copy here** on the row you want to overwrite.
3. The row asks "Overwrite *row* with *worn*?" (for example "Overwrite Tank with DPS?").
   Press **Overwrite**, or **Cancel** to leave it as it is.

- It copies what you're wearing right now: gear, modules, skills, talents, Battle Imagines
  and the Deep-Slumber binding. The loadout's name stays the same.
- If you have unsaved changes, or the copy would change that loadout's class, a warning line
  appears in the footer before you confirm.
- The game confirms with its own "Loadout saved successfully!" message. If the game refuses
  (for example in combat), nothing is changed.

## Tips

- Turn on **Don't pass hotkeys through to the game** at the top of the Hotkeys panel so your
  bound keys don't also trigger game actions while you switch.

## Feedback

Switch results use the game's native notice banners: a success banner when the plan is
applied, and a guard notice when switching isn't possible right now (for example while
another switch is still in flight, or when the loadout API isn't ready yet).
