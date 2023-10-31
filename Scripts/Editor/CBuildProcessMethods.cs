using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace CDK {
    public class CBuildProcessMethods : IPostprocessBuildWithReport {

        public int callbackOrder => 0;
        
        public void OnPostprocessBuild(BuildReport report) {
            if (report == null || report.summary.result != BuildResult.Succeeded) return;
            CPlayerSettings.RaiseBuildVersion();
        }

    }
}