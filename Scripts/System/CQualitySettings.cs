using System;
using System.Collections;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace CDK {
    public static class CQualitySettings {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitializeAfterSceneLoad() {

			#if UNITY_STANDALONE
			StaticRoutinesRunner.RunRoutine(MonitorResolution());
			IEnumerator MonitorResolution()
        	{
				if(Application.isEditor) yield break;
            	Debug.Log($"Starting monitoring for screen resolution smaller than 1024x768 to resize.");
            	while (true) {
                	yield return null;
              	  	if (Screen.fullScreenMode == FullScreenMode.Windowed && (Screen.width < 1024 || Screen.height < 768)) {
                   	 	Debug.Log($"Setting Screen Resolution to 1024x768 and fullscreen mode to false.");
                    	Screen.SetResolution(1024, 768, false);
         	       	}
           	 	}
        	}
			#endif
        }



        public static bool GetVsync() {
            return QualitySettings.vSyncCount == 1;
        }

        public static void SetVsync(bool value) {
            QualitySettings.vSyncCount = value ? 1 : 0;
        }

    }
}