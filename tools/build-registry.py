#!/usr/bin/env python3
"""Build + publish the Stellar plugin registry to MinIO.

Scans plugins/<id>/manifest.json (+ the DLL beside each), computes sha256, assembles
plugins.json, and uploads every DLL + plugins.json to the public `stellar` MinIO bucket.
This repo OWNS the registry — decoupled from the framework's releases. Community plugins
are added by PR (a new plugins/<id>/).

Each plugin carries a versions[] history and declares the framework (modsystem) range it
runs on (minModSystemVersion / optional maxModSystemVersion), so the launcher can gate
versions against the installed framework. DLLs are uploaded under version-specific keys
(plugins/<id>/<name>-<version>.dll), and the new release is APPENDED to the published
history — old versions stay downloadable so users can roll back. See docs/manifest-standard.md.

Usage:
  build-registry.py            # validate + build dist/plugins.json (merges published history)
  build-registry.py --publish  # also upload DLLs + plugins.json to MinIO (needs S3 creds)
"""
from __future__ import annotations

import hashlib
import json
import os
import subprocess
import sys
import urllib.request
from pathlib import Path

ENDPOINT = "https://minio.revette.io"
BUCKET = "stellar"
ROOT = Path(__file__).resolve().parents[1]
PLUGINS_DIR = ROOT / "plugins"
REQUIRED = ("id", "name", "description", "version", "dll", "author", "minModSystemVersion")


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as fh:
        for chunk in iter(lambda: fh.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def ver_key(v: str) -> tuple:
    return tuple(int(p) if p.isdigit() else 0 for p in str(v).lstrip("vV").split("."))


def aws_cp(src: str, key: str) -> None:
    env = {**os.environ,
           "AWS_DEFAULT_REGION": "sa-east-1",
           "AWS_REQUEST_CHECKSUM_CALCULATION": "when_required",
           "AWS_RESPONSE_CHECKSUM_VALIDATION": "when_required"}
    subprocess.run(["aws", "s3", "cp", src, f"s3://{BUCKET}/{key}",
                    "--endpoint-url", ENDPOINT], check=True, env=env)


def fetch_published() -> dict:
    """Current published plugins.json (read-only, public). Best-effort — {} if absent."""
    try:
        with urllib.request.urlopen(f"{ENDPOINT}/{BUCKET}/plugins.json", timeout=10) as r:
            return json.loads(r.read().decode("utf-8"))
    except Exception:
        return {}


def collect() -> list[dict]:
    """One record per plugin: the current version's entry + the DLL to upload."""
    plugins = []
    for manifest_path in sorted(PLUGINS_DIR.glob("*/manifest.json")):
        m = json.loads(manifest_path.read_text(encoding="utf-8"))
        missing = [k for k in REQUIRED if not m.get(k)]
        if missing:
            sys.exit(f"{manifest_path}: missing fields {missing}")
        if "/" in m["id"] or ".." in m["id"] or "/" in m["dll"] or ".." in m["dll"]:
            sys.exit(f"{manifest_path}: unsafe id/dll")
        dll = manifest_path.parent / m["dll"]
        if not dll.is_file():
            sys.exit(f"{manifest_path}: dll not found: {dll}")

        # Version-specific object key so older builds stay downloadable.
        stem, suffix = Path(m["dll"]).stem, Path(m["dll"]).suffix
        key = f"plugins/{m['id']}/{stem}-{m['version']}{suffix}"

        version_entry = {
            "version": m["version"],
            "date": m.get("date", ""),
            "dll": m["dll"],                 # canonical on-disk filename (the assembly name, e.g. Stellar.X.dll)
            "dllUrl": f"{ENDPOINT}/{BUCKET}/{key}",
            "sha256": sha256(dll),
            "minModSystemVersion": m["minModSystemVersion"],
            "maxModSystemVersion": m.get("maxModSystemVersion"),
        }
        if m.get("changelog"):
            version_entry["changelog"] = m["changelog"]

        plugins.append({
            "_dll": dll, "_key": key,
            "meta": {"id": m["id"], "name": m["name"], "description": m["description"], "author": m["author"]},
            "version": version_entry,
        })
    return plugins


def build_registry(plugins: list[dict], published: dict) -> dict:
    """Merge each plugin's current version into its published history (newest first)."""
    prior: dict[str, list] = {}
    for p in published.get("plugins", []):
        prior[p["id"]] = list(p.get("versions", []))

    entries = []
    for p in plugins:
        cur = p["version"]
        versions = [cur] + [v for v in prior.get(p["meta"]["id"], []) if v.get("version") != cur["version"]]
        versions.sort(key=lambda v: ver_key(v["version"]), reverse=True)
        entries.append({**p["meta"], "versions": versions})
    return {"plugins": entries}


def main() -> None:
    publish = "--publish" in sys.argv[1:]
    plugins = collect()
    if not plugins:
        sys.exit("no plugins found under plugins/*/manifest.json")

    registry = build_registry(plugins, fetch_published())
    out = ROOT / "dist" / "plugins.json"
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(registry, indent=2) + "\n", encoding="utf-8")
    print(f"built dist/plugins.json with {len(plugins)} plugins")

    if publish:
        for p in plugins:
            aws_cp(str(p["_dll"]), p["_key"])   # version-specific key
        aws_cp(str(out), "plugins.json")
        print("published DLLs + plugins.json to MinIO")


if __name__ == "__main__":
    main()
