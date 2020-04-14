using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;		
#endif

namespace CDK {
	public class CGameSettings : ScriptableObject {

		public static CGameSettings get {
			get {
				if (_instance == null) {
					_instance = Resources.Load<CGameSettings>("GameSettings");
				}

				return _instance;
			}
		}
		private static CGameSettings _instance;

		public const float MAX_NAVMESH_FINDPOSITION_DISTANCE = 1000f;
		public const float ANGLE_TO_BEGIN_SLIDING = 80;
		public LayerMask LineOfSightBlockingLayers = 1;
		public LayerMask AttackableLayers = 1;
		public LayerMask WalkableLayers;
		public CanvasGroup FadeCanvasGroupPrefab;

		
		
		
		#if UNITY_EDITOR
		[MenuItem("Tools/Open GameSettings")]
		private static void OpenGameSettingsData() {
			var gameSettings = get;
			if (gameSettings == null) {
				Debug.LogError("Could not open Game Settings data. Create a GameSettings scriptable object inside a Resources folder.");
				return;
			}
			Selection.activeObject = gameSettings;
			AssetDatabase.OpenAsset(gameSettings);
		}
		#endif
	}
}