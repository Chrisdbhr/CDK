using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CRetainable {
        class ReleaseHelper : IDisposable {
            readonly CRetainable _retainable;
            readonly object _source;
            bool _isDisposed;

            public ReleaseHelper(CRetainable retainable, object source) {
                _retainable = retainable;
                _source = source;
            }

            public void Dispose() {
                if (_isDisposed) return;
                _isDisposed = true;
                _retainable.Release(_source);
            }
        }

		#region <<---------- Properties and Fields ---------->>

        public bool IsRetained {
            get {
                UpdateRetainedState();
                return _isRetained;
            }
            private set {
                if(_isRetained == value) return;
                _isRetained = value;
                StateEvent.Invoke(value);
            }
        }
        [NonSerialized] bool _isRetained;

        public event Action<bool> StateEvent = delegate { };

        #if UNITY_EDITOR
        public IReadOnlyCollection<object> DebugRetainedObjectsCollection => _retainedObjects;
        #endif
        [NonSerialized] List<object> _retainedObjects;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Initializers ---------->>

		public CRetainable() {
            _retainedObjects = new ();
        }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- General ---------->>

		public IDisposable Retain(object source) {
            if (_retainedObjects.Contains(source)) return Disposables.Empty;
            _retainedObjects.Add(source);
            UpdateRetainedState();
            return new ReleaseHelper(this, source);
		}

		public void Release(object source) {
            if (!_retainedObjects.Remove(source)) return;
            UpdateRetainedState();
        }

        void UpdateRetainedState() {
            int count = _retainedObjects.Count;
            if (count <= 0) {
                IsRetained = false;
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
                _retainedObjects.RemoveAt(i);
            }
            IsRetained = _retainedObjects.Count > 0;
        }

        #endregion <<---------- General ---------->>

    }
}