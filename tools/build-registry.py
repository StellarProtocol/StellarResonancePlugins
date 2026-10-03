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
import re
import subprocess
import sys
import urllib.error
import urllib.parse
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
# tags/homepage/media/guide are presentation metadata for the launcher's plugin detail page —
# they describe the plugin, not one build, so they are shared across channels.
SHARED_FIELDS = ("id", "name", "description", "author", "dll", "repository", "projectPath",
                 "tags", "homepage", "media", "guide", "icon")
# Fields a manifest.testing.json may carry — everything version-specific. A testing override
# may ONLY set these; shared fields come from manifest.json so they can't drift between files.
OVERRIDABLE = ("version", "date", "commit", "tag", "minModSystemVersion",
               "maxModSystemVersion", "capPriorVersionsAt", "changelog", "dependencies")


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


USER_AGENT = "stellar-registry-build/1 (+https://github.com/StellarProtocol/StellarResonancePlugins)"

# Media/guide (plugin detail page). Media types the launcher renders: "image" (lightbox),
# "youtube" (thumbnail tile, opens the watch page), "video" (hosted file, opens in browser).
MEDIA_TYPES = ("image", "youtube", "video")
MAX_MEDIA_BYTES = 25 << 20   # attached media file cap — keeps the repo and CDN lean
MAX_GUIDE_BYTES = 1 << 20


def is_http_url(u) -> bool:
    return isinstance(u, str) and (u.startswith("https://") or u.startswith("http://"))


def safe_rel_path(p) -> bool:
    return isinstance(p, str) and bool(p) and not p.startswith("/") and ".." not in p and "\\" not in p


_SHA256 = re.compile(r"^[0-9a-fA-F]{64}$")
_FORBIDDEN_GAME_PREFIXES = ("bepinex/", "stellar/plugins", "stellar/deps")
MAX_DEPENDENCY_BYTES = 512 << 20
# Launcher's DependencyPaths.IsValidPluginId (src/StellarLauncher.Core/Dependencies/DependencyPaths.cs:55-57):
# non-empty, not "." or "..", every char in [A-Za-z0-9._-].
_PLUGIN_ID_CHARS = re.compile(r"^[A-Za-z0-9._-]+$")
# Pure display/identity strings (no other format check already rejects whitespace-only below).
_STRIP_REQUIRED_DEP_FIELDS = ("id", "name", "version", "license", "licenseUrl", "sourceUrl")


def _bad_rel_path(p: str, *, allow_trailing_slash: bool = False) -> bool:
    """Mirrors the launcher's DependencyPaths.Resolve exactly (DependencyPaths.cs:15-17):
    reject None/empty, any backslash, a leading '/', ANY colon (not just a drive letter — this also
    catches an NTFS alternate-data-stream suffix like "dxgi.dll:ads"), and any '.'/'..'/empty
    PATH SEGMENT (not just '..') — so "./x", "a/./b" and "a//b" are rejected exactly like the launcher
    rejects them, instead of only the ".." case the registry used to check.

    `allow_trailing_slash` is a REGISTRY-only widening for a `kind: "zip"` directory-prefix
    destination (`to` ending in '/', paired with a `from` prefix ending in '/', e.g.
    `{"from": "Shaders/", "to": "fx/Shaders/"}`) — the launcher never calls Resolve with that raw
    `to`; it first appends the zip entry's own name (DependencyService.Zip.cs:81) and validates
    THAT concatenated result. For `kind: "file"` (or a `from` that is a single entry, not a prefix)
    `to` IS the literal destination Resolve sees, so a trailing '/' there must stay rejected."""
    if not isinstance(p, str) or not p or "\\" in p or p.startswith("/") or ":" in p:
        return True
    segments = p.split("/")
    if allow_trailing_slash and segments and segments[-1] == "":
        segments = segments[:-1]
        if not segments:
            return True
    return any(part in ("", ".", "..") for part in segments)


def _normalize_rel_path(p: str) -> str:
    """Join Split('/') segments back together — mirrors DependencyPaths.cs:16,18. A no-op once
    `_bad_rel_path` has rejected dot/empty segments, but it anchors every path-comparison site (the
    scan-path ban below, and `game_claims`' cross-plugin uniqueness check) to the same normalised
    form the launcher derives, rather than each site re-deriving it ad hoc (I1)."""
    return "/".join(p.split("/"))


def _valid_id_charset(s) -> bool:
    """Non-empty, not "." or "..", every char in [A-Za-z0-9._-] — the launcher's
    DependencyPaths.IsValidPluginId charset. Shared by a plugin's own `id` (`is_valid_plugin_id`,
    below) and a dependency's own `id` (`validate_dependencies`): both become path segments
    (`stellar/deps/<pluginId>/...` and the dependency ledger's own entries)."""
    return isinstance(s, str) and bool(s) and s not in (".", "..") and bool(_PLUGIN_ID_CHARS.match(s))


def is_valid_plugin_id(pid) -> bool:
    """The launcher's plugin-id charset (DependencyPaths.IsValidPluginId) — applied to a plugin's own
    `id` when IT declares `dependencies`, since that id becomes a path segment
    (`stellar/deps/<pluginId>/...`)."""
    return _valid_id_charset(pid)


def validate_dependencies(deps, where):
    """Errors for one plugin version's `dependencies` (empty list = valid). Generic: the launcher
    installs whatever passes here before the game starts, without knowing what any dependency is for.

    Field types are enforced strictly (C1) because the launcher parses the WHOLE registry with one
    `GetFromJsonAsync` — a single malformed field throws a `JsonException` and empties the entire
    curated catalog, not just the offending plugin. In particular `type(size) is int` (not
    `isinstance`): `isinstance(True, int)` is `True` in Python, so `"size": true` would otherwise be
    silently accepted as `size=1`. `moddedOnly`/`optional` must be real booleans, `requires` must be a
    list of strings (not silently iterated per-character when someone writes a bare string), and
    `licenseUrl`/`sourceUrl` are now REQUIRED strings (launcher manifest-standard.md §3: license,
    licenseUrl and sourceUrl are all mandatory) with `notice` optional-but-typed."""
    errs, ids = [], set()
    if not isinstance(deps, list):
        return [f"{where}: dependencies must be a list"]
    # (target, normalised-to.lower()) -> owning dependency id — duplicate-destination check across
    # EVERY dependency of this one plugin (M-c): two deps silently writing to the same place is a
    # bug the launcher would otherwise resolve arbitrarily (last-installed wins).
    dest_seen: dict[tuple[str, str], str] = {}
    for i, d in enumerate(deps):
        at = f"{where}: dependencies[{i}]"
        if not isinstance(d, dict):
            errs.append(f"{at} must be an object"); continue
        for k in ("id", "name", "version", "url", "sha256", "kind", "target", "license",
                  "licenseUrl", "sourceUrl"):
            v = d.get(k)
            if not isinstance(v, str) or not v:
                errs.append(f"{at}.{k} is required")
            # A handful of fields are pure display/identity strings with no other format check below
            # (url/sha256/kind/target are already caught by their own dedicated checks even when
            # whitespace-only) — those six must be non-empty after stripping, so "  " can't pass as
            # a "value" the way plain truthiness would let it.
            elif k in _STRIP_REQUIRED_DEP_FIELDS and not v.strip():
                errs.append(f"{at}.{k} is required")
        # "in d" (not "d.get(...) is not None"): a JSON `null` is indistinguishable from "absent" via
        # .get(), but `"optional": null` / `"moddedOnly": "true"` must still be flagged — the key IS
        # present, just with the wrong type.
        if "notice" in d and not isinstance(d.get("notice"), str):
            errs.append(f"{at}.notice must be a string")
        dep_id = d.get("id")
        if isinstance(dep_id, str) and dep_id and not _valid_id_charset(dep_id):
            errs.append(f"{at}.id must match the launcher's id charset [A-Za-z0-9._-]+ "
                        "(no '/', no '..', no whitespace)")
        if dep_id in ids:
            errs.append(f"{at}.id '{dep_id}' is duplicated")
        ids.add(dep_id)
        url = d.get("url")
        if isinstance(url, str) and url:
            if any(ch.isspace() for ch in url):
                errs.append(f"{at}.url must not contain whitespace")
            parsed = urllib.parse.urlparse(url)
            if parsed.scheme != "https" or not parsed.netloc:
                errs.append(f"{at}.url must be https")
        if not _SHA256.match(str(d.get("sha256", ""))):
            errs.append(f"{at}.sha256 must be 64 hex characters")
        size = d.get("size")
        # type(size) is int, NOT isinstance(size, int): bool is a subclass of int in Python, so
        # isinstance(True, int) is True and "size": true would sail through as size=1.
        if type(size) is not int or size <= 0 or size > MAX_DEPENDENCY_BYTES:
            errs.append(f"{at}.size must be an int, 1..{MAX_DEPENDENCY_BYTES} bytes")
        kind, target, files = d.get("kind"), d.get("target"), d.get("files")
        if kind not in ("file", "zip"):
            errs.append(f"{at}.kind must be file or zip")
        if target not in ("plugin", "game"):
            errs.append(f"{at}.target must be plugin or game")
        modded_only = d.get("moddedOnly")
        if "moddedOnly" in d and not isinstance(modded_only, bool):
            errs.append(f"{at}.moddedOnly must be a boolean")
        if modded_only and target != "game":
            errs.append(f"{at}.moddedOnly needs target game")
        if "optional" in d and not isinstance(d.get("optional"), bool):
            errs.append(f"{at}.optional must be a boolean")
        requires = d.get("requires")
        if "requires" in d and (not isinstance(requires, list) or not all(isinstance(r, str) for r in requires)):
            errs.append(f"{at}.requires must be a list of strings")
        if not isinstance(files, list) or not files:
            errs.append(f"{at}.files must be a non-empty list"); continue
        if kind == "file" and len(files) != 1:
            errs.append(f"{at}: kind file needs exactly one files entry")
        for j, f in enumerate(files):
            to = f.get("to") if isinstance(f, dict) else None
            from_val = f.get("from") if isinstance(f, dict) else None
            if isinstance(f, dict) and "from" in f and not isinstance(from_val, str):
                errs.append(f"{at}.files[{j}].from must be a string")
                from_val = None
            dir_prefix = kind == "zip" and isinstance(from_val, str) and from_val.endswith("/")
            if _bad_rel_path(to, allow_trailing_slash=dir_prefix):
                errs.append(f"{at}.files[{j}].to must be a relative path without ..")
            else:
                # I1: the scan-path ban runs on the NORMALISED path (mirrors DependencyPaths.cs:18),
                # case-insensitively, same as the launcher's StringComparison.OrdinalIgnoreCase.
                normalized = _normalize_rel_path(to)
                if target == "game" and normalized.lower().startswith(_FORBIDDEN_GAME_PREFIXES):
                    errs.append(f"{at}.files[{j}].to '{to}' is inside a scan path (BepInEx/, stellar/plugins, stellar/deps)")
                dest_key = (target, normalized.lower())
                if dest_key in dest_seen:
                    errs.append(f"{at}.files[{j}].to '{to}' duplicates a target {target!r} destination "
                                f"already used by dependency '{dest_seen[dest_key]}'")
                else:
                    dest_seen[dest_key] = d.get("id")
            if kind == "zip" and _bad_rel_path(from_val, allow_trailing_slash=True):
                errs.append(f"{at}.files[{j}].from is required for zip and must be relative")
    seen = set()
    for i, d in enumerate(deps):
        if not isinstance(d, dict):
            continue
        requires = d.get("requires")
        if isinstance(requires, list):
            for r in requires:
                if not isinstance(r, str):
                    continue  # type error already recorded above
                if r not in ids:
                    errs.append(f"{where}: dependencies[{i}].requires '{r}' is not a dependency of this plugin")
                elif r not in seen:
                    errs.append(f"{where}: dependencies[{i}].requires '{r}' must be listed before it (the launcher installs in order)")
        seen.add(d.get("id"))
    return errs


def game_claims(deps):
    """Normalised (I1) game-target destinations, for the cross-plugin uniqueness check."""
    out = []
    for d in deps or []:
        if isinstance(d, dict) and d.get("target") == "game":
            out += [_normalize_rel_path(f["to"]).lower() for f in d.get("files") or []
                    if isinstance(f, dict) and isinstance(f.get("to"), str)]
    return out


def resolve_docs(plugin_dir: Path, m: dict, where: str) -> tuple[dict, list[tuple[Path, str]]]:
    """Validate the optional presentation fields (tags / homepage / media / guide) and resolve
    repo-local files into published CDN keys. Returns (extra registry metadata, doc uploads).

    Unlike DLLs these live at stable, non-versioned keys (plugins/<id>/guide.md,
    plugins/<id>/media/<name>) — they are documentation, deliberately mutable so a typo fix
    doesn't require a release; the immutability rule covers release binaries only."""
    extra: dict = {}
    uploads: list[tuple[Path, str]] = []
    pid = m["id"]

    tags = m.get("tags")
    if tags is not None:
        if not isinstance(tags, list) or not tags or \
                not all(isinstance(t, str) and t.strip() for t in tags):
            sys.exit(f"{where}: tags must be a non-empty list of non-empty strings")
        extra["tags"] = [t.strip() for t in tags]

    homepage = m.get("homepage")
    if homepage is not None:
        if not is_http_url(homepage):
            sys.exit(f"{where}: homepage must be an http(s) URL")
        extra["homepage"] = homepage

    media = m.get("media")
    if media is not None:
        if not isinstance(media, list) or not media:
            sys.exit(f"{where}: media must be a non-empty list of objects")
        resolved_media = []
        for i, entry in enumerate(media):
            spot = f"{where}: media[{i}]"
            if not isinstance(entry, dict):
                sys.exit(f"{spot}: must be an object")
            mtype = entry.get("type")
            if mtype not in MEDIA_TYPES:
                sys.exit(f"{spot}: type must be one of {MEDIA_TYPES}")
            url, file = entry.get("url"), entry.get("file")
            if (url is None) == (file is None):
                sys.exit(f"{spot}: exactly one of url / file")
            if mtype == "youtube" and url is None:
                sys.exit(f"{spot}: youtube entries take a url (watch/shorts/embed link)")
            if url is not None and not is_http_url(url):
                sys.exit(f"{spot}: url must be http(s)")
            if file is not None:
                if not safe_rel_path(file):
                    sys.exit(f"{spot}: unsafe file path {file!r}")
                path = plugin_dir / file
                if not path.is_file():
                    sys.exit(f"{spot}: file not found: {file}")
                if path.stat().st_size > MAX_MEDIA_BYTES:
                    sys.exit(f"{spot}: {file} exceeds {MAX_MEDIA_BYTES >> 20} MB — host it (or YouTube) and use url")
                key = f"plugins/{pid}/media/{path.name}"
                uploads.append((path, key))
                url = f"{PUBLIC_BASE}/{key}"
            item = {"type": mtype, "url": url}
            caption = entry.get("caption")
            if caption is not None:
                if not isinstance(caption, str):
                    sys.exit(f"{spot}: caption must be a string")
                item["caption"] = caption
            resolved_media.append(item)
        keys = [key for _, key in uploads]
        if len(keys) != len(set(keys)):
            sys.exit(f"{where}: media file basenames must be unique (they share plugins/{pid}/media/)")
        extra["media"] = resolved_media

    icon = m.get("icon")
    if icon is not None:
        if is_http_url(icon):
            extra["iconUrl"] = icon
        else:
            if not safe_rel_path(icon):
                sys.exit(f"{where}: unsafe icon path {icon!r}")
            path = plugin_dir / icon
            if not path.is_file():
                sys.exit(f"{where}: icon not found: {icon}")
            if path.stat().st_size > MAX_MEDIA_BYTES:
                sys.exit(f"{where}: icon exceeds {MAX_MEDIA_BYTES >> 20} MB")
            key = f"plugins/{pid}/icon{path.suffix.lower()}"
            uploads.append((path, key))
            extra["iconUrl"] = f"{PUBLIC_BASE}/{key}"

    guide = m.get("guide")
    if guide is not None:
        if not safe_rel_path(guide):
            sys.exit(f"{where}: unsafe guide path {guide!r}")
        path = plugin_dir / guide
        if not path.is_file():
            sys.exit(f"{where}: guide not found: {guide}")
        if path.stat().st_size > MAX_GUIDE_BYTES:
            sys.exit(f"{where}: guide exceeds {MAX_GUIDE_BYTES >> 20} MB")
        key = f"plugins/{pid}/guide.md"
        uploads.append((path, key))
        extra["guideUrl"] = f"{PUBLIC_BASE}/{key}"

    return extra, uploads


def fetch_published(obj: str = "plugins.json", required: bool = False) -> dict:
    """Current published registry <obj> (read-only, public CDN). {} when legitimately absent (404).

    Always sends an explicit User-Agent: the CDN (Cloudflare) answers the default
    `Python-urllib/x.y` UA with 403, and the old bare `except: return {}` swallowed that on
    every CI publish — each release then merged against an EMPTY history and silently wiped
    every prior entry from versions[] (the launcher's version list collapsed to latest-only,
    2026-06-18..2026-07-11). `required=True` (publish mode) makes any failure other than a
    404 fatal: refusing to publish beats republishing with the history erased.
    """
    url = f"{PUBLIC_BASE}/{obj}"
    try:
        req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT})
        with urllib.request.urlopen(req, timeout=10) as r:
            return json.loads(r.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        if e.code == 404:
            return {}
        failure = f"HTTP {e.code}"
    except Exception as e:  # DNS, timeout, bad JSON, ...
        failure = str(e) or type(e).__name__
    if required:
        sys.exit(f"cannot fetch published {url} ({failure}) — refusing to publish: rebuilding "
                 "without the existing registry would wipe every prior versions[] entry")
    print(f"warning: fetch {url} failed ({failure}) — building without published history",
          file=sys.stderr)
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
    # game-target destination path (normalised, from game_claims) -> plugin id that claimed it,
    # tracked across EVERY plugin in this run so two unrelated plugins can't silently clobber the
    # same game-install path.
    claimed: dict[str, str] = {}
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

            deps = m.get("dependencies")
            if deps is not None:
                if not is_valid_plugin_id(m["id"]):
                    sys.exit(f"{where}: id {m['id']!r} must match the launcher's plugin-id charset "
                             "[A-Za-z0-9._-] to declare dependencies (it becomes a path segment "
                             "under stellar/deps/<id>/)")
                dep_errs = validate_dependencies(deps, f"{m['id']} {m['version']}")
                if dep_errs:
                    sys.exit("\n".join(dep_errs))
                for path in game_claims(deps):
                    if path in claimed and claimed[path] != m["id"]:
                        sys.exit(f"{where}: dependencies claim '{path}' already claimed by {claimed[path]}")
                    claimed[path] = m["id"]

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
            if deps is not None:
                version_entry["dependencies"] = deps
            # Provenance: when a plugin builds from its own pinned public repo (DIP17 model),
            # record where the binary came from so the registry is auditable. commit is
            # authoritative; tag (if any) is display-only.
            if m.get("repository"):
                version_entry["sourceRepository"] = m["repository"]
                version_entry["sourceCommit"] = m["commit"]
                if m.get("tag"):
                    version_entry["sourceTag"] = m["tag"]

            extra_meta, doc_uploads = resolve_docs(plugin_dir, m, where)

            plugins.append({
                "_dll": staged, "_key": key, "_docs": doc_uploads,
                # capPriorVersionsAt: when this build requires a newer framework, retro-cap older
                # published versions (whose maxModSystemVersion is still null) at this framework
                # version, so the launcher stops offering them on the newer framework. The published
                # history is otherwise carried forward verbatim, so this is the only sanctioned way
                # to bound a prior build. See docs/manifest-standard.md § Compatibility rule.
                "cap_prior": m.get("capPriorVersionsAt"),
                "channel": channel,
                "meta": {"id": m["id"], "name": m["name"], "description": m["description"],
                         "author": m["author"], **extra_meta},
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
    registry = build_registry(plugins, fetch_published(obj, required=publish))
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
        seen_docs: set[str] = set()             # stable+testing records share the same doc files
        for p in plugins:
            for path, doc_key in p["_docs"]:
                if doc_key in seen_docs:
                    continue
                seen_docs.add(doc_key)
                aws_cp(str(path), doc_key)
    _emit("plugins.json", stable, publish)
    _emit("plugins-testing.json", plugins, publish)
    if publish:
        print("published DLLs + guide/media docs + plugins.json + plugins-testing.json to MinIO")


if __name__ == "__main__":
    main()
