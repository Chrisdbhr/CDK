using UnityEditor;

namespace CDK.Editor {
    /// <summary>
    /// Adds a menu item that opens the Package Manifest json, bellow the Package Manager button.
    /// </summary>
    public class COpenManifestShortcut {

        [MenuItem("Window/Package Manifest", priority = 1501)]
        static void OpenPackageManifest() {
            EditorUtility.OpenWithDefaultApp("Packages/manifest.json");
        }

    }
}