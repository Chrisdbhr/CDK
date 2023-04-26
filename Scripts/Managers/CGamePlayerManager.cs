using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDK {
	public class CGamePlayerManager {

        #region <<---------- Singleton ---------->>
        
        public static CGamePlayerManager get {
            get {
                if (CSingletonHelper.CannotCreateAnyInstance() || _current != null) return _current;
                return _current = new CGamePlayerManager();
            }
        }
        private static CGamePlayerManager _current;
        
        #endregion <<---------- Singleton ---------->>


        

        #region <<---------- Initializers ---------->>

        private CGamePlayerManager() {
            
        }

        #endregion <<---------- Initializers ---------->>


        
        
		#region <<---------- Properties ---------->>

		private readonly List<CGamePlayer> _gamePlayers = new List<CGamePlayer>();

		#endregion <<---------- Properties ---------->>




		#region <<---------- Player ---------->>
        
		public CGamePlayer CreatePlayer() {
			var pNumber = this._gamePlayers.Count;
            Debug.Log($"Creating Game Player number {pNumber}");
			var player = new CGamePlayer(pNumber);
			this._gamePlayers.Add(player);
			return player;
		}
		
		public CGamePlayer GetPlayerByPlayerNumber(int playerNumber) {
			return this._gamePlayers.FirstOrDefault(x => x.PlayerNumber == playerNumber);
		}

        public void DestroyAllPlayers() {
            Debug.Log($"Removing and destroying all '{this._gamePlayers.Count}' players.");
            foreach (var player in this._gamePlayers) {
                player.RemoveAndDestroyAllControllingCharactersAndCameras();   
            }
            foreach (var player in this._gamePlayers) {
                player?.Dispose();
            }
            this._gamePlayers.Clear();
        }
        
		#endregion <<---------- Player ---------->>




		#region <<---------- Characters Managment ---------->>
		
		public List<GameObject> GetAllGameObjectsRelatedToCharacter(CCharacter_Base characterBase) {
			foreach (var player in this._gamePlayers) {
				if (!player.IsControllingCharacter(characterBase)) continue;
				return player.GetAllRelatedGameObjects();
			}
			return new List<GameObject>();
		}

		public CGamePlayer GetPlayerControllingCharacter(CCharacter_Base characterBase) {
			return this._gamePlayers.FirstOrDefault(player => player.IsControllingCharacter(characterBase));
		}
        
        public CGamePlayer GetPlayerFromTransform(Transform transformToCheck) {
            return this._gamePlayers.FirstOrDefault(player => player.IsTransformRelatedToPlayer(transformToCheck));
        }
		
		public bool IsTransformFromAPlayerCharacter(Transform aTransform) {
            if(aTransform == null) return false;
			if(!aTransform.gameObject.activeInHierarchy) return false;
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

	}
}
