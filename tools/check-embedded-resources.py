#!/usr/bin/env python3
"""Fail when a plugin's source references an embedded resource its build didn't produce.

A plugin that calls `Assembly.GetManifestResourceStream("Some.Resource.png")` but never embeds that
resource (a missing `<EmbeddedResource>` in the .csproj, or a `LogicalName` that doesn't match the
requested string) still COMPILES and RUNS — the call just returns null at runtime, so the framework
silently falls back to a placeholder (the launcher's puzzle-piece glyph, a blank stat-icon square…).
That exact regression shipped three plugins iconless. This check closes the gap in CI: scan the built
source for single-string `GetManifestResourceStream("NAME")` literals and assert each NAME is actually
embedded in the built DLL.

Detection is byte-level and dependency-free. A .NET assembly stores an embedded resource's NAME in the
`#Strings` metadata heap as a contiguous UTF-8 run, whereas a C# string literal lives in the `#US` heap
as UTF-16 (each character followed by a NUL byte). So the resource name appears as a contiguous ASCII
byte-run in the DLL ONLY when the resource is genuinely embedded under that exact name — which also
catches a LogicalName/requested-name mismatch (the requested string won't be in #Strings). The
two-arg `GetManifestResourceStream(typeof(T), "x")` overload (name = type namespace + "x") is
deliberately NOT matched — skipping it avoids false failures, never a false pass of the single-string
form this check targets.

Usage: check-embedded-resources.py <source-dir> <built-dll>
Exits non-zero (with GitHub ::error:: annotations) if any referenced resource is missing.
"""
import re
import sys
from pathlib import Path

# Single-string overload only: GetManifestResourceStream("Full.Logical.Name").
PATTERN = re.compile(r'GetManifestResourceStream\s*\(\s*"([^"]+)"')


def referenced_names(src: Path) -> set:
    names = set()
    for cs in src.rglob("*.cs"):
        p = str(cs)
        if "/obj/" in p or "/bin/" in p:
            continue
        try:
            names.update(PATTERN.findall(cs.read_text(encoding="utf-8", errors="ignore")))
        except OSError:
            continue
    return names


def main() -> int:
    if len(sys.argv) != 3:
        print("usage: check-embedded-resources.py <source-dir> <built-dll>", file=sys.stderr)
        return 2
    src, dll = Path(sys.argv[1]), Path(sys.argv[2])
    names = referenced_names(src)
    if not names:
        return 0
    blob = dll.read_bytes()
    missing = sorted(n for n in names if n.encode("utf-8") not in blob)
    for n in missing:
        print(f"::error::embedded resource '{n}' is referenced by GetManifestResourceStream() but is "
              f"NOT embedded in {dll.name} — add an <EmbeddedResource Include=\"...\" "
              f"LogicalName=\"{n}\" /> to the .csproj.")
    if missing:
        print(f"{dll.name}: {len(missing)} referenced resource(s) missing from the build "
              f"(of {len(names)} referenced).", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
