using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;		
#endif

namespace CDK {
	public class CGameSettings : ScriptableObject {

		#region <<---------- Constants ---------->>

		private const string GAME_SETTINGS_ASSET_PATH = "Assets/Resources/GameSettings.asset";
		
		public const float MAX_NAVMESH_FINDPOSITION_DISTANCE = 1000f;
		
		#endregion <<---------- Constants ---------->>
		
		
		
		
		#region <<---------- Properties ---------->>
		
		public AssetReference AssetRef_UiLoading => this._assetRefUiLoading;
		[Header("Asset References")]
		[SerializeField] protected AssetReference _assetRefUiLoading;

		public AssetReference AssetRef_PauseMenu => this._assetRefPauseMenu;
		[SerializeField] protected AssetReference _assetRefPauseMenu;
		
		public AssetReference AssetRef_ConfirmationPopup => this._assetRefConfirmationPopup;
		[SerializeField] protected AssetReference _assetRefConfirmationPopup;
		
		public bool CursorStartsHidden => this._cursorStartsHidden;
		[SerializeField] protected bool _cursorStartsHidden;

		public LayerMask LineOfSightBlockingLayers => this._lineOfSightBlockingLayers;
		[SerializeField] protected LayerMask _lineOfSightBlockingLayers = 1;


		[Header("Default Sounds")]
		[SerializeField] [EventRef] protected string _soundSelect;
		public string SoundSelect => this._soundSelect;
		
		[SerializeField] [EventRef] protected string _soundSubmit;
		public string SoundSubmit => this._soundSubmit;
		
		[SerializeField] [EventRef] protected string _soundCancel;
		public string SoundCancel => this._soundCancel;

		[SerializeField] [EventRef] protected string _soundOpenMenu;
		public string SoundOpenMenu => this._soundOpenMenu;

		[SerializeField] [EventRef] protected string _soundCloseMenu;
		public string SoundCloseMenu => this._soundCloseMenu;

		
		#endregion <<---------- Properties ---------->>


		

		#region <<---------- Editor ---------->>
		
		#if UNITY_EDITOR

		[MenuItem("Game/Open GameSettings")]
		public static void EditorOpenGameSettingsData() {
			var gameSettings = EditorTryLoadingGameSettings();
			if (!gameSettings) EditorCreateGameSettingsResourceIfNeeded();
			Selection.activeObject = gameSettings;
			AssetDatabase.OpenAsset(gameSettings);
		}

		public static CGameSettings EditorTryLoadingGameSettings() {
			var gameSettingsScriptObj = Resources.Load<CGameSettings>("GameSettings");
			if (gameSettingsScriptObj != null) return gameSettingsScriptObj;

			gameSettingsScriptObj = AssetDatabase.LoadAssetAtPath<CGameSettings>(GAME_SETTINGS_ASSET_PATH);
			return gameSettingsScriptObj;
		}

		public static CGameSettings EditorCreateGameSettingsResourceIfNeeded() {
			var gameSettingsScriptObj = EditorTryLoadingGameSettings();
			if (gameSettingsScriptObj != null) return gameSettingsScriptObj;
			
			gameSettingsScriptObj = ScriptableObject.CreateInstance<CGameSettings>();
			AssetDatabase.CreateAsset(gameSettingsScriptObj, GAME_SETTINGS_ASSET_PATH);
			Debug.Log($"Created GameSettings scriptable object at path: '{GAME_SETTINGS_ASSET_PATH}'");
			return gameSettingsScriptObj;
		}
		
		#endif

		#endregion <<---------- Editor ---------->>
	
	}
	
}
