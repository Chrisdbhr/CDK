using System.Diagnostics;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CDK {
    public class CBuildProcessMethods : IPostprocessBuildWithReport {

        public int callbackOrder => 1;
        
        public void OnPostprocessBuild(BuildReport report) {
            if (report.summary.result != BuildResult.Succeeded && report.summary.result != BuildResult.Unknown) return;
            CEditorPlayerSettings.RaiseBuildVersion();
            var outputPath = report.summary.outputPath.Replace($"/{Application.productName}.exe", string.Empty);
            // Create Shortcut to application Logs Directory
            CreateShortcut(outputPath);
        }

        static void CreateShortcut(string outputPath)
        {
            string targetPath = Path.Combine("%USERPROFILE%\\AppData\\LocalLow", Application.companyName, Application.productName);
            string shortcutPath = Path.Combine(outputPath, "Logs and Save Directory.lnk");

            string cmd = $"/c powershell -Command \"$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('{shortcutPath}'); $Shortcut.TargetPath = '{targetPath}'; $Shortcut.Save()\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = cmd,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                UnityEngine.Debug.Log($"Shortcut creation output: {output}");
            }
        }

    }
}