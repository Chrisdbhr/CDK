using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEditor;

namespace CDK {
	
	[InitializeOnLoad]
	public static class CGameVersionInfoChecker {

		private const string GAME_VERSION_FILE_NAME = "GameVersion";
		private static readonly string TARGET_FOLDER = Application.dataPath + "/Resources";
		static CGameVersionInfoChecker () {
			CheckVersion().CAwait();
		}

		[MenuItem("Tools/Check game version info")]
		private static async Task CheckVersion() {
			var textAsset = (TextAsset)Resources.Load(GAME_VERSION_FILE_NAME);
			if (textAsset == null) {
				Directory.CreateDirectory(TARGET_FOLDER);
				WriteFileWithGameVersion();
			}

			await Observable.NextFrame();

			textAsset = (TextAsset)Resources.Load(GAME_VERSION_FILE_NAME);
			//TODO fix error on first project import here.
			
			string lastVersion = textAsset.text;
			if (lastVersion != PlayerSettings.bundleVersion) {
				WriteFileWithGameVersion();
				Debug.Log ($"Found new bundle version {PlayerSettings.bundleVersion}, previous version was {lastVersion}");
			}
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void WriteFileWithGameVersion () {
			File.WriteAllText($"{TARGET_FOLDER}/{GAME_VERSION_FILE_NAME}.txt", PlayerSettings.bundleVersion);
		}
		
	}		
}