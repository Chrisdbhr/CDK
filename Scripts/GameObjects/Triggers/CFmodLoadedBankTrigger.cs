#if FMOD
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CFmodLoadedBankTrigger : StudioBankLoader {
        
        [SerializeField] GameObject[] SetActiveOnBankLoaded;
        [SerializeField] UnityEvent OnBanksLoaded;
        bool _banksLoaded;

        void Update() {
            if (_banksLoaded) return;
            if (Banks.Count <= 0 || !RuntimeManager.HaveMasterBanksLoaded || !Banks.Where(b => !b.CIsNullOrEmpty()).Any(RuntimeManager.HasBankLoaded)) return;
            _banksLoaded = true;
            OnBanksLoaded?.Invoke();
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