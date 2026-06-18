# Plugin source repos (submodules)

Each curated plugin lives in its own `Stellar<Name>Plugin` repo (DIP17 model); this directory
submodules them so the whole plugin ecosystem can be checked out + browsed in one workspace:

```bash
git clone --recurse-submodules https://github.com/StellarProtocol/StellarResonancePlugins.git
# or, in an existing checkout:
git submodule update --init repos/
```

These submodules are **source mirrors for convenience**. The registry publishes each plugin by the
`repository` + `commit` pinned in its `plugins/<id>/manifest.json` (CI clones + builds that commit in
an isolated container) — not from these submodules. Keep a submodule's pin in step with its manifest
commit when you bump a plugin.
