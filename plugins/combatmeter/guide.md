# CombatMeter

A real-time party combat meter for Star Resonance: live **DPS**, **HPS** and **damage taken**
for everyone in your party, with role colours and class crests so you can read the group at a
glance.

![Live party meter](media/combat-meter.png)

## Getting started

1. Install the plugin and launch the game **Modded**.
2. The meter appears as an overlay window once you're in the world. Drag its header to move
   it — the position is remembered. **F9** shows or hides it.
3. Enter combat: rows appear per party member and update live.

## The CombatMeter hub

Click the **CombatMeter** tile in the Stellar launcher to open the hub. Its header shows the
meter's live status, and it has three cards:

- **Meter** — click to show or hide the meter (the same as F9). The chip on the right says
  **Shown** or **Hidden**; the line below shows the current view and metric.
- **Combat History** — how many runs are saved. Click to open it (or press **Shift+F9**).
- **Settings** — opens the Settings window.

![CombatMeter hub](media/combatmeter-hub.png)

## Reading the meter

- The header has the metric menu (**DPS** / **HPS** / **Taken**), the **List | Party** switch, a
  live status line (fight timer, player count), and four round buttons: **Pause**, **Archive**,
  **Reset** and **⋯** (more).
- **Pause** stops the meter counting; the button turns amber and the status line says
  **Paused**. Click it again to resume.
- **Archive** saves the current fight to Combat History right away. It's greyed out until
  something has been recorded, and flashes a green ✓ when the fight is saved.
- **⋯** opens a small menu: in List view, pick who is listed (**Self** / **Party** / **All**); in
  Party view, the leader can switch **Party size** between 5 and 20. It also has shortcuts to
  Combat History and Settings.
- Each row is one player. Row colours follow the member's **role** (tank / healer / DPS) and the
  crest shows their class. Buffs and debuffs on a row show their icons: the skill's icon when
  the effect comes from a skill, otherwise a category icon.
- In Party view, the party leader gets a **Leader** line under the header with **Mark**,
  **Ready check**, **Convene** and **Countdown** (the game's pull timer for the whole party).

## Row size

Pick how much space each player takes under **Settings → Look → Row → Row size**:

- **Normal** — two lines per player, with every detail.
- **Compact** — one line per player, with the bar behind the text. A raid of 20 takes far less
  space.
- **Mini** — just the bar with rank, name and numbers.

![Compact rows](media/combatmeter-compact.png)

![Mini rows](media/combatmeter-mini.png)

Each view (**List**, **Party (5)**, **Party (20)**) keeps its own size and look: choose the view
to change with **Editing** at the top of the Look tab. Settings the chosen size doesn't draw
are greyed out and keep their value.

## Settings

One Settings window with five tabs. The **?** next to a setting explains what it does.

- **Look** — what each row shows (rank, crest, name line, ability score, share, totals,
  per second…), **Row size**, Bars, Battle Imagine and Numbers. Under **Meter window**, three
  sliders apply to every view:
  - **Background** — a rounded panel behind the meter. 0 % keeps the meter see-through.
  - **Row background** — fades only the dark backing of each row, so you see more of the game
    behind it. Bars, names and numbers stay solid.
  - **Foreground** — fades the whole meter (never below 20 %, so it never disappears).
- **Effects** — the buffs and debuffs shown on rows. Choose **All except ticked** or **Only
  ticked**, then tick effects in the Buffs and Debuffs lists (with search). Turn on **Hidden
  effects** to also list effects the game doesn't normally show. Compact rows show up to 3 effects; Mini rows show none.
- **Archive** — when fights are saved automatically (see below) and how many runs History keeps.
- **Uploads** — what uploads to StellarLogs (**Auto** / **Manual** / **Off** per content type),
  plus an optional Discord webhook that posts your runs.
- **Account** — link this character to your StellarLogs account with a code from the site.

## Combat History

![Combat History](media/combatmeter-history.png)

- **Runs** on the left, newest first, with a search box. Select a run to see it on the right.
- The run panel shows its upload status (**Uploaded** / **Not uploaded**), **Upload segment**,
  **Upload all**, and round buttons for **Copy link** and **Delete**. Below it, the **Segments**
  of the run (trash clear, boss kill, run end…) — pick one to view it.
- The chart switches between **DPS**, **HPS** and **Taken**; click a player in the table to add
  their line.
- In the player table, **›** opens that player's Skill Breakdown and **⌕** shows their gear
  and stats from that fight.
- **Clear all**, **Delete** and **Upload all** always ask before they act. A deleted run stays
  on StellarLogs if it was already uploaded.

## Skill Breakdown

Open it with **›** on a player in Combat History.

![Skill breakdown](media/skill-breakdown.png)

- **Totals** — total, DPS, crit %, luck %, max hit, hits, kills, uptime, healing and more,
  including **Taken** (the damage that player took).
- **Skills** — every skill with its damage, share, crit and luck. Skills the game doesn't name
  get a readable name instead of a number.
- Opened from the **Taken** chart, it lists the damage taken, skill by skill.

## Archives — saving a fight

An *archive* banks the current fight into Combat History and resets the live view.

- **Manual archive** (the **Archive** button in the meter header) is **always saved**, no
  matter what — you always get visible feedback that it was banked.
- **Auto-archive** can bank fights for you. Control it in **Settings → Archive**: master on/off,
  and when to archive (team wipe, boss phase, combat idle, dungeon stage change), minimum gap
  between archives, settle wait, wipe revive-grace, and ignore-when-solo.
- **Boss phase "keep before"** lets a few seconds of run-up before your first boss hit ride
  into the boss segment (default off = cut exactly at the first hit).
- An auto archive is skipped only when literally nothing happened (every row all-zero). A
  skipped archive wipes nothing — everything carries into the next one.

## Uploads and run replay

Archived runs upload to [Stellar Logs](https://logs.stellarresonance.app), where you get the
full run page: damage charts, skill details, boss phases — and a **movement replay** of the
whole run, from the moment you walked into the dungeon to the kill.

- **Copy link** in Combat History puts the run's short URL on your clipboard to share with your
  party.
- **Combat History** keeps your archived runs across relaunches. Re-uploading an archived run
  reproduces the exact original upload — summary, full combat detail, and the movement track —
  even if the server-side data was lost.

## Tips

- The meter works even if profession data hasn't loaded yet — role colour and crest are
  inferred from your spec until the roster fills in.
- If a run link fails to upload, retry from Combat History: an already-uploaded run resolves to
  its existing link instead of failing.
