# Auto Fishing

Fully automates the fishing minigame — from casting to catch — so you can leave it running
unattended.

![Auto Fishing window](media/autofishing-running.png)

## How to use

1. Install the plugin and launch the game **Modded**.
2. Go to a fishing spot with a rod equipped and open **Auto Fishing** from the Stellar menu.
3. Press **Start**. The plugin casts, plays the tension minigame and lands the catch on its own,
   then starts over. Press **Stop** (or close the window) to stop.

## The window

- **Status line** under the title: *Go to a fishing spot to start*, *Ready*, *Running* with the
  current step, or *Paused* while it closes a popup. **Start** is greyed out until you're at a
  fishing spot.
- **Session** counts the fish caught, fish missed and rods used since you pressed Start.
- A short message at the bottom tells you about each catch, missed fish or rod change.

## Settings

- **Release at** — while reeling, the rod is let go when the tension bar reaches this value, so
  the line doesn't snap. Lower is safer but slower. Default 25%.
- **Pull at** — once tension drops to this value, the rod is held again. Must stay below
  Release at. Default 8%.
- **Auto rod change** — equips the best rod from your bag when yours breaks or none is equipped.
  If off, fishing waits until you equip one yourself.
- **Close daily login popup** — closes the daily login card and reward popups when they appear,
  then resumes fishing after a moment.
- Press **?** next to a setting for a short explanation.

![Help popover](media/autofishing-help.png)
