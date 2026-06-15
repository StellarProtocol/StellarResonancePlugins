# Contributing

Thanks for helping grow the StellarResonance plugin ecosystem. This repo is the curated
**plugin registry** (and reference plugin **source**) for the
[StellarResonance](https://github.com/StellarProtocol/StellarResonanceModSystem) framework.

## What's allowed

Plugins must be **quality-of-life only**, matching the framework's policy:

- ✅ UI overlays, HUDs, chat tooling, log viewers, accessibility helpers, inspectors.
- ❌ No packet construction/modification, no memory read/write, no automation that creates unfair
  advantage, no anti-cheat evasion.

The framework's abstraction surface (`Stellar.Abstractions`) is read-only by design — a plugin
*physically cannot* reach lower-level capabilities. PRs adding cheat-shaped behavior are rejected.

## Two ways to contribute

### 1. Add a plugin to the registry

This is the common path — publish a plugin so the launcher offers it. You contribute a manifest +
your built DLL; you do **not** need your source in this repo.

1. Build your plugin into `<Plugin>.dll` (see "Developing a plugin" below, or use your own project).
2. Create `plugins/<your-id>/` and add your `<Plugin>.dll`.
3. Add `plugins/<your-id>/manifest.json` (see the README for the schema + field rules).
4. Validate: `python tools/build-registry.py` (must succeed — it checks required fields, a
   filesystem-safe `id`/`dll`, and that the DLL exists).
5. Open a PR. CI re-validates; on merge to `main` it publishes to MinIO and the launcher picks it up.

### 2. Develop a reference plugin in `samples/`

The `samples/` directory holds buildable reference plugins. To add or modify one:

1. Add `samples/Stellar.<Name>/` with a `.csproj` and your source. Mirror an existing sample —
   reference **only** `$(StellarFrameworkSrc)/Stellar.Abstractions/Stellar.Abstractions.csproj`.
2. Build it (requires a local framework checkout + the game's IL2CPP interop):

   ```bash
   dotnet build samples/Stellar.<Name>/Stellar.<Name>.csproj -c Release \
     -p:StellarFrameworkSrc=/path/to/StellarResonanceModSystem/src \
     -p:BepInExCore=/path/to/<game_mini>/BepInEx/core \
     -p:GameInterop=/path/to/<game_mini>/BepInEx/interop
   ```

3. To also publish it, follow path #1 with the built DLL.

The registry CI does **not** build `samples/` — it validates and publishes prebuilt DLLs. Sample
source is reference material and built locally.

## Writing the plugin code

A plugin is a single class implementing `IStellarPlugin`, constructed with `IPluginServices`.
Two references in the framework repo:

- [**API reference**](https://github.com/StellarProtocol/StellarResonanceModSystem/tree/main/docs/api) —
  every public interface, record, and enum in `Stellar.Abstractions` (the complete contract you build against).
- [**Developer guide**](https://github.com/StellarProtocol/StellarResonanceModSystem/blob/main/docs/plugin-development.md) —
  narrative walkthrough: services, lifecycle, the declarative uGUI toolkit, IL2CPP quirks.

## License

By contributing, you agree your contribution is licensed under **AGPL-3.0** (this repo's license).
