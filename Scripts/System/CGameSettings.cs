using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;		
#endif

namespace CDK {
	public class CGameSettings : ScriptableObject {

		#region <<---------- Singleton ---------->>
		
		protected static CGameSettings Instance {
			get {
				if (_instance == null) {
					_instance = Resources.Load<CGameSettings>("GameSettings");
					Debug.Log($"Loaded GameSettings resource.");
				}
				return _instance;
			}
		}
		private static CGameSettings _instance;
		
		#endregion <<---------- Singleton ---------->>


		
		
		#region <<---------- Consts ---------->>
		
		public const float MAX_NAVMESH_FINDPOSITION_DISTANCE = 1000f;

		#endregion <<---------- Consts ---------->>
		
		
		
		
		#region <<---------- Properties ---------->>
		
		public static AssetReference AssetRef_UiLoading => Instance._assetRefUiLoading;
		[Header("Asset References")]
		[SerializeField] protected AssetReference _assetRefUiLoading;

		public static AssetReference AssetRef_PauseMenu => Instance._assetRefPauseMenu;
		[SerializeField] protected AssetReference _assetRefPauseMenu;
		
		public static AssetReference AssetRef_ConfirmationPopup => Instance._assetRefConfirmationPopup;
		[SerializeField] protected AssetReference _assetRefConfirmationPopup;
		
		public static bool CursorStartsHidden => Instance._cursorStartsHidden;
		[SerializeField] protected bool _cursorStartsHidden;

		public static LayerMask LineOfSightBlockingLayers => Instance._lineOfSightBlockingLayers;
		[SerializeField] protected LayerMask _lineOfSightBlockingLayers = 1;


		[Header("Default Sounds")]
		[SerializeField] [EventRef] protected string _soundSelect;
		public static string SoundSelect => Instance._soundSelect;
		
		[SerializeField] [EventRef] protected string _soundSubmit;
		public static string SoundSubmit => Instance._soundSubmit;
		
		[SerializeField] [EventRef] protected string _soundCancel;
		public static string SoundCancel => Instance._soundCancel;

		[SerializeField] [EventRef] protected string _soundOpenMenu;
		public static string SoundOpenMenu => Instance._soundOpenMenu;

		[SerializeField] [EventRef] protected string _soundCloseMenu;
		public static string SoundCloseMenu => Instance._soundCloseMenu;

		
		#endregion <<---------- Properties ---------->>


		

		#region <<---------- Editor ---------->>
		
		#if UNITY_EDITOR

		[MenuItem("Game/Open GameSettings")]
		public static void OpenGameSettingsData() {
			var gameSettings = Instance;
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
			var path = "Assets/Resources/GameSettings.asset";
			AssetDatabase.CreateAsset(gameSettingsScriptObj, path);
			Debug.Log($"Created GameSettings scriptable object at path: '{path}'");
			return gameSettingsScriptObj;
		}
		
		#endif

		#endregion <<---------- Editor ---------->>
	
	}
	
}
