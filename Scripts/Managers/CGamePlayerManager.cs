using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CDK {
	public class CGamePlayerManager {

		#region <<---------- Properties ---------->>

		private readonly List<CGamePlayer> _gamePlayers = new List<CGamePlayer>();

		#endregion <<---------- Properties ---------->>

		

		
		#region <<---------- Player ---------->>

		public async Task<CGamePlayer> CreatePlayer() {
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

		public CGamePlayer GetPlayerControllingCharacter(CCharacterBase characterBase) {
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
