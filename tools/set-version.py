#!/usr/bin/env python3
"""Set a plugin's release version + framework-compatibility in its manifest.

Releasing a plugin build that targets a newer framework is a TWO-sided version
change; this scripts both sides so it's a repeatable step, not hand-edited JSON:

  - NEW build  -> version + minModSystemVersion (the framework floor it runs on)
                  + the pinned commit (authoritative) and optional tag (provenance).
  - OLD builds -> capPriorVersionsAt: the last framework the already-published
    versions run on. build-registry.py stamps it as their maxModSystemVersion
    (when still null), so the launcher stops offering them on the newer
    framework. See docs/manifest-standard.md and tools/build-registry.py.

Channels (two-file source model — see build-registry.py / docs/manifest-standard.md):
  - default       edit plugins/<id>/manifest.json (the canonical record / stable build).
  - --testing     edit plugins/<id>/manifest.testing.json (a testing-channel build that
                  inherits shared fields from manifest.json). A plugin can be live on
                  stable AND testing at once. --testing requires --commit.
  - --promote     fold manifest.testing.json into manifest.json (the testing build becomes
                  the new stable) and delete the testing override.

The registry build/publish (CI: .github/workflows/publish.yml) then applies the
cap and ships the result — no manual plugins.json editing.

Usage:
  set-version.py <id>  --version 1.1.0 --min 1.1.0 [--commit SHA] [--tag v1.1.0]
                       [--cap-prior 1.0.1] [--date YYYY-MM-DD]
  set-version.py --all --version 1.1.0 --min 1.1.0 [--cap-prior 1.0.1] [--date YYYY-MM-DD]
  set-version.py <id> --testing --version 1.2.0-beta --min 1.1.0 --commit SHA [--tag v1.2.0-beta] [--date …]
  set-version.py <id> --promote
"""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PLUGINS_DIR = ROOT / "plugins"

# Fields a manifest.testing.json override owns (mirrors build-registry.py OVERRIDABLE).
OVERRIDABLE = ("version", "date", "commit", "tag", "minModSystemVersion",
               "maxModSystemVersion", "capPriorVersionsAt", "changelog")


def stable_path(plugin: str) -> Path:
    mp = PLUGINS_DIR / plugin / "manifest.json"
    if not mp.is_file():
        sys.exit(f"no manifest for plugin id '{plugin}' at {mp}")
    return mp


def apply_fields(m: dict, args: argparse.Namespace) -> None:
    m["version"] = args.version
    m["minModSystemVersion"] = args.min
    if args.commit:
        m["commit"] = args.commit
    if args.tag:
        m["tag"] = args.tag
    if args.cap_prior:
        m["capPriorVersionsAt"] = args.cap_prior
    elif not args.testing:
        # On the stable manifest, an unset --cap-prior clears any stale cap. The testing
        # override only ever holds keys it explicitly sets, so don't synthesize a removal there.
        m.pop("capPriorVersionsAt", None)
    if args.date:
        m["date"] = args.date


def write(path: Path, m: dict) -> None:
    path.write_text(json.dumps(m, indent=2) + "\n", encoding="utf-8")
    print(f"{path.relative_to(ROOT)}: v{m['version']} min={m.get('minModSystemVersion')} "
          f"commit={m.get('commit', '-')[:12]} cap={m.get('capPriorVersionsAt')}")


def do_promote(plugin: str) -> None:
    """The testing build becomes stable: copy its overridable fields onto manifest.json,
    drop manifest.json's testing-only `channel` marker, and delete the override file."""
    mp = stable_path(plugin)
    tp = PLUGINS_DIR / plugin / "manifest.testing.json"
    if not tp.is_file():
        sys.exit(f"no manifest.testing.json to promote for '{plugin}'")
    m = json.loads(mp.read_text(encoding="utf-8"))
    t = json.loads(tp.read_text(encoding="utf-8"))
    for k in OVERRIDABLE:
        if k in t:
            m[k] = t[k]
    m.pop("channel", None)            # promoted build is the stable one now
    mp.write_text(json.dumps(m, indent=2) + "\n", encoding="utf-8")
    tp.unlink()
    print(f"promoted {plugin}: testing v{m['version']} -> stable (manifest.testing.json removed)")


def do_set(args: argparse.Namespace) -> None:
    if args.testing:
        if args.all:
            sys.exit("--testing is per-plugin; can't combine with --all")
        if not args.commit:
            sys.exit("--testing requires --commit (a testing build must pin a real commit)")
        stable_path(args.plugin)      # ensure the canonical manifest exists to inherit from
        tp = PLUGINS_DIR / args.plugin / "manifest.testing.json"
        m = json.loads(tp.read_text(encoding="utf-8")) if tp.is_file() else {}
        apply_fields(m, args)
        m = {k: m[k] for k in OVERRIDABLE if k in m}   # keep only overridable keys
        write(tp, m)
        return

    if not args.all and not args.plugin:
        sys.exit("specify a plugin id or --all")
    targets = (sorted(PLUGINS_DIR.glob("*/manifest.json")) if args.all
               else [stable_path(args.plugin)])
    for mp in targets:
        m = json.loads(mp.read_text(encoding="utf-8"))
        apply_fields(m, args)
        write(mp, m)


def main() -> None:
    ap = argparse.ArgumentParser(description="Set plugin release version + framework compat.")
    ap.add_argument("plugin", nargs="?", help="plugin id (omit when using --all)")
    ap.add_argument("--all", action="store_true", help="apply to every plugin (stable manifests)")
    ap.add_argument("--testing", action="store_true", help="edit manifest.testing.json (testing channel)")
    ap.add_argument("--promote", action="store_true", help="promote the testing build to stable")
    ap.add_argument("--version", help="new build version, e.g. 1.1.0")
    ap.add_argument("--min", help="minModSystemVersion (framework floor)")
    ap.add_argument("--commit", help="pinned source commit (authoritative; required for --testing)")
    ap.add_argument("--tag", help="source tag (display-only provenance; CI verifies tag->commit)")
    ap.add_argument("--cap-prior", help="cap already-published versions at this framework version")
    ap.add_argument("--date", help="release date YYYY-MM-DD (UTC); omit to leave unchanged")
    args = ap.parse_args()

    if args.promote:
        if not args.plugin:
            sys.exit("--promote needs a plugin id")
        do_promote(args.plugin)
        return

    if not args.version or not args.min:
        sys.exit("--version and --min are required (unless --promote)")
    do_set(args)


if __name__ == "__main__":
    main()
