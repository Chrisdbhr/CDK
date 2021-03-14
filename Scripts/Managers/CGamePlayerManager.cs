using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CDK {
	public class CGamePlayerManager : IDisposable {

		#region <<---------- Properties ---------->>
		
		public static CGamePlayerManager get {
			get { return _instance ??= new CGamePlayerManager(); }
		}
		private static CGamePlayerManager _instance;


		private readonly List<CGamePlayer> _gamePlayers = new List<CGamePlayer>();

		private static Task _rewiredLoadTask;
		
		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- Initializers ---------->>
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			get?.Dispose();

			if (!Rewired.ReInput.isReady && GameObject.FindObjectOfType<Rewired.InputManager_Base>() == null) {
				_rewiredLoadTask = CAssets.LoadAndInstantiateGameObjectAsync("Rewired Input Manager");
			}
		}

		#endregion <<---------- Initializers ---------->>


		

		#region <<---------- General ---------->>

		public async Task CreatePlayer(AssetReference charToCreate = null) {
			if(_rewiredLoadTask != null) await _rewiredLoadTask;
			
			var pNumber = this._gamePlayers.Count;
			var player = new CGamePlayer(pNumber);
			this._gamePlayers.Add(player);

			if (charToCreate == null || !charToCreate.RuntimeKeyIsValid()) {
				Debug.LogWarning($"Created player {pNumber} with no controlling character.");
				return;
			}

			var createdGo = await CAssets.LoadAndInstantiateGameObjectAsync(charToCreate.RuntimeKey.ToString());
			if (createdGo == null) {
				Debug.LogWarning($"Created player {pNumber} but cant find character '{charToCreate}' to control.");
				return;
			}

			createdGo.name = $"[Character] {charToCreate}";
			createdGo.transform.Translate(0f,0.0001f,0f); // prevent spawning at 0 position so engine does not think it is maybe inside the ground next frame.
			var character = createdGo.GetComponent<CCharacterBase>();

			if (character == null) {
				Debug.LogError($"{charToCreate} gameobject doesnt have a {nameof(CCharacterBase)} component on it! could not create player!");
				return;
			}
				
			await player.AddControllingCharacter(character);
				
			Debug.Log($"Created player {pNumber} controlling character '{charToCreate}'.");
		}

		public bool IsRootTransformFromAPlayerCharacter(Transform aTransform) {
			var gameObjectToCheck = aTransform.root.gameObject;
			if (!gameObjectToCheck.activeInHierarchy) return false;
			return this.IsGameObjectAPlayerCharacter(gameObjectToCheck);
		}
		
		public bool IsGameObjectAPlayerCharacter(GameObject gameObjectToCheck) {
			foreach (var player in this._gamePlayers) {
				var playerCharacter = player.GetControllingCharacter();
				if (playerCharacter == null) {
					Debug.LogWarning($"Player {player.PlayerNumber} method '{nameof(CGamePlayer.GetControllingCharacter)}' returned null!");
					continue;
				}
				if (playerCharacter.gameObject == gameObjectToCheck) return true;
			}
			return false;
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
