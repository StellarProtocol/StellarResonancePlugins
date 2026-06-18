#!/usr/bin/env python3
"""Build + publish the Stellar plugin registry to MinIO.

Scans plugins/<id>/manifest.json (+ the DLL beside each), computes sha256, assembles
plugins.json, and uploads every DLL + plugins.json to the public `stellar` MinIO bucket.
This repo OWNS the registry — decoupled from the framework's releases. Community plugins
are added by PR (a new plugins/<id>/).

Channels (two-file source model):
  - plugins/<id>/manifest.json          canonical record: shared fields + ONE version.
                                        Its optional `channel` (default "stable") is the
                                        channel of THAT version — set "testing" for a
                                        not-yet-stable plugin.
  - plugins/<id>/manifest.testing.json  OPTIONAL sibling: a second, testing-channel version
                                        that INHERITS the shared fields (id/name/dll/repo/…)
                                        from manifest.json and overrides only version-specific
                                        ones (version/commit/tag/min/…). This is how one plugin
                                        is live on stable AND testing at once without
                                        duplicating shared metadata.

build-registry emits two registry files: plugins.json (stable versions only) and
plugins-testing.json (a superset — every version of every plugin). The launcher reads the
file for the user's selected channel. Each plugin carries a versions[] history (newest
first) and declares the framework (modsystem) range each build runs on
(minModSystemVersion / optional maxModSystemVersion). DLLs are uploaded under
version-specific keys (plugins/<id>/<name>-<version>.dll), and the new release is APPENDED
to the published history — old versions stay downloadable so users can roll back. See
docs/manifest-standard.md.

Source provenance: a build is pinned by `commit` (immutable, authoritative — CI builds this
exact SHA). An optional `tag` is display-only provenance; CI may verify tag→commit but never
builds from a tag alone.

Usage:
  build-registry.py            # validate + build dist/plugins{,-testing}.json (merges history)
  build-registry.py --publish  # also upload DLLs + the two registry files (needs S3 creds)
  build-registry.py --targets  # print the per-(plugin,channel) build plan as TSV (for CI)
"""
from __future__ import annotations

import hashlib
import json
import os
import subprocess
import sys
import urllib.request
from pathlib import Path

# Storage = Cloudflare R2. The S3 API endpoint (writes) and the public CDN base (reads) differ:
# writes go to s3://<BUCKET>/<key> via S3_ENDPOINT; the public custom domain maps to the bucket
# root, so public URLs are PUBLIC_BASE/<key> (no bucket segment in the path).
S3_ENDPOINT = "https://757f61fd2bda67f9a3bc7c3b9b8d62e1.r2.cloudflarestorage.com"
BUCKET = "cdn"
PUBLIC_BASE = "https://cdn.revette.io"
ROOT = Path(__file__).resolve().parents[1]
PLUGINS_DIR = ROOT / "plugins"
REQUIRED = ("id", "name", "description", "version", "dll", "author", "minModSystemVersion")

# Fields the canonical manifest.json owns and a testing override INHERITS (never repeats).
SHARED_FIELDS = ("id", "name", "description", "author", "dll", "repository", "projectPath")
# Fields a manifest.testing.json may carry — everything version-specific. A testing override
# may ONLY set these; shared fields come from manifest.json so they can't drift between files.
OVERRIDABLE = ("version", "date", "commit", "tag", "minModSystemVersion",
               "maxModSystemVersion", "capPriorVersionsAt", "changelog")


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as fh:
        for chunk in iter(lambda: fh.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def ver_key(v: str) -> tuple:
    return tuple(int(p) if p.isdigit() else 0 for p in str(v).lstrip("vV").split("."))


def staged_name(dll: str, version: str) -> str:
    """On-disk filename CI stages each build under — version-suffixed so a plugin's stable and
    testing builds (same assembly name, different commits) don't collide in plugins/<id>/."""
    stem, suffix = Path(dll).stem, Path(dll).suffix
    return f"{stem}-{version}{suffix}"


def aws_cp(src: str, key: str) -> None:
    env = {**os.environ,
           "AWS_DEFAULT_REGION": "auto",   # Cloudflare R2
           "AWS_REQUEST_CHECKSUM_CALCULATION": "when_required",
           "AWS_RESPONSE_CHECKSUM_VALIDATION": "when_required"}
    subprocess.run(["aws", "s3", "cp", src, f"s3://{BUCKET}/{key}",
                    "--endpoint-url", S3_ENDPOINT], check=True, env=env)


def fetch_published(obj: str = "plugins.json") -> dict:
    """Current published registry <obj> (read-only, public CDN). Best-effort — {} if absent."""
    try:
        with urllib.request.urlopen(f"{PUBLIC_BASE}/{obj}", timeout=10) as r:
            return json.loads(r.read().decode("utf-8"))
    except Exception:
        return {}


def load_records(plugin_dir: Path) -> list[tuple[str, dict]]:
    """Resolve a plugin dir into (channel, resolved-manifest) records.

    manifest.json is the canonical record; its `channel` (default "stable") is the channel of
    its own version. An optional manifest.testing.json adds a second, testing-channel record
    whose shared fields are INHERITED from manifest.json and whose version-specific fields come
    from the override — so the shared metadata lives in exactly one place (no drift)."""
    base = json.loads((plugin_dir / "manifest.json").read_text(encoding="utf-8"))
    base_channel = base.get("channel", "stable")
    records: list[tuple[str, dict]] = [(base_channel, base)]

    testing_path = plugin_dir / "manifest.testing.json"
    if testing_path.is_file():
        if base_channel == "testing":
            sys.exit(f"{testing_path}: manifest.json is already channel=testing — a plugin can't have "
                     "both a testing-only manifest.json and a manifest.testing.json. Drop the "
                     "`channel` field from manifest.json (so it's the stable build) or delete this file.")
        override = json.loads(testing_path.read_text(encoding="utf-8"))
        stray = [k for k in override if k not in OVERRIDABLE]
        if stray:
            sys.exit(f"{testing_path}: may only override {OVERRIDABLE}; stray keys {stray} "
                     "(shared fields are inherited from manifest.json — don't repeat them)")
        merged = {k: base[k] for k in SHARED_FIELDS if k in base}
        merged.update(override)
        records.append(("testing", merged))
    return records


def plugin_dirs() -> list[Path]:
    return sorted(d for d in PLUGINS_DIR.iterdir() if (d / "manifest.json").is_file())


def collect() -> list[dict]:
    """One record per (plugin, channel-version): its registry version entry + the DLL to upload."""
    plugins = []
    for plugin_dir in plugin_dirs():
        for channel, m in load_records(plugin_dir):
            where = f"{plugin_dir.name}[{channel}]"
            missing = [k for k in REQUIRED if not m.get(k)]
            if missing:
                sys.exit(f"{where}: missing fields {missing}")
            if "/" in m["id"] or ".." in m["id"] or "/" in m["dll"] or ".." in m["dll"]:
                sys.exit(f"{where}: unsafe id/dll")
            if m.get("repository") and not m.get("commit"):
                sys.exit(f"{where}: repository pinned but no commit (commit is authoritative)")

            staged = plugin_dir / staged_name(m["dll"], m["version"])
            if not staged.is_file():
                sys.exit(f"{where}: built dll not found: {staged.name} "
                         "(CI stages each build version-suffixed via publish.yml; build it first)")

            key = f"plugins/{m['id']}/{staged.name}"
            version_entry = {
                "version": m["version"],
                "date": m.get("date", ""),
                "dll": m["dll"],                 # canonical on-disk filename (the assembly name, e.g. Stellar.X.dll)
                "dllUrl": f"{PUBLIC_BASE}/{key}",
                "sha256": sha256(staged),
                "minModSystemVersion": m["minModSystemVersion"],
                "maxModSystemVersion": m.get("maxModSystemVersion"),
            }
            if m.get("changelog"):
                version_entry["changelog"] = m["changelog"]
            # Provenance: when a plugin builds from its own pinned public repo (DIP17 model),
            # record where the binary came from so the registry is auditable. commit is
            # authoritative; tag (if any) is display-only.
            if m.get("repository"):
                version_entry["sourceRepository"] = m["repository"]
                version_entry["sourceCommit"] = m["commit"]
                if m.get("tag"):
                    version_entry["sourceTag"] = m["tag"]

            plugins.append({
                "_dll": staged, "_key": key,
                # capPriorVersionsAt: when this build requires a newer framework, retro-cap older
                # published versions (whose maxModSystemVersion is still null) at this framework
                # version, so the launcher stops offering them on the newer framework. The published
                # history is otherwise carried forward verbatim, so this is the only sanctioned way
                # to bound a prior build. See docs/manifest-standard.md § Compatibility rule.
                "cap_prior": m.get("capPriorVersionsAt"),
                "channel": channel,
                "meta": {"id": m["id"], "name": m["name"], "description": m["description"], "author": m["author"]},
                "version": version_entry,
            })
    return plugins


def build_registry(plugins: list[dict], published: dict) -> dict:
    """Merge each plugin's current version(s) into its published history (newest first).

    A plugin may contribute MORE THAN ONE current record here (its stable + its testing build),
    so records are grouped by id and all current versions land in the same versions[] list."""
    prior: dict[str, list] = {}
    for p in published.get("plugins", []):
        prior[p["id"]] = list(p.get("versions", []))

    grouped: dict[str, dict] = {}
    order: list[str] = []
    for p in plugins:
        pid = p["meta"]["id"]
        g = grouped.get(pid)
        if g is None:
            g = grouped[pid] = {"meta": p["meta"], "curs": [], "cap": None}
            order.append(pid)
        g["curs"].append(p["version"])
        if p.get("cap_prior"):
            g["cap"] = p["cap_prior"]

    entries = []
    for pid in order:
        g = grouped[pid]
        cur_strs = {v["version"] for v in g["curs"]}
        olds = [v for v in prior.get(pid, []) if v.get("version") not in cur_strs]
        if g["cap"]:
            for v in olds:
                if not v.get("maxModSystemVersion"):
                    v["maxModSystemVersion"] = g["cap"]
        versions = list(g["curs"]) + olds
        versions.sort(key=lambda v: ver_key(v["version"]), reverse=True)
        entries.append({**g["meta"], "versions": versions})
    return {"plugins": entries}


def _emit(obj: str, plugins: list[dict], publish: bool) -> None:
    """Build dist/<obj> from these plugins (merged with the published <obj> history); upload if asked."""
    registry = build_registry(plugins, fetch_published(obj))
    out = ROOT / "dist" / obj
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(registry, indent=2) + "\n", encoding="utf-8")
    print(f"built dist/{obj} with {len(registry['plugins'])} plugins")
    if publish:
        aws_cp(str(out), obj)


def print_targets() -> None:
    """Emit the per-(plugin,channel) build plan as TSV for CI's sandboxed clone-and-build.
    Columns: id  channel  repository  commit  tag  projectPath  dll  version  stagedName

    Possibly-empty fields (repository/commit/tag) are emitted as "-" so no field is ever the empty
    string: bash `read` with IFS=$'\\t' treats tab as IFS-whitespace and COLLAPSES adjacent tabs,
    which would swallow an empty field and shift every later column. CI maps "-" back to empty."""
    def s(v: str) -> str:
        return v if v else "-"
    for plugin_dir in plugin_dirs():
        for channel, m in load_records(plugin_dir):
            print("\t".join([
                m["id"], channel, s(m.get("repository", "")), s(m.get("commit", "")),
                s(m.get("tag", "")), m.get("projectPath", "."), m["dll"], m["version"],
                staged_name(m["dll"], m["version"]),
            ]))


def main() -> None:
    argv = sys.argv[1:]
    if "--targets" in argv:
        print_targets()
        return

    publish = "--publish" in argv
    plugins = collect()
    if not plugins:
        sys.exit("no plugins found under plugins/*/manifest.json")

    # Two channels: plugins-testing.json carries ALL versions of all plugins; plugins.json carries
    # only the stable ones. The launcher reads the file for the user's selected channel (testing is
    # a superset). DLLs are shared (version-specific keys), uploaded once.
    stable = [p for p in plugins if p.get("channel", "stable") != "testing"]
    if publish:
        for p in plugins:
            aws_cp(str(p["_dll"]), p["_key"])   # version-specific key, shared across channels
    _emit("plugins.json", stable, publish)
    _emit("plugins-testing.json", plugins, publish)
    if publish:
        print("published DLLs + plugins.json + plugins-testing.json to MinIO")


if __name__ == "__main__":
    main()
