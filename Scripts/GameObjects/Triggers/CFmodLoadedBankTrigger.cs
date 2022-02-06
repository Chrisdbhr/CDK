#if FMOD
using System;
using FMODUnity;
using UnityEngine;

namespace CDK {
	public class CFmodLoadedBankTrigger : MonoBehaviour {
		[SerializeField] private CUnityEvent OnBanksLoaded;
		
		private void Update() {
			if (!RuntimeManager.HaveMasterBanksLoaded) return;
			this.enabled = false;
			this.OnBanksLoaded?.Invoke();
		}
		
		
	}
}
#endif