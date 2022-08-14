using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if Newtonsoft_Json_for_Unity
using Newtonsoft.Json;
#endif

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace CDK {
	public abstract class CPersistentData {

		#region <<---------- Saving ---------->>
        
        protected static bool SaveJsonTextToFile(string json, string filePath) {
			try {
                using (var streamWriter = File.CreateText(filePath)) {
                    streamWriter.Write(json);
                }
                return true;
            }
			catch (Exception e) {
				Debug.LogError(e);
                return false;
            }
		}
		
		#endregion <<---------- Saving ---------->>




		#region <<---------- Path ---------->>

        protected static string GetRootSaveFolder() {
            if(Debug.isDebugBuild) Debug.Log($"Application.persistentDataPath: '{Application.persistentDataPath}'");
            return Application.persistentDataPath; //Path.Combine(Application.persistentDataPath, "idbfs");
        }

        #endregion <<---------- Path ---------->>


        
        
        #region <<---------- JS External Invocation ---------->>
#if UNITY_WEBGL
        [DllImport("__Internal")]
        protected static extern void WebglSyncFiles();
#endif
        #endregion <<---------- JS External Invocation ---------->>


        
        
		#region <<---------- Editor ---------->>
	
		#if UNITY_EDITOR

		[MenuItem("Game/Open root save folder")]
		public static void OpenSaveFolder() {
			EditorUtility.RevealInFinder(GetRootSaveFolder());
		}
		
		#endif
		
		#endregion <<---------- Editor ---------->>
		
	}
}
