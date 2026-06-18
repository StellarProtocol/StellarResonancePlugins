<p align="center">
  <img src=".github/logo.png" alt="StellarResonance" width="160">
</p>

# StellarResonance Plugins

The **curated plugin registry** for the
[StellarResonance Launcher](https://github.com/StellarProtocol/StellarResonance) and the
[`StellarResonanceModSystem`](https://github.com/StellarProtocol/StellarResonanceModSystem) framework.

This repo holds **manifests only** — each plugin lives in **its own public repo**, and CI builds it from
a pinned commit and publishes the index + DLLs to the public registry the launcher reads
(`https://minio.revette.io/stellar/plugins.json`). The model is **inspired by Dalamud's
[DIP17](https://github.com/goatcorp/DIPs/blob/main/text/17-automated-build-and-submit-pipeline.md)**,
customised for StellarResonance (our own SDK on NuGet.org, sandboxed container builds, MinIO registry).

> **Not affiliated with, endorsed by, or connected to** the game's publisher or developer.
> Plugins are quality-of-life only; they ship no game code or assets.

---

## Write a plugin (quickstart)

You need **no framework checkout and no game install** — just the SDK from
[NuGet.org](https://www.nuget.org/profiles/dorasu).

1. **Create your plugin in its own public repo**, named **`Stellar<Name>Plugin`**
   (e.g. `StellarMyOverlayPlugin`; the assembly/DLL is `Stellar.MyOverlay.dll`).
2. **`Stellar.MyOverlay.csproj`** — reference the SDK packages:
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <TargetFramework>net6.0</TargetFramework>
       <AssemblyName>Stellar.MyOverlay</AssemblyName>
       <Version>1.0.0</Version>          <!-- fixed version: same commit ⇒ same binary -->
       <Nullable>enable</Nullable><ImplicitUsings>disable</ImplicitUsings><LangVersion>latest</LangVersion>
     </PropertyGroup>
     <ItemGroup>
       <PackageReference Include="Stellar.Abstractions" Version="1.1.1" />
       <PackageReference Include="Stellar.Plugin.InteropRefs" Version="1.1.1" />
       <!-- + Stellar.PluginContracts if you use the inter-plugin exchange -->
     </ItemGroup>
   </Project>
   ```
3. **Implement `IStellarPlugin`** — a single class constructed with `IPluginServices`:
   ```csharp
   using Stellar.Abstractions.Services;
   public sealed class MyOverlay : IStellarPlugin {
       public string Name => "My Overlay";
       public MyOverlay(IPluginServices services) { /* draw HUDs, read state, … */ }
       public void Dispose() { }
   }
   ```
4. **Build** — `dotnet build -c Release`. That's it; no game needed to compile.
   (To test *in-game*, point `GameInterop`/`BepInExCore` at a real `<game_mini>/BepInEx` install.)

### The SDK packages (on NuGet.org)

All three are published to **[NuGet.org](https://www.nuget.org/profiles/dorasu)** (no feed config needed):

| Package | What it's for |
|---|---|
| **[`Stellar.Abstractions`](https://www.nuget.org/packages/Stellar.Abstractions)** | the plugin API — services, the declarative uGUI element tree, domain types |
| **[`Stellar.Plugin.InteropRefs`](https://www.nuget.org/packages/Stellar.Plugin.InteropRefs)** | compile-time Unity/Il2Cpp/BepInEx **reference stubs** so you build without the game (the game provides the real ones at runtime) |
| **[`Stellar.PluginContracts`](https://www.nuget.org/packages/Stellar.PluginContracts)** | shared contracts for the inter-plugin exchange (`IPluginExchange`) — only if you use it |

Install with the CLI (or use the `<PackageReference>` block above):

```sh
dotnet add package Stellar.Abstractions
dotnet add package Stellar.Plugin.InteropRefs
dotnet add package Stellar.PluginContracts   # only if you use the inter-plugin exchange
```

> Pin a **fixed** version that matches your target framework release (e.g. `--version 1.1.1`) so a
> given commit always builds the same binary — the registry rebuilds your plugin from its pinned commit.

Versions track the framework release. Full API: the framework's
[**API reference**](https://github.com/StellarProtocol/StellarResonanceModSystem/tree/main/docs/api) +
[**developer guide**](https://github.com/StellarProtocol/StellarResonanceModSystem/blob/main/docs/plugin-development.md).

---

## Publish it to the registry

Add **one manifest** here pinning your repo + commit — CI clones that commit, **builds it in an isolated
container**, and publishes it (after review + a maintainer approval).

`plugins/<your-id>/manifest.json`:
```jsonc
{
  "id": "myoverlay", "name": "My Overlay",
  "description": "One-line summary shown in the launcher.", "author": "your-handle",
  "dll": "Stellar.MyOverlay.dll",
  "repository": "https://github.com/you/StellarMyOverlayPlugin.git",
  "commit": "<full 40-char sha>",       // immutable — CI builds + attests THIS exact commit
  "tag": "v1.0.0",                       // optional, display-only; CI verifies tag → commit
  "projectPath": ".",
  "version": "1.0.0",
  "minModSystemVersion": "1.1.0",        // lowest framework version this build runs on
  "channel": "testing",                  // new plugins start on testing (see Channels)
  "changelog": { "added": ["…"] }
}
```
`commit` is **authoritative** (CI builds that exact SHA); `tag` is optional, display-only provenance.
`tools/set-version.py <id> --version … --min … [--commit … --tag … --cap-prior …]` helps with the
version/compat fields.
Then **open a PR** — CI sandbox-builds your pinned commit + validates the registry (the PR diff + your
pinned source are the review surface). On merge to `main`, the build is rebuilt and published to MinIO
with provenance (`sourceRepository`/`sourceCommit`), after a maintainer's `Production` approval.

To **update**: bump `commit` (and `version`) via a new PR.

### Channels
The build emits `plugins.json` (stable only) and `plugins-testing.json` (a **superset** — every
version). A plugin can be live on **both** at once via a two-file source model:
- `manifest.json` — the canonical record; its `"channel"` (default `"stable"`) is the channel of *that*
  version. New/risky builds set `"testing"`; promote with `set-version.py <id> --promote`.
- `manifest.testing.json` — *optional* sibling adding a **second, testing build** that inherits the
  shared fields and overrides only the version-specific ones (`version`/`commit`/`tag`/`min`/…), so a
  beta runs alongside the stable release. Create it with `set-version.py <id> --testing …`.

### Security model — important
A plugin DLL is **arbitrary code in the game process** (BepInEx IL2CPP); it is **not sandboxed at
runtime**. `Stellar.Abstractions` is a read-only API *shape*, not a security boundary. Trust comes from
**reviewable source + a build we control** — that's why the curated registry only builds from a **pinned
commit in a public repo**, never an uploaded binary. Cheat-shaped / hostile PRs are rejected. Don't want
to open-source? Host your own `plugins.json` and have users add its URL (surfaced as **unverified**).

See [`CONTRIBUTING.md`](CONTRIBUTING.md) for the full guide.

---

## How the registry works

```
your repo @ commit  ──(PR pins it in plugins/<id>/manifest.json)──►  this registry (manifests only)
       │
       ▼  publish.yml: clone @commit ─► build in isolated dotnet/sdk container (no secrets) ─► stage DLL
   tools/build-registry.py  (sha256 + provenance + merge history)
       │
       ▼  Production-gated publish  ─►  minio.revette.io/stellar/{plugins.json, plugins-testing.json}
                                        + /stellar/plugins/<id>/<Name>-<version>.dll
       ▼
   the StellarResonance Launcher reads the channel's file and gates each plugin by framework version
```

- **PR** → CI builds from the pinned commit + validates (no credentials).
- **Merge to `main`** → CI rebuilds + publishes (gated on the `Production` environment).

Layout: `plugins/<id>/manifest.json` (registry), `tools/` (`build-registry.py`, `set-version.py`),
`samples/` (a few in-repo reference/dev plugins). Full contract:
[manifest standard](https://github.com/StellarProtocol/StellarResonance-DevKit/blob/main/docs/manifest-standard.md).

## License

[GNU Affero General Public License v3.0](LICENSE) (AGPL-3.0-or-later). Plugins reference the AGPL SDK, so
curated plugins are open source too.
