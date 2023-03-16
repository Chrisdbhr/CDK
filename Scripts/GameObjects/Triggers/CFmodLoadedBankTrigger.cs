#if FMOD
using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UniRx;
using UnityEngine;

namespace CDK {
	public class CFmodLoadedBankTrigger : MonoBehaviour {
		[SerializeField] private CUnityEvent OnBanksLoaded;
        [SerializeField] [BankRef] private List<string> _otherBankNameToCheck = new List<string>();

        private IDisposable _triggerDisposable;

        private void OnEnable() {
            this._triggerDisposable = Observable.EveryUpdate()
            .Subscribe(_ => {
                if (!RuntimeManager.HaveMasterBanksLoaded) return;
                if (this._otherBankNameToCheck.Count > 0 && !this._otherBankNameToCheck.Where(b => !b.CIsNullOrEmpty()).All(RuntimeManager.HasBankLoaded)) return;
                this.OnBanksLoaded?.Invoke();
                this.enabled = false;
                this._triggerDisposable?.Dispose();
            });
        }

        private void OnDisable() {
            this._triggerDisposable?.Dispose();
        }
    }
}
#endif