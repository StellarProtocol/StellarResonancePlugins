# Maestro

A MIDI auto-player for Star Resonance's Season-3 band (Musician) instrument. Drop your `.mid`
files in a folder, pick a song, and Maestro performs it for you on your summoned instrument —
solo or in a group — with playlists, group sync, and a local sound preview.

![Performing in-world with the Maestro window open](media/maestro-in-world.png)

## Getting started

1. Install the plugin and launch the game **Modded**.
2. Open **Maestro** from the Stellar launcher (Plugins group). Band tools are in-world only.
3. Put your `.mid` / `.midi` files in the game's `midi\` folder — Maestro creates it for you. In the
   **Library**, **Open folder** takes you straight there, and **Rescan** picks up files you've added.
4. Summon a free-play instrument in-game, press **+ Add** to pick songs from the Library, and press
   **Play**.

## Maestro Link — play as a band from the website

Maestro Link lets a whole band play together without an in-game ensemble. A band leader runs a room
on the website [maestro.stellarresonance.app](https://maestro.stellarresonance.app), picks the songs
and decides who plays which part — then every player's character equips its instrument and plays
its part by itself, in sync with everyone else.

![Playing a Maestro Link song on piano in-world](media/link-in-world.png)

### Joining a room

1. Get the **6-character room code** — create a room on the website, or ask your band leader for it.
2. In-game, open **Maestro** → **Maestro Link**.
3. Type the code in the box and press **Join**.
4. That's it — when the room plays a song, your character plays its part automatically.

The **How it works** panel sums up these steps, and **Open website** opens the Maestro Link site.

![Maestro Link — enter the room code and press Join](media/link-join.png)

### In a room

![Maestro Link in a room — room, now playing and the players list](media/link-connected.png)

- **Room** shows the room code and name, your ping and the part you're playing.
- **Now playing** shows the current song, its progress and what's **Next**.
- **Players** lists everyone in the room with their name, instrument, state and ping. The state is
  **Playing**, **Ready**, **Idle** or **Problem**. *Problem* only appears when a part that's playing
  right now can't play — not having an instrument yet is fine, because Link equips it for the next
  song. **Hide** collapses the list.
- **Stop my part** (amber) rests your part while the band keeps playing; the button then becomes
  **Resume my part**.
- **Disconnect** (red) leaves the room. Logging out disconnects you automatically.

### The website

The room page on the website is where the band is run: it lists everyone who joined, shows
**Now playing** (pause, stop, skip, repeat), and holds the song queue, where you choose who plays
each part. One part can go to several players, and one player can take several parts of the same
instrument. **Room settings** apply to every player at once, and you can still adjust any single
player on their own.

![The Maestro Link website — players, Now playing and the song queue](media/web-room.png)

## Playing songs

The main window is your player. At the top, **Now playing** shows the current song and its
progress, with ◀◀ / **Play** (**Pause**) / ▶▶ / **Stop** and a **Next:** line telling you what
comes after it.

![The main window — Now playing, the queue and the four cards](media/auto-player.png)

**The queue**

- Pick a playlist from the dropdown, or use **+ New**, **Rename** and **✕** (delete the playlist).
- **Drag a song** up or down to reorder it, and press the **✕** on a row to remove it.
- Switching to another playlist doesn't stop the song that's playing — it keeps going until you pick
  a song in the other playlist and press **Play**.
- The icons under the queue switch **Auto next**, **Loop** (off / all songs / one song) and
  **Shuffle**. Hover an icon and its name and setting appear in the **QUEUE** heading.
- **Shuffle** plays every song in the playlist once, in random order. With Loop off it stops after
  the last song; with Loop all it reshuffles for each new round. The **Next:** line always shows the
  song that will really play next.
- **+ Add** opens the Library.

At the bottom, four cards open the other windows: **Library**, **Maestro Link**, **Preview** and
**Settings**. Each card shows a quick status, such as your song count or whether you're in a room.

## Finding songs

![Library — search your songs and click one to add it to the playlist](media/library.png)

- **Search** by name (✕ clears the search), **Rescan** to pick up new files, and **Open folder** to
  jump to your MIDI folder.
- **Click a song** to add it to the current playlist. The row flashes green with **Added**, and the
  line at the bottom confirms it, e.g. *✓ Added "Bad Apple (Piano)" to Playlist 1 · 11 songs*.
- Playback performs a single instrument at a time, so just add the stem you want to play.

## Preview before you play

Audition a song through the **real in-game instrument sounds** without summoning an instrument — the
preview plays just for you, so no one around you hears it.

![Preview — the song being previewed, band sync options and one row per stem](media/preview.png)

- **Previewing** shows the selected queue song with its progress, plus **Preview** and **Stop**.
- **Sync with band**: **Instrument Sync** waits for your band performance to start and plays along
  with it — mute the parts you're playing yourself. **Sync offset** delays the preview so it lines
  up with what the band hears; **= Band delay** sets it for you.
- **Stems** has one row per file, each with a mute toggle, its instrument, a **Sustain** mode and a
  **Tone** switch, so you can hear exactly how a song will come out.

Preview is where the multi-stem naming matters: give a song's parts the same base name, ending in
the instrument in parentheses, and Preview loads the **whole set** when you select any one of them.

```
Song (Piano).mid   Song (Guitar).mid   Song (Bass).mid   Song (Bass 2).mid   Song (Drum).mid
```

Duplicates like `(Bass 2)` become their own track on the same instrument sound.

## Per-song controls & group play

Open the **Settings** card for all fine-tuning and group options, in one window with two columns.
Every setting has a round **?** button that opens a short explanation — click it again, or its ✕, to
close it. Each slider also has a **↺** button that puts it back to its default.

![Settings — Playback, Ensemble, Sound and Network Sync, with the Monitor volume help open](media/settings.png)

- **Playback**: Transpose, Tempo, Note hold, Max notes (how many notes may sound at once),
  Restrike gap (default 33 ms, so fast repeated notes don't drop) and Gap between songs.
- **Ensemble**: **Sync to ensemble** locks playback to your group's shared beat (count-in to the
  downbeat), **Match ensemble tempo** follows the ensemble's BPM, and **Auto-accept ensemble
  invites** saves you clicking Accept. Join or start an ensemble in-game first.
- **Sound**: Monitor volume, Force sustain and Apply tone from MIDI instrument.
- **Network Sync**: **Network Sync Mode** streams your notes ahead of time so listeners around you
  hear a steadier performance, even with dense note streams. It also unlocks **Mute my local sound**
  and **Show key animation**. Fine-tune it with **Lookahead**, **Send every** and **Tone lead**.
  **Tone relay workaround (experimental)** is explained under *Known limitation* below.

![The Network Sync section of Settings](media/network.png)

## Playing in an ensemble

Ensemble mode locks a whole party's performance to the same beat, so several players can play
different parts of the same song together, in sync. Everyone must be **in the same party**.

1. **Turn on ensemble sync.** In **Settings**, enable **Sync to ensemble** — on every player. Turning
   on **Auto-accept ensemble invites** too is optional, so you don't have to accept each invite by
   hand.
2. **Each player picks their part and presses Play.** Choose the stem you'll play and press Play —
   instead of starting, Maestro holds and shows **"Waiting for the ensemble…"**.
3. **The party leader starts the ensemble in-game.** This is the game's own ensemble start.
4. **Everyone plays in sync.** All the waiting players begin together on the downbeat, locked to the
   ensemble's beat.

For a full band, have each player queue a **different** part (Guitar, Bass, Drum, Piano) of the same
song, and optionally turn on **Match ensemble tempo** so playback follows the ensemble's BPM. Use
**Preview** beforehand to hear how the whole set fits together.

## Preparing your MIDI files

Maestro plays your MIDI as written, so a little prep makes songs sound right on the game
instruments.

### Effects (tone & technique)

Turn on **Apply tone from MIDI instrument** in Settings, and Maestro picks the
guitar/bass effect from the **instrument (program) assigned to the stem's track**. Set that track's
General MIDI instrument in your DAW:

**Guitar stems**

| Assign this GM instrument | Program # | Plays as |
|---|:--:|---|
| Nylon / Steel / Jazz / Clean Electric Guitar | 25–28 | Clean |
| Muted Guitar | 29 | Muffled (palm-mute) |
| Overdriven Guitar | 30 | Overdrive |
| Distortion Guitar | 31 | Distortion |
| Guitar Harmonics | 32 | Harmonics |

**Bass stems**

| Assign this GM instrument | Program # | Plays as |
|---|:--:|---|
| Acoustic / Finger / Pick / Fretless Bass | 33–36 | Clean |
| Slap Bass 1 / 2 | 37 / 38 | Slap |
| Synth Bass 1 / 2 | 39 / 40 | Overdrive |

- Program numbers are the **1–128** values your DAW shows; match by **instrument name** if unsure.
- Only the stem's **main instrument** is read, so keep one instrument per stem. A program change
  partway through the track switches the effect from that point on.
- Effects apply to **guitar and bass only** — piano and drums ignore them.
- The effect is matched to the instrument you actually summon. **Bass has no distortion** — it
  plays as Overdrive — and any technique the summoned instrument can't do falls back to normal.
- **Overdrive / Distortion is local-only in Network Sync** (see *Known limitation* below). Muffled,
  Harmonics and Slap are heard by everyone.

### Drums

The game drum kit is a fixed **9-piece kit** on the keys below, and Maestro plays your notes exactly
as written — it does **not** auto-convert General MIDI drums — so a drum stem must use these keys:

| MIDI note | Piece |
|:--:|---|
| 62 (D4) | Closed Hi-Hat |
| 65 (F4) | Kick |
| 69 (A4) | Floor Tom |
| 72 (C5) | Snare |
| 74 (D5) | Mid Tom |
| 76 (E5) | High Tom |
| 77 (F5) | Ride |
| 79 (G5) | Open Hi-Hat |
| 81 (A5) | Crash |

Any note off these keys is silent. Starting from a standard General MIDI drum track? Remap the
usual GM notes onto these keys — kick (GM 35/36) → **F4**, snare (38/40) → **C5**, closed hat (42)
→ **D4**, open hat (46) → **G5**, ride (51) → **F5**, crash (49) → **A5**, toms → **A4 / D5 / E5**.

### A few more tips

- **One note per pitch:** each instrument sounds one voice per pitch, so two identical overlapping
  notes count as one — avoid stacked unisons.
- **Mind the range:** notes outside an instrument's playable range are silent. Use **Transpose**
  (Settings) to bring a part into range.
- **Volume isn't reproduced:** every note plays at a fixed loudness, so velocity and dynamics won't
  carry over.
- MIDI **type 0 or 1** files are supported.

## Known limitation

**Overdrive / Distortion doesn't reach other players — this is a game bug, not something Maestro can
fix.** The game only ever renders the guitar/bass overdrive/distortion *tone* on your own client and
never sends it to the people around you. So it plays Clean for everyone else whenever **Network Sync
is on**, and even with Network Sync **off**, only *you* hear the distortion — other players never
hear your overdrive/distortion in either mode. The experimental **Tone relay workaround** in
Settings sends your tone another way, but only listeners who also run Stellar hear it; everyone else
still hears Clean until the game fixes it. *Techniques* (Muffled, Harmonics, Slap) are unaffected
and are heard by everyone. For distortion-critical songs, play with Network Sync **off** so at least
it sounds right to you.
