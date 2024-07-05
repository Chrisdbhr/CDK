using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CRetainable {

		#region <<---------- Properties and Fields ---------->>

        public bool IsRetained {
            get {
                UpdateRetainedState();
                return this._isRetainedRx.Value;
            }
        }

        private ReactiveProperty<bool> _isRetainedRx;

        public IReadOnlyCollection<object> RetainedObjectsCollection => _retainedObjects;
        private List<object> _retainedObjects;

        private CompositeDisposable _disposables;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Initializers ---------->>

		public CRetainable() {
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();
            this._retainedObjects = new List<object>();
			this._isRetainedRx = new ReactiveProperty<bool>();
        }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- General ---------->>

		public void Retain(object source) {
            if (this._retainedObjects.Contains(source)) return;
            this._retainedObjects.Add(source);
            UpdateRetainedState();
		}

		public void Release(object source) {
            if (!this._retainedObjects.Remove(source)) {
                //Debug.LogWarning($"Tried to remove a Release source that was not in retainedObjects list.");
                return;
            }
            UpdateRetainedState();
        }

        private void UpdateRetainedState() {
            int count = this._retainedObjects.Count;
            if (count <= 0) {
                this._isRetainedRx.Value = false;
                return;
            }
            for (int i = count - 1; i >= 0; i--) {
                var currentObject = _retainedObjects[i];
                if (currentObject is Object uObject) {
                    if (uObject != null) continue;
                }
                else {
                    if (currentObject != null) continue;
                }
                Debug.LogWarning($"Removing null retainable at index {i}/{count}.");
                this._retainedObjects.RemoveAt(i);
            }
            this._isRetainedRx.Value = _retainedObjects.Count > 0;
        }

        #endregion <<---------- General ---------->>




        #region <<---------- Observables ---------->>

        public Observable<bool> IsRetainedAsObservable() {
            return this._isRetainedRx.DistinctUntilChanged().AsObservable();
        }

        #endregion <<---------- Observables ---------->>

    }
}