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

if __name__ == "__main__":
    unittest.main()
