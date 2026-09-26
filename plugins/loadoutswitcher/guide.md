# LoadoutSwitcher

Switch between your saved in-game loadouts (Role Plans) with a hotkey.

## How to use

1. Install the plugin and launch the game **Modded**.
2. Save your loadouts in the game as usual (Role Plans).
3. Open **Stellar Settings → Hotkeys**, expand the **loadout** group and bind keys to
   **apply.1** through **apply.8** (they ship unbound). Hotkey *n* applies the *n*-th
   loadout in your saved list.

![Hotkey bindings](media/loadoutswitcher-hotkeys.png)

4. Press the hotkey in the world — the plugin switches through the game's own loadout flow.

## Copying a loadout

Open the Loadout Switcher window. Every loadout except the one you're wearing shows a
button with your current loadout's name, like "← Ici-LF". Click it on the row you want to
overwrite, check the question ("Overwrite with Ici-LF?") and press ✓.

- It copies what you're wearing right now: gear, modules, skills, talents, Battle Imagines
  and the Deep-Slumber binding. The loadout's name stays the same.
- If you have unsaved changes, or the copy would change that loadout's class, the confirm
  shows a warning line first.
- The game confirms with its own "Loadout saved successfully!" message. If the game refuses
  (for example in combat), nothing is changed.

## Tips

- Turn on **Block hotkeys from game** at the top of the Hotkeys panel so your bound keys
  don't also trigger game actions while you switch.

## Feedback

Switch results use the game's native notice banners: a success banner when the plan is
applied, and a guard notice when switching isn't possible right now (for example while
another switch is still in flight, or when the loadout API isn't ready yet).
