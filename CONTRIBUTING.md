# Contributing

Thanks for helping grow the StellarResonance plugin ecosystem. This repo is the curated **plugin
registry** for the [StellarResonance](https://github.com/StellarProtocol/StellarResonanceModSystem)
framework — it holds **manifests only**. Each plugin's source lives in its **own public repo**, and
**our CI builds it** from a pinned commit.

> **Pipeline credit.** Our build-and-publish model is **inspired by Dalamud's
> [DIP17](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md)**
> — a manifest pins a public repo + commit, and CI clones and builds it — **customised for
> StellarResonance**: our own SDK on NuGet.org, committed interop reference stubs, a MinIO registry,
> per-plugin framework-compat gating, and container-isolated builds.

## What's allowed

Plugins must be **quality-of-life only**, matching the framework's policy:

> **Security model — read this.** A plugin DLL is **arbitrary code running in the game process**
> (BepInEx IL2CPP); it is **not sandboxed at runtime**. `Stellar.Abstractions` is a *read-only API
> shape*, **not** a security boundary — a plugin can ignore it and call Unity, game internals, the
> filesystem, or the network directly. Trust therefore comes from **reviewable source + a build we
> control**: every curated plugin is built by our CI from a **pinned commit in a public repo**, its
> source is reviewed in the PR, and the build runs **isolated in a container** (no secrets). There is
> **no "upload a prebuilt DLL" path**. PRs with cheat-shaped or hostile behaviour are rejected.

## Publish a plugin

1. **Put your plugin in its own public repo**, named **`Stellar<Name>Plugin`** (e.g.
   `StellarCombatMeterPlugin`). The assembly/DLL stays `Stellar.<Name>` (`Stellar.CombatMeter.dll`).
   Its `.csproj` references the SDK from NuGet.org — no framework checkout or game install:
   ```xml
   <ItemGroup>
     <PackageReference Include="Stellar.Abstractions" Version="1.1.1" />
     <PackageReference Include="Stellar.Plugin.InteropRefs" Version="1.1.1" />
     <!-- + Stellar.PluginContracts if you use the inter-plugin exchange -->
   </ItemGroup>
   ```
   Use a **fixed version** (no timestamp/auto-increment) so a given commit always builds the same binary.
2. **Add a manifest here** — `plugins/<your-id>/manifest.json` — pinning your repo + commit:
   ```json
   {
     "id": "yourplugin", "name": "Your Plugin", "description": "…", "author": "you",
     "dll": "Stellar.YourPlugin.dll",
     "repository": "https://github.com/you/StellarYourPlugin.git",
     "commit": "<full 40-char sha>", "tag": "v1.0.0", "projectPath": ".",
     "version": "1.0.0", "minModSystemVersion": "1.1.0", "channel": "testing"
   }
   ```
   `commit` is **authoritative** — CI builds that exact SHA. `tag` is **optional, display-only**
   provenance: CI verifies the tag resolves to the pinned commit but never builds from a tag alone
   (tags are mutable; a pinned commit is not).
   `tools/set-version.py <id> --version … --min … [--commit … --tag … --cap-prior …]` helps with
   version/compat fields.
3. **Open a PR.** CI **clones your repo at the pinned commit and builds it in an isolated container**,
   then validates the registry. Your pinned source + the manifest diff are the review surface.
4. On merge to `main`, `publish.yml` rebuilds from the pinned commit and — after the `Production`
   approval — publishes the registry + your DLL to MinIO, recording `sourceRepository`/`sourceCommit`
   (provenance). The launcher picks it up.

To **update**, bump `commit` (and `version`) in the manifest via a new PR.

## Channels

The build emits two registry files; the launcher fetches the one for the user's selected channel:

- **`plugins.json`** — **stable** versions only.
- **`plugins-testing.json`** — a **superset**: every version of every plugin (the *testing* channel).

A plugin's channels come from a **two-file source model**, so one plugin can be live on stable **and**
testing **at the same time** (a beta running alongside the proven release):

- **`plugins/<id>/manifest.json`** — the canonical record (all shared fields + one version). Its
  optional **`"channel"`** (default `"stable"`) is the channel of *that* version. Set `"testing"` for a
  **brand-new, not-yet-stable** plugin (it then appears only in `plugins-testing.json`).
- **`plugins/<id>/manifest.testing.json`** — *optional* sibling adding a **second, testing-channel
  build**. It **inherits the shared fields** (`id`/`name`/`dll`/`repository`/`projectPath`/…) from
  `manifest.json` and carries **only the version-specific overrides**:
  ```json
  { "version": "1.2.0-beta", "commit": "<beta sha>", "tag": "v1.2.0-beta", "minModSystemVersion": "1.1.0" }
  ```
  Result: `plugins.json` keeps the stable version; `plugins-testing.json` lists the beta **and** the
  stable version. (Shared fields live in exactly one place — they can't drift between the two files.)

Lifecycle, scripted (no hand-edited JSON):

- Start a beta:  `tools/set-version.py <id> --testing --version 1.2.0-beta --min 1.1.0 --commit <sha> [--tag …]`
- Promote it:    `tools/set-version.py <id> --promote`  (folds the testing build into `manifest.json`
  and removes the override — the beta becomes the new stable).

### Worked example — a beta alongside the stable release

`plugins/combatmeter/manifest.json` — the proven release, **unchanged**, stays on stable:

```json
{
  "id": "combatmeter", "name": "CombatMeter",
  "description": "Real-time party DPS/HPS meter.", "author": "Stellar",
  "dll": "Stellar.CombatMeter.dll",
  "repository": "https://github.com/StellarProtocol/StellarCombatMeterPlugin.git",
  "commit": "a517395f68d995b319504b77c52a4519f95f4aa4", "projectPath": ".",
  "version": "1.1.0", "minModSystemVersion": "1.1.0"
}
```

`plugins/combatmeter/manifest.testing.json` — the beta. It carries **only** the version-specific
fields; `id`/`name`/`dll`/`repository`/`projectPath`/`author`/`description` are **inherited** from
`manifest.json`, so they can't drift:

```json
{
  "version": "1.2.0-beta",
  "commit": "9f3c1d20e7b4a6f8c2d1e0b9a8f7c6d5e4b3a2f1",
  "tag": "v1.2.0-beta",
  "minModSystemVersion": "1.1.0",
  "changelog": { "added": ["New encounter-timeline view (beta)."] }
}
```

Published result — the launcher reads one file per the user's selected channel:

| Channel file | combatmeter `versions[]` (newest first) |
|---|---|
| `plugins.json` (stable) | `1.1.0`, …history — **no beta** |
| `plugins-testing.json` (testing) | `1.2.0-beta`, `1.1.0`, …history |

CI builds **both** commits and uploads each under its own version-specific DLL key, so a tester can
install `1.2.0-beta` while everyone on stable keeps `1.1.0`.

**Use case:** ship a risky/early build to opt-in *testing*-channel users for feedback **without**
disturbing the stable release everyone else runs. When the beta proves out,
`set-version.py combatmeter --promote` makes it the new stable (and deletes the override); if it's
abandoned, just delete `manifest.testing.json`.

> **`manifest.testing.json` vs `"channel": "testing"`** — two different things. The override file adds a
> testing build *alongside* a stable one (the plugin is on **both** channels). Setting `"channel":
> "testing"` on `manifest.json` itself makes the plugin testing-**only** — it leaves stable entirely.
> Use the latter for a brand-new plugin that has never had a stable release.

## Manifest data structure

### `plugins/<id>/manifest.json` — the canonical record

| Field | Type | Required | Notes |
|---|---|---|---|
| `id` | string | ✓ | unique plugin id; URL-safe (no `/` or `..`) |
| `name` | string | ✓ | display name in the launcher |
| `description` | string | ✓ | one-line summary in the launcher |
| `author` | string | ✓ | author handle |
| `dll` | string | ✓ | assembly filename, e.g. `Stellar.MyPlugin.dll` (the on-disk install name) |
| `repository` | string (git URL) | ✓¹ | public repo CI clones and builds |
| `commit` | string (40-hex) | ✓² | **authoritative** pinned commit CI builds + attests |
| `tag` | string | — | display-only provenance; CI verifies `tag` → `commit`, never builds from it |
| `projectPath` | string | — | path within the repo to `dotnet build` (default `"."`) |
| `version` | string (semver) | ✓ | this build's version |
| `minModSystemVersion` | string (semver) | ✓ | lowest framework version this build runs on |
| `maxModSystemVersion` | string \| null | — | upper bound; `null`/omitted = none |
| `capPriorVersionsAt` | string (semver) | — | retro-cap already-published versions' `maxModSystemVersion` at this framework version |
| `channel` | `"stable"` \| `"testing"` | — | channel of **this** version (default `"stable"`; `"testing"` = the plugin is testing-**only**) |
| `date` | string `YYYY-MM-DD` | — | release date (UTC) |
| `changelog` | object | — | `{ "added": [], "changed": [], "fixed": [], "removed": [] }` — any subset; arrays of strings |
| `tags` | string[] | — | keyword chips on the launcher's plugin detail page |
| `homepage` | string (URL) | — | http(s) link shown as "Homepage ↗" |
| `media` | array | — | detail-page gallery — see below |
| `guide` | string (path) | — | repo-relative markdown usage guide (conventionally `guide.md`, ≤ 1 MB); CI publishes it to `plugins/<id>/guide.md` |
| `icon` | string (path or URL) | — | badge image shown on the launcher's plugin list and detail header; repo-relative file (published to `plugins/<id>/icon.<ext>`) or absolute http(s) URL. Without it the launcher uses the first `media` image, else a monogram tile. |
| `dependencies` | array | — | things the launcher installs alongside this build (DLLs the plugin needs, a shared asset the game itself needs, …) — see below |
| `i18n` | object | — | per-language name / description / media captions / this version's changelog — see [Translations](#translations-i18n--guidelangmd) below |

¹ Required by the **curated** registry (CI refuses a manifest without a pinned public repo).
² Required whenever `repository` is set.

#### Detail page media + guide (`tags` / `homepage` / `media` / `guide`)

These optional fields feed the launcher's **plugin detail page** so users can see what a plugin
does before installing. Each `media` entry is
`{ "type": "image" | "youtube" | "video", "url" or "file", "caption"? }`:

- `"image"` — a screenshot. Use `file` for a picture committed in `plugins/<id>/` (e.g.
  `"media/overview.png"`, ≤ 25 MB; CI uploads it to `plugins/<id>/media/<name>` and publishes the
  CDN URL) or `url` for an already-hosted http(s) image.
- `"youtube"` — a watch/shorts/embed link (`url` required). The launcher shows the video
  thumbnail and opens the watch page in the user's browser.
- `"video"` — a hosted video file (`url` or attached `file`); the launcher shows a ▶ tile that
  opens it in the browser.

The `guide` markdown renders natively in the launcher (headings, lists, code fences, quotes,
links, images, bold/italic — raw HTML stays literal text). Write it for **players**: what the
plugin does, how to open/use it, and tips. **Reference your screenshots with relative paths**
(`![Overview](media/overview.png)`) — the launcher resolves them against the published guide's
own URL, so you never write your plugin id or any CDN base, and the same guide renders
correctly on GitHub. Guides and media live at stable, non-versioned CDN keys — fixing a typo
is just another PR, no release needed.

#### Translations (`i18n` + `guide.<lang>.md`)

The launcher shows a plugin in the player's launcher language when a translation exists, and falls
back **per field** to English otherwise. English always lives in the normal top-level fields — they
stay required and are the fallback. Supported language codes: `ja`, `th`, `id`, `fil`, `ko` (`en` is
not an `i18n` key). Everything here is optional and additive: older launchers ignore it.

**Guides** — put `guide.<lang>.md` next to your `guide.md` (same folder, same name with the
language code before `.md`, ≤ 1 MB). CI finds them automatically (no manifest field), publishes each
to `plugins/<id>/guide.<lang>.md` beside the English guide, and lists the ones present in the
registry entry's `guideUrls` (`{ "ja": "<url>", … }`). Because they publish next to `guide.md`, the
same relative image paths (`media/overview.png`) work unchanged. Translate only the prose — keep
headings structure, image paths, links and code exactly as in the English guide. A
`guide.<lang>.md` with no English `guide`, or with an unsupported code (e.g. `guide.jp.md`), fails CI.

**Manifest `i18n`** — keyed by language; every key inside is optional:

```json
"i18n": {
  "ja": {
    "name": "フォトスタジオ",
    "description": "…",
    "captions": ["…", null, "…"],
    "changelog": { "added": ["…"], "fixed": ["…"] }
  },
  "ko": { "description": "…" }
}
```

| Key | Rules | Published as |
|---|---|---|
| `name` | non-empty string, ≤ 100 chars. Only translate a plugin name if the plugin itself shows a translated name in-game; otherwise leave it out | `i18n.<lang>.name` on the registry entry |
| `description` | non-empty string, ≤ 1000 chars | `i18n.<lang>.description` |
| `captions` | list, one entry per `media` item **by index** (≤ the number of media items); `null` = not translated; a translated caption needs an English `caption` on that media item | `i18n.<lang>.captions` |
| `changelog` | same shape as this version's English `changelog`, using only its section names; a section may be `[]` only when the English one is empty too; strings ≤ 2000 chars. Requires an English `changelog` | `changelogI18n.<lang>` on **this version's** `versions[]` entry |

Name/description/captions describe the plugin (shared by both channels); the `changelog` belongs to
the one version in that manifest. Update the translated changelog together with the English one on
every release — a stale translation is shown as-is.

#### Plugin dependencies (`dependencies`)

A plugin build can declare things the **launcher** must download, verify, and place before the
game starts — a native DLL the plugin needs beside it, or a shared asset the **game itself** needs
(e.g. an external runtime the game loads directly). The launcher is **generic**: it installs
whatever passes registry validation without knowing what any particular dependency is for.

`dependencies` is an array; each entry is an object:

| Field | Type | Required | Notes |
|---|---|---|---|
| `id` | string | ✓ | unique **within this plugin's** dependency list; must match `[A-Za-z0-9._-]+` (letters, digits, `.`, `_`, `-` only — no `/`, no whitespace, and not bare `.` or `..`), the same charset the launcher's `DependencyPaths.IsValidPluginId` enforces on a plugin's own id |
| `name` | string | ✓ | display name shown to the user during install |
| `version` | string | ✓ | this dependency's own version (not the plugin's) |
| `url` | string | ✓ | **https-only** download URL |
| `sha256` | string | ✓ | 64 hex characters — verified after download, before install |
| `size` | integer | ✓ | expected byte size, `1`..512 MiB |
| `kind` | `"file"` \| `"zip"` | ✓ | `"file"` installs a single file; `"zip"` extracts entries out of a downloaded archive |
| `files` | array | ✓ | destination(s) — see below; `kind: "file"` takes **exactly one** entry |
| `target` | `"plugin"` \| `"game"` | ✓ | `"game"` installs relative to the game install root; `"plugin"` installs under `game_mini/stellar/deps/<pluginId>/<to>` — **not** the plugin's own install folder |
| `moddedOnly` | boolean | — | default `false`; **requires `target: "game"`** — installed while Stellar is present, then **parked** (moved aside) for a Vanilla launch and while the plugin is disabled, and **restored** when the player launches Modded again with the plugin enabled (never left in place for a vanilla client) |
| `optional` | boolean | — | default `false`; the user may decline it and the plugin still installs |
| `requires` | string[] | — | other `id`s (from this same list) that must be installed **first**; each referenced id must exist and be **listed earlier** in the array |
| `license` | string | ✓ | the dependency's license (e.g. `"BSD-3-Clause"`) — shown to the user before install |
| `licenseUrl` | string | ✓ | link to the full license text |
| `sourceUrl` | string | ✓ | link to the dependency's own source/homepage |
| `notice` | string | — | short free-text notice shown alongside the license (e.g. attribution) |
| `description` | string | — | one or two sentences saying what the dependency adds; the launcher shows it in its install step |

Each `files` entry is `{ "to": "<relative path>", "from"?: "<entry path or prefix>" }`:

- **`to`** is always required — a path under either `stellar/deps/<pluginId>/` or the game root
  (per `target`), matching the launcher's own `DependencyPaths.Resolve` rules exactly:
  - **relative only** — no leading `/` (not rooted), and no `\` anywhere;
  - **no `:` anywhere** — not just a drive letter (`C:/x`); this also refuses an NTFS
    alternate-data-stream suffix like `dxgi.dll:ads`;
  - **no empty, `.`, or `..` path segment** — `a//b`, `./a`, `a/./b` and `a/../b` are all refused,
    not just a literal `..`;
  - **no trailing `/`**, with ONE exception: a `kind: "zip"` entry whose `from` is a directory
    prefix (itself ending in `/`) may have a `to` that also ends in `/`, naming the destination
    folder the whole subtree is extracted into (see the `from`/`to` pair below). For `kind: "file"`
    — or a `kind: "zip"` entry whose `from` names one archive entry rather than a prefix — `to` is
    the literal destination and a trailing `/` is refused.
  - **`target: "game"` destinations may never land inside a path the loaders themselves scan** —
    `BepInEx/`, `stellar/plugins`, or `stellar/deps` (case-insensitive) are refused, so a dependency
    can never masquerade as (or collide with) a scanned plugin.

  Two dependencies of the **same plugin** may also never claim the same `(target, to)` destination
  (case-insensitive) — whichever comes second in the array fails validation. Two **different**
  plugins may also never both claim the same `target: "game"` destination — whichever PR adds the
  clash fails CI.
- **`from`** is required only for `kind: "zip"`: either one entry's path inside the archive, or a
  prefix ending in `/` to extract a whole subtree. Unused (and ignored) for `kind: "file"`.

Example — an example runtime DLL, installed into the game root only while Stellar is present, which
the user may decline:

```json
"dependencies": [
  {
    "id": "examplert", "name": "Example Runtime", "version": "1.2.3",
    "url": "https://example.com/deps/examplert-1.2.3.dll",
    "sha256": "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
    "size": 4312576, "kind": "file", "target": "game",
    "moddedOnly": true, "optional": true,
    "files": [ { "to": "examplert.dll" } ],
    "license": "BSD-3-Clause", "licenseUrl": "https://example.com/license",
    "sourceUrl": "https://example.com"
  }
]
```

### `plugins/<id>/manifest.testing.json` — the optional testing override

A second, **testing-channel** build that runs alongside the stable `manifest.json`. It **inherits** the
shared fields and may set **only** the version-specific ones below — any other key is **rejected**.

| Field | Type | Required | Notes |
|---|---|---|---|
| `version` | string (semver) | ✓ | the testing build's version, e.g. `1.2.0-beta` |
| `commit` | string (40-hex) | ✓ | the testing build's pinned commit (authoritative) |
| `minModSystemVersion` | string (semver) | ✓ | framework floor for this build |
| `tag` | string | — | display-only; CI verifies `tag` → `commit` |
| `date` | string `YYYY-MM-DD` | — | release date |
| `maxModSystemVersion` | string \| null | — | upper bound |
| `capPriorVersionsAt` | string (semver) | — | retro-cap prior published versions |
| `changelog` | object | — | as above |
| `dependencies` | array | — | this testing build's own dependencies (see above) — may differ from the stable manifest's |
| `i18n` | object | — | **changelog only**: `{ "<lang>": { "changelog": {…} } }` for this testing build. The stable manifest's translated name/description/captions are inherited; its translated changelog is **not** (it belongs to the stable version) |
| **inherited — do _not_ repeat** | | | `id`, `name`, `description`, `author`, `dll`, `repository`, `projectPath`, `tags`, `homepage`, `media`, `guide`, `icon`, `i18n` (name/description/captions) come from `manifest.json`; translated guides are the same files for both channels |

## Third-party / unverified plugins

Don't want to open-source into the curated registry? Distribute from **your own repo** and have users
add its `plugins.json` URL in the launcher — it's surfaced as **third-party / install-at-your-own-risk**
and never mixed into the curated default list.

## Writing the plugin code

A plugin is a single class implementing `IStellarPlugin`, constructed with `IPluginServices`. References:

- [**API reference**](https://github.com/StellarProtocol/StellarResonanceModSystem/tree/main/docs/api) —
  every public type in `Stellar.Abstractions` (the contract you build against).
- [**Developer guide**](https://github.com/StellarProtocol/StellarResonanceModSystem/blob/main/docs/plugin-development.md) —
  services, lifecycle, the declarative uGUI toolkit, IL2CPP quirks.

## License

By contributing you agree your contribution is licensed under **AGPL-3.0-or-later** (matching the SDK
your plugin references — curated plugins are therefore open source).
