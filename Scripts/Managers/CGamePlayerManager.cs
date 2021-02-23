using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IngameDebugConsole;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace CDK {
	public class CGamePlayerManager : IDisposable {

		#region <<---------- Properties ---------->>
		
		public static CGamePlayerManager get {
			get { return _instance ?? (_instance = new CGamePlayerManager()); }
		}
		private static CGamePlayerManager _instance;


		private readonly List<CGamePlayer> _gamePlayers = new List<CGamePlayer>();

		private static AsyncOperationHandle<GameObject> _rewiredLoaded;
		
		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- Initializers ---------->>
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			get?.Dispose();

			_rewiredLoaded = Addressables.LoadAssetAsync<GameObject>("RewiredInputManager");
			_rewiredLoaded.Completed += handle => {
				Object.Instantiate(handle.Result);
			};

			DebugLogConsole.AddCommandInstance("createplayer", "Creates a player with a controlling character or none.", nameof(CreatePlayer), get);
		}

		#endregion <<---------- Initializers ---------->>


		

		#region <<---------- General ---------->>

		public async Task CreatePlayer(string controllingCharName = null) {

			await _rewiredLoaded.Task;
			
			var pNumber = this._gamePlayers.Count;
			var player = new CGamePlayer(pNumber);
			this._gamePlayers.Add(player);

			if (controllingCharName.CIsNullOrEmpty()) {
				Debug.LogWarning($"Created player {pNumber} with no controlling character.");
				return;
			}

			Addressables.LoadAssetAsync<GameObject>(controllingCharName).Completed += handle => {
				if (handle.Result == null) {
					Debug.LogWarning($"Created player {pNumber} but cant find character '{controllingCharName}' to control.");
					return;
				}

				var createdGo = Object.Instantiate(handle.Result);
				createdGo.name = $"[Character] {controllingCharName}";
				createdGo.transform.Translate(0f,0.0001f,0f); // prevent spawning at 0 position so engine does not think it is maybe inside the ground next frame.
				var character = createdGo.GetComponent<CCharacterBase>();

				if (character == null) {
					Debug.LogError($"{controllingCharName} gameobject doesnt have a {nameof(CCharacterBase)} component on it! could not create player!");
					return;
				}
				
				player.AddControllingCharacter(character);
				
				Debug.Log($"Created player {pNumber} controlling character '{controllingCharName}'.");
			};
		}

		#endregion <<---------- General ---------->>



		
		#region <<---------- Characters Managment ---------->>

		public List<GameObject> GetAllGameObjectsRelatedToCharacter(CCharacterBase characterBase) {
			foreach (var player in this._gamePlayers) {
				if (!player.IsControllingCharacter(characterBase)) continue;
				return player.GetAllRelatedGameObjects();
			}
			return new List<GameObject>();
		}
		
		#endregion <<---------- Characters Managment ---------->>
		
		
		
		
		#region <<---------- Dispose ---------->>
		
		public void Dispose() {
			_instance = null;
		}
		
		#endregion <<---------- Dispose ---------->>

	}
}
