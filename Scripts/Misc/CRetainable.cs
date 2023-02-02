using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CRetainable {

		#region <<---------- Properties and Fields ---------->>
        
        public bool IsRetained => this._retainedObjectsRx.Count > 0;
		private ReactiveProperty<bool> _isRetainedRx;

        public IReadOnlyCollection<object> RetainedObjectsCollection => _retainedObjectsRx;
        private ReactiveCollection<object> _retainedObjectsRx;

        private CompositeDisposable _disposables;

		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- Initializers ---------->>

		public CRetainable() {
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();
            this._retainedObjectsRx = new ReactiveCollection<object>();
			this._isRetainedRx = new ReactiveProperty<bool>();

            this._retainedObjectsRx.ObserveCountChanged(true)
            .Subscribe(count => {
                this._isRetainedRx.Value = count > 0;
            }).AddTo(this._disposables);

            Object uObject;
            Observable.EveryLateUpdate().Subscribe(_ => {
                int count = this._retainedObjectsRx.Count;
                for (int i = count - 1; i >= 0; i--) {
                    uObject = this._retainedObjectsRx[i] as Object;
                    if (uObject != null || this._retainedObjectsRx[i] != default) continue;
                    Debug.LogWarning($"Removing null retainable at index {i}/{count}.");
                    this._retainedObjectsRx.RemoveAt(i);
                }
            })
            .AddTo(this._disposables);
        }
		
		#endregion <<---------- Initializers ---------->>
		
		
		
		
		#region <<---------- General ---------->>
		
		public void Retain(object source) {
            if (source is UnityEngine.Component uComp) {
                uComp.OnDestroyAsObservable().Subscribe(_ => {
                    if (CApplication.IsQuitting) return;
                    this._retainedObjectsRx.Remove(uComp);
                });
            }
            if (this._retainedObjectsRx.Contains(source)) return;
            this._retainedObjectsRx.Add(source);
		}

		public void Release(object source) {
           if (this._retainedObjectsRx.Remove(source)) return;
           //Debug.LogWarning($"Tried to remove a Release source that was not in retainedObjects list.");
        }
        
        #endregion <<---------- General ---------->>




        #region <<---------- Observables ---------->>

        public IObservable<bool> IsRetainedAsObservable() {
            return this._isRetainedRx.DistinctUntilChanged().AsObservable();
        }

        #endregion <<---------- Observables ---------->>

    }
}
