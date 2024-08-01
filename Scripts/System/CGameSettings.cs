using CDK.UI;
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

		#region <<---------- Properties ---------->>

		static string GAME_SETTINGS_ASSET_FULL_PATH => GAME_SETTINGS_ASSET_PATH + GAME_SETTINGS_ASSET_NAME;
		static string GAME_SETTINGS_ASSET_PATH => Application.dataPath + "/Resources/";
		const string GAME_SETTINGS_ASSET_NAME = "GameSettings.asset";

		public CUIViewBase PrefabPauseMenu;
		public CUIViewBase PrefabConfirmationPopup;

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
			if (!gameSettings) {
				gameSettings = EditorCreateGameSettingsResourceIfNeeded();
			}
			Selection.activeObject = gameSettings;
			AssetDatabase.OpenAsset(gameSettings);
		}

		public static CGameSettings EditorTryLoadingGameSettings() {
            AssetDatabase.Refresh();
			var gameSettingsScriptObj = Resources.Load<CGameSettings>("GameSettings");
			if (gameSettingsScriptObj != null) return gameSettingsScriptObj;
			return AssetDatabase.LoadAssetAtPath<CGameSettings>(GAME_SETTINGS_ASSET_FULL_PATH);
		}

		public static CGameSettings EditorCreateGameSettingsResourceIfNeeded() {
			var gameSettingsScriptObj = EditorTryLoadingGameSettings();
			if (gameSettingsScriptObj != null) return gameSettingsScriptObj;

			Debug.Log($"Cant find GameSettings asset, will try to create at: '{GAME_SETTINGS_ASSET_FULL_PATH}'");

            if(!Directory.Exists(GAME_SETTINGS_ASSET_PATH)) Directory.CreateDirectory(GAME_SETTINGS_ASSET_PATH);

			gameSettingsScriptObj = CreateInstance<CGameSettings>();

			AssetDatabase.CreateAsset(gameSettingsScriptObj, "Assets/Resources/" + GAME_SETTINGS_ASSET_NAME);
			
            Debug.Log($"Created GameSettings scriptable object at path: '{GAME_SETTINGS_ASSET_FULL_PATH}'");
			
            return gameSettingsScriptObj;
		}
		
		#endif

		#endregion <<---------- Editor ---------->>
	
	}
	
}