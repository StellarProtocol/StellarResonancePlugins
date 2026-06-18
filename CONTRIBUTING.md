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
     "commit": "<full 40-char sha>", "projectPath": ".",
     "version": "1.0.0", "minModSystemVersion": "1.1.0", "channel": "testing"
   }
   ```
   `tools/set-version.py <id> --version … --min … [--cap-prior …]` helps with version/compat fields.
3. **Open a PR.** CI **clones your repo at the pinned commit and builds it in an isolated container**,
   then validates the registry. Your pinned source + the manifest diff are the review surface.
4. On merge to `main`, `publish.yml` rebuilds from the pinned commit and — after the `Production`
   approval — publishes the registry + your DLL to MinIO, recording `sourceRepository`/`sourceCommit`
   (provenance). The launcher picks it up.

To **update**, bump `commit` (and `version`) in the manifest via a new PR.

## Channels

- **`"stable"`** (default) → in **both** `plugins.json` and `plugins-testing.json`.
- **`"testing"`** → in **only** `plugins-testing.json` (the launcher's *testing* channel).

**New plugins and risky updates start on `testing`**; promote to stable by setting `channel` to
`stable` (or removing it) once proven. The launcher fetches the file for the user's selected channel.

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
