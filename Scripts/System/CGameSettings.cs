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
	public class CGameSettings : ScriptableObject
	{

		#region Singleton

		public static CGameSettings Instance {
			get {
				if (_instance != null) return _instance;
				_instance = Resources.Load<CGameSettings>("GameSettings");
				#if UNITY_EDITOR
				if (_instance == null) {
					_instance = EditorCreateGameSettingsAsset();
				}
				#endif
				return _instance;
			}
		}
		static CGameSettings _instance;

		#endregion Singleton


		#region <<---------- Properties ---------->>

		static string DefaultFullPath => DefaultFolder + DefaultName;
		static string DefaultFolder => Application.dataPath + "/Resources/";
		const string DefaultName = "GameSettings.asset";

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
				gameSettings = EditorCreateGameSettingsAsset();
			}
			Selection.activeObject = gameSettings;
			AssetDatabase.OpenAsset(gameSettings);
		}

		public static CGameSettings EditorTryLoadingGameSettings() {
            if(!Application.isPlaying) AssetDatabase.Refresh();
			var gameSettingsScriptObj = Resources.Load<CGameSettings>("GameSettings");
			if (gameSettingsScriptObj != null) return gameSettingsScriptObj;
			return AssetDatabase.LoadAssetAtPath<CGameSettings>(DefaultFullPath);
		}

		public static CGameSettings EditorCreateGameSettingsAsset() {
            if(!Directory.Exists(DefaultFolder)) Directory.CreateDirectory(DefaultFolder);
			var gameSettingsScriptObj = CreateInstance<CGameSettings>();
			AssetDatabase.CreateAsset(gameSettingsScriptObj, "Assets/Resources/" + DefaultName);
            Debug.Log($"Created Default GameSettings at path: '{DefaultFullPath}'", gameSettingsScriptObj);
            return gameSettingsScriptObj;
		}
		
		#endif

		#endregion <<---------- Editor ---------->>
	
	}
	
}