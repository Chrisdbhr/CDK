using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;		
#endif

namespace CDK {
	[CreateAssetMenu(fileName = "GameSettings", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Game Settings", order = 1001)]
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
		public bool HideCursorInGame = false;
		public bool isGame2Dimension = true;
		public LayerMask LineOfSightBlockingLayers = 1;
		public LayerMask AttackableLayers = 1;
		public LayerMask WalkableLayers = 1;
		public CanvasGroup FadeCanvasGroupPrefab;
		[HideInInspector] public long DiscordClientId;
		
		
		
		private void OnEnable() {
			if (!this.FadeCanvasGroupPrefab) this.FadeCanvasGroupPrefab = Resources.Load<CanvasGroup>("CDK/Resources/FadeCanvas");
		}

		#if UNITY_EDITOR
		[MenuItem("Tools/Open GameSettings")]
		private static void OpenGameSettingsData() {
			var gameSettings = get;
			if (gameSettings == null) {
				gameSettings = CDK.CreateGameSettingsResourceIfNeeded();
			}
			Selection.activeObject = gameSettings;
			AssetDatabase.OpenAsset(gameSettings);
		}
		#endif
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(CGameSettings))]
	public class CGameSettingsEditor : Editor {
		public override void OnInspectorGUI() {
			if (!(this.target is CGameSettings myScript)) return;
			base.OnInspectorGUI();

			myScript.DiscordClientId = Convert.ToInt64(EditorGUILayout.PasswordField("Discord Client Id", myScript.DiscordClientId.ToString()));
		}

	}
	#endif
}
