# Atlas

A shared map of chests, collectibles and the magical creatures of Asteria Plains. Atlas lists the nearest ones
around you with their distance and direction, and puts the game's own guide marker on the one you pick. The points
you turn on are also drawn on the game's big map and minimap.

![Atlas](media/window.png)

## Getting started

1. Install the plugin and launch the game **Modded**.
2. Log in to a character and open the **Atlas** tile in the Stellar launcher.
3. The first time, Atlas shows a notice, **Atlas is a shared map**, that explains what is shared (see
   *Sharing and privacy* below). Press **Got it** to start. Nothing is shared before you do.
4. Under **SHOW**, turn on what you want to see.

The header shows your map, your line and how many chests Atlas knows on this map. When the Atlas server can't be
reached, it says **Offline** and Atlas keeps working from the copy saved from your last visit.

## Chests and collectibles

**SHOW** (click a chip to turn it on or off; your choice is saved per character)

- **Chests**: **Common**, **Epic**, **Legendary** and **Other**.
- **Collectibles**: **Books**.
- **Magical creatures**: one chip per creature (see below). They are off by default.

**NEAREST** lists the closest points of the types you turned on, with their direction (**N**, **NE**, …) and
distance. It combines the shared Atlas map with what the game shows you live. The tag on each row says where it
comes from:

- **Seen live**: the game is showing it to you right now.
- **Map data**: from the shared Atlas map.
- **Unverified**: reported by other players, but not confirmed yet.
- **Opened**: you just opened it. The row leaves the list a moment later.

**Click a row to be guided there.** The game's own guide marker appears on that spot, with its distance. It clears
itself when you arrive, and **Stop guiding** clears it at any time. If you open the chest you were guided to, the
guide moves on to the next nearest point you have turned on.

Chests you open leave your list, only for that character. Chests you opened before you installed Atlas disappear
by themselves once you stand near the spot and nothing is there. If the game shows one there again, it comes back.
This is kept on your PC and is never shared.

When nothing is known yet on a map, the list says so. The chests you find are added for everyone.

## Magical creatures

Atlas follows the five magical creatures of Asteria Plains: **Golden Nappo**, **Silver Nappo**, **Loyal Boarlet**,
**Lovely Boarlet** and **Breezy Boarlet**. Turn any of them on in **SHOW**, and the **MAGICAL CREATURES** panel
appears above **NEAREST** with one row per creature:

- **On your line** or **Line 7** / **Lines 2 · 6 · 9**: the lines it is alive on (yours first), with its spot, HP
  and distance.
- **Seen live**: it is in view right now, with its exact position and HP (and **fighting** while someone is
  fighting it).
- **Absent**: Lovely Boarlet and Breezy Boarlet appear on a fixed schedule. While they are gone, the row shows the
  **next spawn** time and how long until then.
- **Seen by a player**: another Atlas player spotted it, with the line and how long ago.

**Click a creature to open its lines.**

- **Alive only** lists the lines it is alive on. **All N** lists every line of the server, with **Not seen** or the
  next spawn time for the others.
- Every line shows its spot, HP and when it was last updated. Your line is outlined.
- Pick a line, then press **Guide me there**. The game's guide marker goes to that spot. From another map, the
  marker first points to the exit toward Asteria Plains (**Guiding you toward …**). When there is no exit to point
  to, the guide starts once you arrive.
- Switching line is up to you: Atlas only shows you where to go.

The status of every line refreshes about every 30 seconds while you are in Asteria Plains or the window is open.

**Live status for every line is available on the SEA server only for now.** On other servers you see the creatures
other Atlas players spotted, and a note under the list says so.

## Map icons

![Atlas on the big map](media/map.png)

- Every point you turn on in **SHOW** is drawn on the game's big map and minimap. Turn a chip off and its icons go
  away.
- You can pick an Atlas icon on the big map and use the game's own buttons to head there.
- Magical creatures show as round head icons: a green ring when it is alive on your line, a line badge (for example
  **7**) when it is on another line, and slightly faded with its next spawn time while a scheduled boarlet is absent.

## Sharing and privacy

**Atlas is a shared map.** While Atlas runs, what you find and where (chests, resources and magical creatures) is
shared with the Atlas server, so the map stays complete for everyone.

- Your name, UID and chat are **never** sent.
- Nothing is shared until you press **Got it** on the first-run notice.
- The footer of the window reminds you: *Shared map · what you find and where is shared. No names.*
- **To stop sharing, turn Atlas off in the launcher.**

## Tips and FAQ

- **The list is empty.** Turn on a type in **SHOW**. If a map has no known chests yet, the ones you find are added.
- **A chest on the list isn't there.** It was probably opened earlier on this character. Stand near the spot for a
  moment and Atlas hides it for that character.
- **It says Offline.** Atlas can't reach its server right now. Chests still work from the saved copy, and live
  creature status comes back once it reconnects.
- **I'm not on SEA.** Creatures still show when other Atlas players spot them, but not every line live.
- Atlas opens only while you're in the world, and its windows close when you log out.
