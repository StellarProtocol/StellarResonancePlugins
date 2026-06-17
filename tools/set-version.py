#!/usr/bin/env python3
"""Set a plugin's release version + framework-compatibility in its manifest.

Releasing a plugin build that targets a newer framework is a TWO-sided version
change; this scripts both sides so it's a repeatable step, not hand-edited JSON:

  - NEW build  -> version + minModSystemVersion (the framework floor it runs on).
  - OLD builds -> capPriorVersionsAt: the last framework the already-published
    versions run on. build-registry.py stamps it as their maxModSystemVersion
    (when still null), so the launcher stops offering them on the newer
    framework. See docs/manifest-standard.md and tools/build-registry.py.

The registry build/publish (CI: .github/workflows/publish.yml) then applies the
cap and ships the result — no manual plugins.json editing.

Usage:
  set-version.py <plugin-id> --version 1.1.0 --min 1.1.0 [--cap-prior 1.0.1] [--date YYYY-MM-DD]
  set-version.py --all       --version 1.1.0 --min 1.1.0 [--cap-prior 1.0.1] [--date YYYY-MM-DD]
"""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PLUGINS_DIR = ROOT / "plugins"


def manifests(plugin: str | None, every: bool) -> list[Path]:
    if every:
        return sorted(PLUGINS_DIR.glob("*/manifest.json"))
    if not plugin:
        sys.exit("specify a plugin id or --all")
    mp = PLUGINS_DIR / plugin / "manifest.json"
    if not mp.is_file():
        sys.exit(f"no manifest for plugin id '{plugin}' at {mp}")
    return [mp]


def main() -> None:
    ap = argparse.ArgumentParser(description="Set plugin release version + framework compat.")
    ap.add_argument("plugin", nargs="?", help="plugin id (omit when using --all)")
    ap.add_argument("--all", action="store_true", help="apply to every plugin")
    ap.add_argument("--version", required=True, help="new build version, e.g. 1.1.0")
    ap.add_argument("--min", required=True, help="minModSystemVersion (framework floor)")
    ap.add_argument("--cap-prior", help="cap already-published versions at this framework version")
    ap.add_argument("--date", help="release date YYYY-MM-DD (UTC); omit to leave unchanged")
    args = ap.parse_args()

    for mp in manifests(args.plugin, args.all):
        m = json.loads(mp.read_text(encoding="utf-8"))
        m["version"] = args.version
        m["minModSystemVersion"] = args.min
        if args.cap_prior:
            m["capPriorVersionsAt"] = args.cap_prior
        else:
            m.pop("capPriorVersionsAt", None)
        if args.date:
            m["date"] = args.date
        mp.write_text(json.dumps(m, indent=2) + "\n", encoding="utf-8")
        print(f"{mp.parent.name}: v{m['version']} min={m['minModSystemVersion']} "
              f"cap={m.get('capPriorVersionsAt')}")


if __name__ == "__main__":
    main()
