using UnityEngine;
	
#if UNITY_EDITOR
using System.IO;
using UnityEditor;		
#endif

#if FMOD
using FMODUnity;
#endif

namespace CDK {
	public class CGameSettings : ScriptableObject {
    
        #region <<---------- Singleton ---------->>

        public static CGameSettings get {
            get {
                if (CSingletonHelper.CannotCreateAnyInstance() || _instance != null) return _instance;
                return (_instance = Resources.Load<CGameSettings>("GameSettings"));
            }
        }
        private static CGameSettings _instance;

        #endregion <<---------- Singleton ---------->>
        
       
		
		
		#region <<---------- Properties ---------->>

        private const string GAME_SETTINGS_ASSET_PATH = "Assets/Resources/GameSettings.asset";
        public const string AssetRef_PauseMenu = "Prefabs/UI/menus/ui-pausemenu";
		public const string AssetRef_ConfirmationPopup = "Prefabs/UI/menus/ui-confirm popup";
		
		public bool CursorStartsHidden => this._cursorStartsHidden;
		[SerializeField] protected bool _cursorStartsHidden;

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

		[MenuItem("Tools/Open GameSettings")]
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
