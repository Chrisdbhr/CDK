using System;
using UnityEngine;

namespace CDK {
	public class CTriggerControlCharacter : MonoBehaviour {
		[SerializeField] private CCharacterBase _characterToControl;
		private CGamePlayerManager _gamePlayerManager;
		
		
		private void Awake() {
			_gamePlayerManager = CDependencyResolver.Get<CGamePlayerManager>();
		}

		public void AddToPlayer0() {
			this.AddToPlayer(0);
		}

		public void AddToPlayer(int playerNumber = 0) {
			var player = _gamePlayerManager.GetPlayerByPlayerNumber(playerNumber);
			player.AddControllingCharacter(_characterToControl);
		}
		
	}
}
