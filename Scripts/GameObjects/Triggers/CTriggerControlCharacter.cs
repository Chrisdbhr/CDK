using System;
using UnityEngine;

namespace CDK {
	public class CTriggerControlCharacter : MonoBehaviour {
		[SerializeField] private CCharacter_Base _characterToControl;
		


		public void AddToPlayer0() {
			this.AddToPlayer(0);
		}

		public void AddToPlayer(int playerNumber = 0) {
			var player = CGamePlayerManager.get.GetPlayerByPlayerNumber(playerNumber);
			player.AddControllingCharacter(_characterToControl);
		}
		
	}
}
