using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;		
#endif

namespace CDK {
	public class CGameSettings : ScriptableObject {

		#region <<---------- Properties ---------->>

		public const float MAX_NAVMESH_FINDPOSITION_DISTANCE = 1000f;
		public const float ANGLE_TO_BEGIN_SLIDING = 80;

		public static CGameSettings get {
			get {
				if (_instance == null) {
					_instance = Resources.Load<CGameSettings>("GameSettings");
				}
				return _instance;
			}
		}
		private static CGameSettings _instance;

		public bool CursorStartsHidden { get { return this._cursorStartsHidden; } }
		[SerializeField] private bool _cursorStartsHidden;
		
		public LayerMask LineOfSightBlockingLayers { get { return this._lineOfSightBlockingLayers; } }
		[SerializeField] private LayerMask _lineOfSightBlockingLayers = 1;

		public CanvasGroup FadeCanvasGroupPrefab { get { return this._fadeCanvasGroupPrefab; } }
		[SerializeField] private CanvasGroup _fadeCanvasGroupPrefab;
		
		public long DiscordClientId { get { return this._discordClientId; } }
		[HideInInspector] private long _discordClientId;
		
		#endregion <<---------- Properties ---------->>
		
		
		
		
		#if UNITY_EDITOR
		
		[MenuItem("Tools/Open GameSettings")]
		private static void OpenGameSettingsData() {
			var gameSettings = get;
			if (gameSettings == null) {
				gameSettings = EditorCreateGameSettingsResourceIfNeeded();
			}
			Selection.activeObject = gameSettings;
			AssetDatabase.OpenAsset(gameSettings);
		}

		public static CGameSettings EditorCreateGameSettingsResourceIfNeeded() {
			var gameSettingsScriptObj = Resources.Load<CGameSettings>("GameSettings");
			if (gameSettingsScriptObj != null) return gameSettingsScriptObj;
			gameSettingsScriptObj = ScriptableObject.CreateInstance<CGameSettings>();
			AssetDatabase.CreateAsset(gameSettingsScriptObj, "Assets/Resources/GameSettings.asset");
			return gameSettingsScriptObj;
		}
		
		#endif
		
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(CGameSettings))]
	public class CGameSettingsEditor : Editor {
		public override void OnInspectorGUI() {
			if (!(this.target is CGameSettings myScript)) return;
			base.OnInspectorGUI();

			EditorGUILayout.PasswordField("Discord Client Id", myScript.DiscordClientId.ToString());
		}

	}
	#endif
}
