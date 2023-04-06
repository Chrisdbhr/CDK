#if FMOD
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CFmodLoadedBankTrigger : StudioBankLoader {
        
        [SerializeField] private GameObject[] SetActiveOnBankLoaded;
        [SerializeField] private UnityEvent OnBanksLoaded;
        private bool _banksLoaded;

        private void Update() {
            if (this._banksLoaded) return;
            if (this.Banks.Count <= 0 || !RuntimeManager.HaveMasterBanksLoaded || !this.Banks.Where(b => !b.CIsNullOrEmpty()).Any(RuntimeManager.HasBankLoaded)) return;
            this._banksLoaded = true;
            this.OnBanksLoaded?.Invoke();
            if (!SetActiveOnBankLoaded.CIsNullOrEmpty()) {
                foreach (var go in SetActiveOnBankLoaded) {
                    if(go == null) continue;
                    go.SetActive(true);
                }
            }
        }
        
    }
}
#endif