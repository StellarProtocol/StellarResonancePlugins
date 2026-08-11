# Maestro

A MIDI auto-player for Star Resonance's Season-3 band (Musician) instrument. Drop your `.mid`
files in a folder, pick a song, and Maestro performs it for you on your summoned instrument —
solo or in a group — with playlists, group sync, and a local sound preview.

![Performing in-world with the Library, Auto-Player and Preview windows open](media/maestro-in-world.png)

## Getting started

1. Install the plugin and launch the game **Modded**.
2. Open **Maestro** from the Stellar launcher (Plugins group). Band tools are in-world only.
3. Put your `.mid` / `.midi` files in the game's `midi\` folder — Maestro creates it and shows the
   path in the **Library** window. Use **Rescan folder** or **Locate** if you move it.
4. Summon a free-play instrument in-game, add songs from the **Library** to the queue, and hit ▶.

## Playing songs

The main window is your player: a playlist, the now-playing queue, and transport controls.

![Auto-Player — playlist, queue and playback controls](media/auto-player.png)

- Build named **playlists** and reorder the queue; the header shows the current song and position.
- **Auto-advance**, **Loop** (off / all / one), **Shuffle**, and a **gap between songs** are all
  one click away.
- The status line shows the live position and note count for the song that's playing.

## Finding songs

![Library — browse your MIDI folder and add songs to the queue](media/library.png)

- Browse your whole MIDI folder, **search** by name, and click a song to add it to the queue.
- Give a song's parts the same base name, ending in the instrument in parentheses, so Maestro
  loads them together:

  ```
  Song (Piano).mid   Song (Guitar).mid   Song (Bass).mid   Song (Bass 2).mid   Song (Drum).mid
  ```

  Duplicates like `(Bass 2)` become their own track on the same instrument sound.

## Preview before you play

Audition a song through the **real in-game instrument sounds** without summoning an instrument — the
preview plays just for you, so no one around you hears it.

![Local Preview — audition through the real in-game instrument sounds](media/preview.png)

- One row per stem, with per-part **mute** and a **sustain** mode, so you can hear exactly how a
  song will come out.
- **Instrument Sync** waits for the live band player and follows along, muting the parts you'll
  play yourself — handy for jamming with someone else.

## Per-song controls & group play

Open **Settings** for the fine-tuning and group options.

![Settings — per-song controls, Network Sync and Ensemble options](media/settings.png)

- **Per-song**: Transpose, Note hold, Tempo %, Max notes (polyphony cap), Restrike gap, Monitor
  volume, Force sustain, and Apply tone/technique from the MIDI.
- **Network Sync** streams your notes ahead of time so listeners around you hear a steadier
  performance under dense note streams.
- **Ensemble**: lock playback to your group's shared beat (count-in to the downbeat), optionally
  match the ensemble tempo, and auto-accept invites so everyone starts together. Join or start an
  ensemble in-game first.

## Known limitation

**Overdrive / Distortion is local-only.** Guitar and bass distortion tone renders only through the
game's live play path, so it plays Clean whenever **Network Sync is on**. And even with Network Sync
**off**, only *you* hear the distortion — other players never hear your overdrive/distortion in
either mode. *Techniques* (Muffled, Harmonic, Slap) work everywhere and are heard by everyone. For
distortion-critical songs, play with Network Sync **off** so at least it sounds right to you.
