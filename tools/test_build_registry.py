import importlib.util, pathlib, unittest

_spec = importlib.util.spec_from_file_location("build_registry", pathlib.Path(__file__).with_name("build-registry.py"))
br = importlib.util.module_from_spec(_spec); _spec.loader.exec_module(br)

def dep(**kw):
    d = {"id": "fx", "name": "FX", "version": "1.0.0", "url": "https://cdn.example/fx.dll",
         "sha256": "a" * 64, "size": 1000, "kind": "file", "files": [{"to": "dxgi.dll"}],
         "target": "game", "license": "BSD-3-Clause"}
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

if __name__ == "__main__":
    unittest.main()
