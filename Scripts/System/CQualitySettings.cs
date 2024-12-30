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
	                const int targetWidth = 1024;
	                const int targetHeight = 768;
              	  	if (Screen.fullScreenMode == FullScreenMode.Windowed) {
	                    bool setWidth = Screen.width < 1024;
	                    bool setHeight = Screen.height < 768;
	                    if(setWidth || setHeight) {
		                    Screen.SetResolution(setWidth ? targetWidth : Screen.width, setHeight ? targetHeight : Screen.height,false);
	                    }
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