#!/usr/bin/env python3
"""Build + publish the Stellar plugin registry to MinIO.

Scans plugins/<id>/manifest.json (+ the DLL beside each), computes sha256,
assembles plugins.json, and uploads every DLL + plugins.json to the public
`stellar` MinIO bucket. This repo OWNS the registry — decoupled from the
framework's releases. Community plugins are added by PR (a new plugins/<id>/).

Usage:
  build-registry.py            # validate + build dist/plugins.json
  build-registry.py --publish  # also upload DLLs + plugins.json to MinIO (needs S3 creds)
"""
from __future__ import annotations

import hashlib
import json
import os
import subprocess
import sys
from pathlib import Path

ENDPOINT = "https://minio.revette.io"
BUCKET = "stellar"
ROOT = Path(__file__).resolve().parents[1]
PLUGINS_DIR = ROOT / "plugins"
REQUIRED = ("id", "name", "description", "version", "dll", "author")


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as fh:
        for chunk in iter(lambda: fh.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def aws_cp(src: str, key: str) -> None:
    env = {**os.environ,
           "AWS_DEFAULT_REGION": "sa-east-1",
           "AWS_REQUEST_CHECKSUM_CALCULATION": "when_required",
           "AWS_RESPONSE_CHECKSUM_VALIDATION": "when_required"}
    subprocess.run(["aws", "s3", "cp", src, f"s3://{BUCKET}/{key}",
                    "--endpoint-url", ENDPOINT], check=True, env=env)


def collect() -> list[dict]:
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
        plugins.append({"_dll": dll, "entry": {
            "id": m["id"], "name": m["name"], "description": m["description"],
            "version": m["version"], "author": m["author"],
            "dllUrl": f"{ENDPOINT}/{BUCKET}/plugins/{m['dll']}",
            "sha256": sha256(dll),
        }})
    return plugins


def main() -> None:
    publish = "--publish" in sys.argv[1:]
    plugins = collect()
    if not plugins:
        sys.exit("no plugins found under plugins/*/manifest.json")

    registry = {"plugins": [p["entry"] for p in plugins]}
    out = ROOT / "dist" / "plugins.json"
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(registry, indent=2) + "\n", encoding="utf-8")
    print(f"built dist/plugins.json with {len(plugins)} plugins")

    if publish:
        for p in plugins:
            aws_cp(str(p["_dll"]), f"plugins/{p['_dll'].name}")
        aws_cp(str(out), "plugins.json")
        print("published DLLs + plugins.json to MinIO")


if __name__ == "__main__":
    main()
