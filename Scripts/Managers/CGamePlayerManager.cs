using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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


		

		#region <<---------- Player ---------->>

		public async Task<CGamePlayer> CreatePlayer() {
			if(_rewiredLoadTask != null) await _rewiredLoadTask;
			
			var pNumber = this._gamePlayers.Count;
			var player = new CGamePlayer(pNumber);
			this._gamePlayers.Add(player);
			return player;
		}
		
		public CGamePlayer GetPlayerByPlayerNumber(int playerNumber) {
			return this._gamePlayers.FirstOrDefault(x => x.PlayerNumber == playerNumber);
		}
		
		#endregion <<---------- Player ---------->>


		
		
		#region <<---------- Characters Managment ---------->>
		
		public List<GameObject> GetAllGameObjectsRelatedToCharacter(CCharacterBase characterBase) {
			foreach (var player in this._gamePlayers) {
				if (!player.IsControllingCharacter(characterBase)) continue;
				return player.GetAllRelatedGameObjects();
			}
			return new List<GameObject>();
		}
		
		public bool IsTransformFromAPlayerCharacter(Transform aTransform) {
			if (!aTransform.gameObject.activeInHierarchy) return false;
			return this.IsGameObjectAPlayerCharacter(aTransform.gameObject);
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

		#endregion <<---------- Characters Managment ---------->>
		
		
		
		
		#region <<---------- Dispose ---------->>
		
		public void Dispose() {
			_instance = null;
		}
		
		#endregion <<---------- Dispose ---------->>

	}
}
