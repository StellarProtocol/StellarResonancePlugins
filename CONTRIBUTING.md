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

- ✅ UI overlays, HUDs, chat tooling, log viewers, accessibility helpers, inspectors.
- ❌ No packet construction/modification, no memory read/write, no automation that creates unfair
  advantage, no anti-cheat evasion.

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
