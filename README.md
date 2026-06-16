<p align="center">
  <img src=".github/logo.png" alt="StellarResonance" width="160">
</p>

# StellarResonance Plugins

The **plugin registry and reference plugin source** for the
[StellarResonance Launcher](https://github.com/StellarProtocol/StellarResonance) and
[`StellarResonanceModSystem`](https://github.com/StellarProtocol/StellarResonanceModSystem)
framework — the curated plugin catalog, in the spirit of Dalamud's plugin repo.

This repo owns the registry; its CI publishes the index + DLLs to the public MinIO
bucket the launcher reads (`https://minio.revette.io/stellar/plugins.json`). It is
**decoupled from the framework's releases** — plugins ship on their own cadence.

> **Not affiliated with, endorsed by, or connected to** the game's publisher or developer.
> Plugins are read-only, quality-of-life only; they ship no game code or assets.

## Layout

```
plugins/
  <id>/
    manifest.json     # id, name, description, version, dll, author, minModSystemVersion
    <Plugin>.dll      # the built plugin assembly
samples/
  Stellar.<Name>/     # reference plugin SOURCE (build against the framework — see below)
tools/build-registry.py        # validates manifests, computes sha256, builds + publishes plugins.json
.github/workflows/publish.yml  # PR = validate; push to main = validate + publish to MinIO
```

`plugins.json` (generated) is the launcher's curated registry. Each entry carries a **version
history** so the launcher can offer a version picker, and each version declares the framework
(modsystem) range it runs on:

```jsonc
{ "id", "name", "description", "author",
  "versions": [
    { "version", "date", "dllUrl", "sha256",
      "minModSystemVersion", "maxModSystemVersion" /* null = no upper bound */, "changelog" }
  ] }
```

Each release is appended to the published history (older versions keep working — DLLs are stored
under version-specific keys `plugins/<id>/<Name>-<version>.dll`). The full contract is in the
[manifest standard](https://github.com/StellarProtocol/StellarResonance-DevKit/blob/main/docs/manifest-standard.md).

## Reference plugin source (`samples/`)

The `samples/` directory holds the source for the bundled reference plugins (PlayerHUD,
CombatMeter, ChatTools, …). They reference **only** `Stellar.Abstractions` from the framework.
Building them requires a local checkout of
[`StellarResonanceModSystem`](https://github.com/StellarProtocol/StellarResonanceModSystem)
and the game's IL2CPP interop assemblies (generated from your own install):

```bash
dotnet build samples/Stellar.PlayerHUD/Stellar.PlayerHUD.csproj -c Release \
  -p:StellarFrameworkSrc=/path/to/StellarResonanceModSystem/src \
  -p:BepInExCore=/path/to/<game_mini>/BepInEx/core \
  -p:GameInterop=/path/to/<game_mini>/BepInEx/interop
```

See [`samples/Directory.Build.props`](samples/Directory.Build.props) for the overridable paths.

The full plugin-facing contract is documented in the framework's
[**API reference**](https://github.com/StellarProtocol/StellarResonanceModSystem/tree/main/docs/api)
(every public interface/type) and the
[**developer guide**](https://github.com/StellarProtocol/StellarResonanceModSystem/blob/main/docs/plugin-development.md).

## How it works

```
  plugins/<id>/manifest.json + <Plugin>.dll
            │
            ▼   tools/build-registry.py   (validate → sha256 → assemble)
        dist/plugins.json
            │
            ▼   CI (push to main, Production env)   upload to MinIO
   minio.revette.io/stellar/plugins.json  +  /stellar/plugins/<id>/<Plugin>-<version>.dll
            │
            ▼
   the StellarResonance Launcher reads plugins.json and offers each plugin
```

- **PR** → CI runs `build-registry.py` (validation only — no credentials).
- **Push/merge to `main`** → CI runs `build-registry.py --publish`, uploading every DLL +
  `plugins.json` to the `stellar` bucket (uses the `S3_ACCESS_KEY`/`S3_SECRET_KEY` secrets in the
  `Production` environment).

## Adding a plugin

You add a plugin by contributing a **manifest + its built DLL** under `plugins/<id>/`.

1. **Build your plugin** into a DLL. Either develop it under [`samples/`](#reference-plugin-source-samples)
   (against the framework), or build it in your own project — it only needs to reference
   `Stellar.Abstractions` and implement `IStellarPlugin`.
2. **Create `plugins/<your-id>/`** and drop in your built `<Plugin>.dll`.
3. **Add `plugins/<your-id>/manifest.json`** with all required fields:

   ```json
   {
     "id": "yourplugin",
     "name": "Your Plugin",
     "description": "One-line summary shown in the launcher.",
     "version": "1.0.0",
     "dll": "YourPlugin.dll",
     "author": "your-handle",
     "minModSystemVersion": "1.0.0",
     "maxModSystemVersion": null,
     "date": "2026-06-16",
     "changelog": { "added": ["…"], "changed": [], "fixed": [], "removed": [] }
   }
   ```

   | Field | Notes |
   |---|---|
   | `id` | lowercase, filesystem-safe; must match the folder name |
   | `name` / `description` | shown in the launcher |
   | `version` | semver; bump it on every update |
   | `dll` | exact filename of the DLL beside this manifest |
   | `author` | your name/handle |
   | `minModSystemVersion` | **required** — lowest framework version this build runs on |
   | `maxModSystemVersion` | optional (omit/`null` = no upper bound); set when a newer framework breaks this build |
   | `date` | optional `YYYY-MM-DD` |
   | `changelog` | optional `{ added, changed, fixed, removed }` — shown when reviewing the version |

   The manifest describes the **current** version; the builder appends it to the published history,
   so previously released versions remain available in the launcher's picker.

4. **Validate locally** before opening a PR:

   ```bash
   python tools/build-registry.py        # validates all manifests + DLLs, writes dist/plugins.json
   ```

5. **Open a PR.** CI re-validates. On merge to `main`, CI publishes your DLL + the updated
   `plugins.json` to MinIO, and the launcher picks it up automatically.

> **Self-hosting:** the launcher can also add **third-party registry URLs** directly (any
> `plugins.json`), so you can host your own registry instead of submitting here.

See [`CONTRIBUTING.md`](CONTRIBUTING.md) for the full contributor guide.

## License

[GNU Affero General Public License v3.0](LICENSE) (AGPL-3.0).
