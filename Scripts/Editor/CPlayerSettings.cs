using System;
using UnityEditor;

namespace CDK {
    public static class CPlayerSettings {

        public static void RaiseBuildVersion() {
            if (!Version.TryParse(PlayerSettings.bundleVersion, out var version)) return;
            PlayerSettings.bundleVersion = new Version(version.Major, version.Minor, version.Build + 1).ToString();
            AssetDatabase.Refresh();
        }
        
    }
}