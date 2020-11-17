using System;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace CDK {
	public class CPlayerManager : IDisposable {

		#region <<---------- Properties ---------->>
		
		public static CPlayerManager get {
			get { return _instance ?? (_instance = new CPlayerManager()); }
		}
		private static CPlayerManager _instance;


		private readonly List<CGamePlayer> _gamePlayers = new List<CGamePlayer>();
		
		#endregion <<---------- Properties ---------->>

		
		

		#region <<---------- Initializers ---------->>
		
		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			get?.Dispose();
			
			DebugLogConsole.AddCommandInstance("createplayer", "Creates a player with a controlling character or none.", nameof(CreatePlayer), get);
		}

		#endregion <<---------- Initializers ---------->>


		

		#region <<---------- General ---------->>

		public void CreatePlayer(string controllingCharName = null) {
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
				
				var character = handle.Result.GetComponent<CCharacterBase>();
				
				player.AddControllingCharacter(character);
				
				Debug.Log($"Created player {pNumber} controlling character '{controllingCharName}'.");
			};
		}

		#endregion <<---------- General ---------->>
		
		
		
		
		#region <<---------- Dispose ---------->>
		
		public void Dispose() {
			_instance = null;
		}
		
		#endregion <<---------- Dispose ---------->>

	}
}
