using UnityEngine;
	
#if UnityAddressables
using UnityEngine.AddressableAssets;
#endif

#if UNITY_EDITOR
using System.IO;
using UnityEditor;		
#endif

#if FMOD
using FMODUnity;
#endif

namespace CDK {
	public class CGameSettings : ScriptableObject {

		#region <<---------- Constants ---------->>

		private const string GAME_SETTINGS_ASSET_PATH = "Assets/Resources/GameSettings.asset";
        
		#endregion <<---------- Constants ---------->>
		
		
		
		
		#region <<---------- Properties ---------->>

		public string AssetRef_PauseMenu => "Prefabs/UI/menus/ui-pausemenu";
		public string AssetRef_ConfirmationPopup => "Prefabs/UI/menus/ui-confirm popup";
		
		public bool CursorStartsHidden => this._cursorStartsHidden;
		[SerializeField] protected bool _cursorStartsHidden;

		public LayerMask LineOfSightBlockingLayers => this._lineOfSightBlockingLayers;
		[SerializeField] protected LayerMask _lineOfSightBlockingLayers = 1;

		#if FMOD
		[Header("Default Sounds")]
		[SerializeField] protected EventReference _soundSelect;
		public EventReference SoundSelect => this._soundSelect;
		
		[SerializeField] protected EventReference _soundSubmit;
		public EventReference SoundSubmit => this._soundSubmit;
		
		[SerializeField] protected EventReference _soundCancel;
		public EventReference SoundCancel => this._soundCancel;

		[SerializeField] protected EventReference _soundOpenMenu;
		public EventReference SoundOpenMenu => this._soundOpenMenu;

		[SerializeField] protected EventReference _soundCloseMenu;
		public EventReference SoundCloseMenu => this._soundCloseMenu;
		#endif
		
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
			Directory.CreateDirectory(GAME_SETTINGS_ASSET_PATH);
			AssetDatabase.CreateAsset(gameSettingsScriptObj, GAME_SETTINGS_ASSET_PATH);
			Debug.Log($"Created GameSettings scriptable object at path: '{GAME_SETTINGS_ASSET_PATH}'");
			return gameSettingsScriptObj;
		}
		
		#endif

		#endregion <<---------- Editor ---------->>
	
	}
	
}
