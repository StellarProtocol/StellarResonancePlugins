import importlib.util, pathlib, unittest

_spec = importlib.util.spec_from_file_location("build_registry", pathlib.Path(__file__).with_name("build-registry.py"))
br = importlib.util.module_from_spec(_spec); _spec.loader.exec_module(br)

def dep(**kw):
    d = {"id": "fx", "name": "FX", "version": "1.0.0", "url": "https://cdn.example/fx.dll",
         "sha256": "a" * 64, "size": 1000, "kind": "file", "files": [{"to": "dxgi.dll"}],
         "target": "game", "license": "BSD-3-Clause",
         "licenseUrl": "https://example.com/license", "sourceUrl": "https://example.com"}
    d.update(kw); return d

class ValidateDependencies(unittest.TestCase):
    def test_valid(self):
        self.assertEqual([], br.validate_dependencies([dep(), dep(id="b", requires=["fx"], files=[{"to": "b.addon64"}])], "p"))
    def test_https_only(self):
        self.assertTrue(br.validate_dependencies([dep(url="http://x/y")], "p"))
    def test_sha256_shape(self):
        self.assertTrue(br.validate_dependencies([dep(sha256="xyz")], "p"))
    def test_relative_paths_only(self):
        for bad in ["../x.dll", "/abs.dll", "C:/x.dll", "a/../../b"]:
            self.assertTrue(br.validate_dependencies([dep(files=[{"to": bad}])], "p"), bad)
    def test_scan_paths_refused_for_game_target(self):
        for bad in ["BepInEx/plugins/x.dll", "stellar/plugins/x/y.dll", "Stellar/Deps/x", "bepinex/core/x.dll"]:
            self.assertTrue(br.validate_dependencies([dep(files=[{"to": bad}])], "p"), bad)
    def test_modded_only_needs_game_target(self):
        self.assertTrue(br.validate_dependencies([dep(target="plugin", moddedOnly=True)], "p"))
    def test_file_kind_has_one_file(self):
        self.assertTrue(br.validate_dependencies([dep(files=[{"to": "a"}, {"to": "b"}])], "p"))
    def test_zip_needs_from(self):
        self.assertTrue(br.validate_dependencies([dep(kind="zip", files=[{"to": "a"}])], "p"))
        self.assertEqual([], br.validate_dependencies([dep(kind="zip", files=[{"from": "Shaders/", "to": "fx/Shaders/"}])], "p"))
    def test_requires_must_exist_and_ids_unique(self):
        self.assertTrue(br.validate_dependencies([dep(requires=["nope"])], "p"))
        self.assertTrue(br.validate_dependencies([dep(id="b", requires=["fx"], files=[{"to": "b"}]), dep()], "p"))
        self.assertTrue(br.validate_dependencies([dep(), dep()], "p"))
    def test_license_required(self):
        self.assertTrue(br.validate_dependencies([dep(license="")], "p"))
    def test_game_claims_normalise(self):
        self.assertEqual(["dxgi.dll"], br.game_claims([dep(files=[{"to": "DXGI.dll"}]), dep(id="p", target="plugin", files=[{"to": "x"}])]))

    # --- C1: field types (final-review.md REGISTRY §C1) ---------------------------------------
    # A JsonException on ANY field empties the launcher's whole curated catalog (one GetFromJsonAsync
    # over the entire registry) — these repro cases previously passed validate_dependencies silently.

    def test_c1_optional_null_rejected(self):
        self.assertTrue(br.validate_dependencies([dep(optional=None)], "p"))

    def test_c1_modded_only_wrong_type_rejected(self):
        # "true" (a string) must not be accepted in place of the boolean True — even with
        # target="game" so the pre-existing truthy semantic check alone would stay silent.
        self.assertTrue(br.validate_dependencies([dep(moddedOnly="true")], "p"))

    def test_c1_size_bool_rejected(self):
        # isinstance(True, int) is True in Python — size must use type(size) is int, not isinstance.
        self.assertTrue(br.validate_dependencies([dep(size=True)], "p"))

    def test_c1_requires_bare_string_rejected(self):
        # "requires": "a" must not be silently iterated character-by-character.
        errs = br.validate_dependencies([dep(requires="a")], "p")
        self.assertTrue(errs)
        self.assertTrue(any("requires must be a list of strings" in e for e in errs))

    def test_c1_license_url_and_source_url_str_when_present(self):
        self.assertTrue(br.validate_dependencies([dep(licenseUrl=123)], "p"))
        self.assertTrue(br.validate_dependencies([dep(sourceUrl=123)], "p"))

    def test_c1_notice_must_be_string_when_present(self):
        self.assertTrue(br.validate_dependencies([dep(notice=123)], "p"))

    def test_description_must_be_string_when_present(self):
        self.assertTrue(br.validate_dependencies([dep(description=123)], "p"))
        self.assertTrue(br.validate_dependencies([dep(description=None)], "p"))
        self.assertEqual(br.validate_dependencies([dep(description="Adds effects.")], "p"), [])

    def test_c1_files_from_must_be_string_when_present(self):
        self.assertTrue(br.validate_dependencies([dep(files=[{"to": "a", "from": 123}])], "p"))

    # --- I1: path rules must match the launcher's DependencyPaths exactly ----------------------

    def test_i1_launcher_path_rules_bypassed_before_fix(self):
        for bad in ["./BepInEx/plugins/x.dll", "stellar//plugins/x.dll", "dxgi.dll:ads", "a/./b"]:
            self.assertTrue(br.validate_dependencies([dep(files=[{"to": bad}])], "p"), bad)

    def test_i1_trailing_slash_rejected_for_file_kind(self):
        self.assertTrue(br.validate_dependencies([dep(kind="file", files=[{"to": "a/"}])], "p"))

    def test_i1_trailing_slash_still_allowed_for_zip_dir_prefix(self):
        # Not a regression target of I1 — pins that the dir-prefix convention (from "Shaders/" ->
        # to "fx/Shaders/") the launcher's own BuildZipKind relies on keeps working.
        self.assertEqual([], br.validate_dependencies(
            [dep(kind="zip", files=[{"from": "Shaders/", "to": "fx/Shaders/"}])], "p"))

    def test_i1_colon_anywhere_rejected_not_just_drive_letter(self):
        self.assertTrue(br.validate_dependencies([dep(files=[{"to": "dxgi.dll:ads"}])], "p"))

    def test_i1_game_claims_compares_normalized_paths(self):
        # Two different plugins' dependency lists naming the same destination in different case must
        # normalise to the SAME key, so collect()'s cross-plugin `claimed` dict catches the clash
        # instead of silently treating "DXGI.dll" and "dxgi.dll" as different paths.
        a = br.game_claims([dep(id="a", files=[{"to": "DXGI.dll"}])])
        b = br.game_claims([dep(id="b", files=[{"to": "dxgi.dll"}])])
        self.assertEqual(a, b)

    def test_i1_plugin_id_charset_matches_launcher(self):
        # DependencyPaths.IsValidPluginId (DependencyPaths.cs:55-57).
        for good in ["combatmeter", "my.plugin_v2", "a-b-c"]:
            self.assertTrue(br.is_valid_plugin_id(good), good)
        for bad in ["", ".", "..", "my plugin", "my/plugin", "my@plugin", None]:
            self.assertFalse(br.is_valid_plugin_id(bad), bad)

    # --- M-c: duplicate `to` across one plugin's own dependencies -------------------------------

    def test_mc_duplicate_to_within_same_plugin_rejected(self):
        errs = br.validate_dependencies(
            [dep(id="a", files=[{"to": "dxgi.dll"}]), dep(id="b", files=[{"to": "dxgi.dll"}])], "p")
        self.assertTrue(errs)
        self.assertTrue(any("duplicates a target" in e for e in errs))

    def test_mc_duplicate_to_case_insensitive(self):
        errs = br.validate_dependencies(
            [dep(id="a", files=[{"to": "DXGI.dll"}]), dep(id="b", files=[{"to": "dxgi.dll"}])], "p")
        self.assertTrue(any("duplicates a target" in e for e in errs))

    def test_mc_same_to_different_target_not_a_duplicate(self):
        # target "game" and target "plugin" install under different roots, so the same relative
        # "to" string is not actually a collision.
        self.assertEqual([], br.validate_dependencies(
            [dep(id="a", target="game", files=[{"to": "x.dll"}]),
             dep(id="b", target="plugin", files=[{"to": "x.dll"}])], "p"))

    # --- Boundary test: MAX_DEPENDENCY_BYTES ----------------------------------------------------

    def test_boundary_max_dependency_bytes(self):
        self.assertEqual([], br.validate_dependencies([dep(size=br.MAX_DEPENDENCY_BYTES)], "p"))
        self.assertTrue(br.validate_dependencies([dep(size=br.MAX_DEPENDENCY_BYTES + 1)], "p"))
        self.assertTrue(br.validate_dependencies([dep(size=0)], "p"))

    # --- Follow-up mismatches vs. the launcher, second pass --------------------------------------

    def test_dependency_id_charset_matches_launcher(self):
        for bad in ["my fx", "a/b", ".."]:
            self.assertTrue(br.validate_dependencies([dep(id=bad)], "p"), bad)

    def test_url_parsed_strictly_https_with_host(self):
        self.assertTrue(br.validate_dependencies([dep(url="https://")], "p"))

    def test_url_rejects_embedded_whitespace(self):
        self.assertTrue(br.validate_dependencies([dep(url="https://a b/c")], "p"))

    def test_required_strings_non_empty_after_strip(self):
        self.assertTrue(br.validate_dependencies([dep(license="  ")], "p"))

# --- launcher i18n: per-language plugin presentation (spec 2026-10-09 § B) ----------------------

import json, tempfile, contextlib, io

CL = {"added": ["New view."], "fixed": ["A crash."]}
MEDIA = [{"type": "image", "url": "https://x/a.png", "caption": "Overview"},
         {"type": "image", "url": "https://x/b.png"}]

class ValidateI18n(unittest.TestCase):
    def ok(self, i18n, media=MEDIA, cl=CL):
        self.assertEqual([], br.validate_i18n(i18n, media, cl, "p"))
    def bad(self, i18n, media=MEDIA, cl=CL, needle=None):
        errs = br.validate_i18n(i18n, media, cl, "p")
        self.assertTrue(errs, i18n)
        if needle:
            self.assertTrue(any(needle in e for e in errs), errs)

    def test_valid_full_block(self):
        self.ok({"ja": {"name": "コンバットメーター", "description": "説明", "captions": ["概要"],
                        "changelog": {"added": ["新しいビュー"]}},
                 "ko": {"description": "설명"}, "fil": {"captions": [None]}})
    def test_all_five_languages_accepted(self):
        self.ok({lang: {"name": "N"} for lang in ("ja", "th", "id", "fil", "ko")})
    def test_unknown_or_english_language_rejected(self):
        self.bad({"jp": {"name": "x"}}, needle="unsupported language")
        self.bad({"en": {"name": "x"}}, needle="unsupported language")
        self.bad({"zh": {"name": "x"}})
    def test_not_an_object(self):
        self.bad(["ja"]); self.bad({"ja": "x"}); self.bad({"ja": {}})
    def test_unknown_key_rejected(self):
        self.bad({"ja": {"title": "x"}}, needle="unknown keys")
    def test_name_and_description_types_and_sizes(self):
        self.bad({"ja": {"name": ""}}); self.bad({"ja": {"name": "  "}}); self.bad({"ja": {"name": 3}})
        self.bad({"ja": {"name": "x" * (br.MAX_I18N_NAME + 1)}})
        self.ok({"ja": {"name": "x" * br.MAX_I18N_NAME}})
        self.bad({"ja": {"description": None}})
        self.bad({"ja": {"description": "x" * (br.MAX_I18N_DESCRIPTION + 1)}})
    def test_captions_per_media_index(self):
        self.ok({"ja": {"captions": ["概要", None]}})           # null = untranslated
        self.bad({"ja": {"captions": ["a", None, "c"]}}, needle="media has 2")
        self.bad({"ja": {"captions": ["a"]}}, media=None, needle="media has 0")
        self.bad({"ja": {"captions": []}}); self.bad({"ja": {"captions": "a"}})
        self.bad({"ja": {"captions": [3]}})
    def test_caption_needs_english_caption(self):
        self.bad({"ja": {"captions": [None, "b"]}}, needle="no English caption")
    def test_changelog_shape(self):
        self.ok({"th": {"changelog": {"fixed": ["แก้ไข"]}}})
        self.bad({"th": {"changelog": {"removed": ["x"]}}}, needle="not a section")
        self.bad({"th": {"changelog": {"added": []}}}, needle="English section is not")
        self.ok({"th": {"changelog": {"added": ["x"], "removed": []}}}, cl={**CL, "removed": []})  # mirrors an empty English section
        self.bad({"th": {"changelog": {"added": "x"}}})
        self.bad({"th": {"changelog": {"added": [""]}}})
        self.bad({"th": {"changelog": {}}})
    def test_changelog_needs_english_changelog(self):
        self.bad({"th": {"changelog": {"added": ["x"]}}}, cl=None, needle="no English `changelog`")

class I18nEmit(unittest.TestCase):
    def test_presentation_split_and_lang_order(self):
        i18n = {"ko": {"name": "K", "changelog": {"added": ["k"]}}, "ja": {"captions": ["c"]},
                "th": {"changelog": {"added": ["t"]}}}
        self.assertEqual(br.i18n_presentation(i18n), {"ja": {"captions": ["c"]}, "ko": {"name": "K"}})
        self.assertEqual(list(br.i18n_presentation(i18n)), ["ja", "ko"])
        self.assertEqual(br.i18n_changelogs(i18n), {"th": {"added": ["t"]}, "ko": {"added": ["k"]}})
    def test_absent_emits_nothing(self):
        self.assertEqual(br.i18n_presentation(None), {})
        self.assertEqual(br.i18n_changelogs(None), {})


def manifest(**kw):
    m = {"id": "demo", "name": "Demo", "description": "A demo.", "version": "1.0.0",
         "dll": "Stellar.Demo.dll", "author": "me", "minModSystemVersion": "2.0.0",
         "changelog": dict(CL), "media": [dict(MEDIA[0])], "guide": "guide.md"}
    m.update(kw)
    return {k: v for k, v in m.items() if v is not None}


class RegistryTree(unittest.TestCase):
    """End-to-end over a throwaway plugins/ tree (collect + build_registry), no network."""
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory(); root = pathlib.Path(self._tmp.name)
        self.dir = root / "plugins" / "demo"; self.dir.mkdir(parents=True)
        (self.dir / "guide.md").write_text("# Demo\n![x](media/a.png)\n")
        self._saved = (br.ROOT, br.PLUGINS_DIR); br.ROOT, br.PLUGINS_DIR = root, root / "plugins"
    def tearDown(self):
        br.ROOT, br.PLUGINS_DIR = self._saved; self._tmp.cleanup()

    def write(self, m, testing=None):
        (self.dir / "manifest.json").write_text(json.dumps(m))
        versions = [m["version"]]
        if testing is not None:
            (self.dir / "manifest.testing.json").write_text(json.dumps(testing)); versions.append(testing["version"])
        for v in versions:
            (self.dir / br.staged_name(m["dll"], v)).write_bytes(b"dll" + v.encode())

    def build(self, channel=None):
        plugins = br.collect()
        if channel:
            plugins = [p for p in plugins if p["channel"] == channel]
        return br.build_registry(plugins, {})["plugins"][0], plugins

    def fails(self):
        with self.assertRaises(SystemExit) as cm, contextlib.redirect_stderr(io.StringIO()):
            br.collect()
        return str(cm.exception.code)

    def test_english_unchanged_and_no_new_keys_without_i18n(self):
        self.write(manifest())
        entry, plugins = self.build()
        self.assertEqual(list(entry), ["id", "name", "description", "author", "media", "guideUrl", "versions"])
        self.assertNotIn("changelogI18n", entry["versions"][0])
        self.assertEqual(entry["versions"][0]["changelog"], CL)
        self.assertEqual([k for _, k in plugins[0]["_docs"]], ["plugins/demo/guide.md"])

    def test_i18n_emitted_english_kept(self):
        self.write(manifest(i18n={"ko": {"name": "데모", "captions": ["개요"],
                                         "changelog": {"added": ["새 보기"]}},
                                  "ja": {"description": "デモ"}}))
        entry, _ = self.build()
        self.assertEqual((entry["name"], entry["description"]), ("Demo", "A demo."))
        self.assertEqual(entry["media"][0]["caption"], "Overview")
        self.assertEqual(entry["i18n"], {"ja": {"description": "デモ"},
                                         "ko": {"name": "데모", "captions": ["개요"]}})
        v = entry["versions"][0]
        self.assertEqual(v["changelog"], CL)
        self.assertEqual(v["changelogI18n"], {"ko": {"added": ["새 보기"]}})

    def test_guide_urls_only_for_present_files(self):
        (self.dir / "guide.ja.md").write_text("# デモ\n")
        (self.dir / "guide.ko.md").write_text("# 데모\n")
        self.write(manifest())
        entry, plugins = self.build()
        self.assertEqual(entry["guideUrl"], f"{br.PUBLIC_BASE}/plugins/demo/guide.md")
        self.assertEqual(entry["guideUrls"], {"ja": f"{br.PUBLIC_BASE}/plugins/demo/guide.ja.md",
                                              "ko": f"{br.PUBLIC_BASE}/plugins/demo/guide.ko.md"})
        self.assertEqual(sorted(k for _, k in plugins[0]["_docs"]),
                         ["plugins/demo/guide.ja.md", "plugins/demo/guide.ko.md", "plugins/demo/guide.md"])

    def test_guide_unsupported_language_file_rejected(self):
        (self.dir / "guide.jp.md").write_text("x")
        self.write(manifest())
        self.assertIn("unsupported guide language 'jp'", self.fails())

    def test_translated_guide_without_english_guide_rejected(self):
        (self.dir / "guide.ja.md").write_text("x")
        self.write(manifest(guide=None))
        self.assertIn("has no `guide`", self.fails())

    def test_translated_guide_size_cap(self):
        (self.dir / "guide.th.md").write_bytes(b"x" * (br.MAX_GUIDE_BYTES + 1))
        self.write(manifest())
        self.assertIn("guide.th.md exceeds", self.fails())

    def test_invalid_i18n_fails_build(self):
        self.write(manifest(i18n={"ja": {"captions": ["a", "b"]}}))
        self.assertIn("media has 1", self.fails())

    def test_testing_override_inherits_presentation_not_stable_changelog(self):
        base = manifest(i18n={"ja": {"name": "デモ", "changelog": {"added": ["安定版"]}}})
        self.write(base, testing={"version": "1.1.0", "minModSystemVersion": "2.0.0",
                                  "changelog": {"fixed": ["beta fix"]}})
        entry, _ = self.build()
        by_ver = {v["version"]: v for v in entry["versions"]}
        self.assertEqual(entry["i18n"], {"ja": {"name": "デモ"}})
        self.assertEqual(by_ver["1.0.0"]["changelogI18n"], {"ja": {"added": ["安定版"]}})
        self.assertNotIn("changelogI18n", by_ver["1.1.0"])

    def test_testing_override_own_changelog_i18n(self):
        self.write(manifest(), testing={"version": "1.1.0", "minModSystemVersion": "2.0.0",
                                        "changelog": {"fixed": ["beta fix"]},
                                        "i18n": {"ko": {"changelog": {"fixed": ["베타 수정"]}}}})
        entry, _ = self.build()
        by_ver = {v["version"]: v for v in entry["versions"]}
        self.assertEqual(by_ver["1.1.0"]["changelogI18n"], {"ko": {"fixed": ["베타 수정"]}})
        self.assertNotIn("changelogI18n", by_ver["1.0.0"])
        self.assertNotIn("i18n", entry)

    def test_testing_override_may_not_set_presentation(self):
        self.write(manifest(), testing={"version": "1.1.0", "minModSystemVersion": "2.0.0",
                                        "i18n": {"ko": {"name": "x"}}})
        self.assertIn("may only carry", self.fails())


_sv_spec = importlib.util.spec_from_file_location("set_version", pathlib.Path(__file__).with_name("set-version.py"))
sv = importlib.util.module_from_spec(_sv_spec); _sv_spec.loader.exec_module(sv)

class SetVersionI18n(unittest.TestCase):
    def test_overridable_lists_stay_in_sync(self):
        self.assertEqual(set(sv.OVERRIDABLE), set(br.OVERRIDABLE) - {"dependencies"})
    def test_promote_replaces_translated_changelog_keeps_presentation(self):
        stable = {"ja": {"name": "N", "changelog": {"added": ["old"]}}, "ko": {"changelog": {"added": ["old"]}}}
        self.assertEqual(sv.promoted_i18n(stable, {"ko": {"changelog": {"fixed": ["new"]}}}, True),
                         {"ja": {"name": "N"}, "ko": {"changelog": {"fixed": ["new"]}}})
    def test_promote_without_testing_changelog_keeps_stable_translation(self):
        stable = {"ja": {"name": "N", "changelog": {"added": ["old"]}}}
        self.assertEqual(sv.promoted_i18n(stable, None, False), stable)
    def test_manifests_written_as_readable_utf8(self):
        self.assertIn("포토 스튜디오", sv.dump({"i18n": {"ko": {"name": "포토 스튜디오"}}}))


if __name__ == "__main__":
    unittest.main()
